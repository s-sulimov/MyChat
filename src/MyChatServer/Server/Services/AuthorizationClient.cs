using Newtonsoft.Json;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using System.Net;
using System.Text;

namespace Sulimov.MyChat.Server.Services;

/// <inheritdoc />
public class AuthorizationClient : IAuthorizationClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public AuthorizationClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<Result<AuthenticationResponse>> Login(AuthenticationRequest request)
    {
        using var requestContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        string? authServiceUri = configuration.GetSection("Services").GetValue<string>("Authorization");
        using var response = await this.httpClient.PostAsync($"{authServiceUri}/api/authorization/login", requestContent);

        if (response == null)
        {
            return new Result<AuthenticationResponse>(ResultStatus.UnhandledError, "Can't login.");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new Result<AuthenticationResponse>(ResultStatus.InconsistentData, "Bad request.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new Result<AuthenticationResponse>(ResultStatus.AccessDenied, "Access denied.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<AuthenticationResponse>(responseContent);

        return new Result<AuthenticationResponse>(ResultStatus.Success, responseData!);
    }
}
