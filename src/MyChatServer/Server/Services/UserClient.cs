using Newtonsoft.Json;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Sulimov.MyChat.Server.Services;

/// <inheritdoc />
public class UserClient : IUserClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public UserClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> ChangeEmail(string userId, string password, string email, string? token)
    {
        var requestData = new ChangeUserEmailRequest
        {
            Password = password,
            Email = email,
        };

        string? baseUrl = configuration.GetSection("Services").GetValue<string>("Authorization");
        string authServiceUri = $"{baseUrl}/api/users/change-email";
        string bodyContent = JsonConvert.SerializeObject(requestData);

        return await SendPatchRequest(authServiceUri, bodyContent, token);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> ChangePassword(string userId, string currentPassword, string newPassword, string? token)
    {
        var requestData = new ChangeUserPasswordRequest
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword,
        };

        string? baseUrl = configuration.GetSection("Services").GetValue<string>("Authorization");
        string authServiceUri = $"{baseUrl}/api/users/change-password";
        string bodyContent = JsonConvert.SerializeObject(requestData);

        return await SendPatchRequest(authServiceUri, bodyContent, token);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> CreateUser(string name, string email, string password)
    {
        var requestData = new CreateUserRequest
        {
            Name = name,
            Email = email,
            Password = password,
        };

        string? baseUrl = configuration.GetSection("Services").GetValue<string>("Authorization");
        string authServiceUri = $"{baseUrl}/api/users/create";
        string bodyContent = JsonConvert.SerializeObject(requestData);

        return await SendPostRequest(authServiceUri, bodyContent, null);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> GetUser(string userName, string? token)
    {
        SetToken(token);

        string? baseUrl = configuration.GetSection("Services").GetValue<string>("Authorization");
        string authServiceUri = $"{baseUrl}/api/users/user?name={userName}";

        using var response = await this.httpClient.GetAsync(authServiceUri);

        return await HandleHttpResponse(response);
    }

    private static async Task<Result<UserDto>> HandleHttpResponse(HttpResponseMessage? response)
    {
        if (response == null)
        {
            return new Result<UserDto>(ResultStatus.UnhandledError, "Server error.");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new Result<UserDto>(ResultStatus.InconsistentData, "Bad request.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new Result<UserDto>(ResultStatus.AccessDenied, "Access denied.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return new Result<UserDto>(ResultStatus.UnhandledError, "Unknown error.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<UserDto>(responseContent);

        return new Result<UserDto>(ResultStatus.Success, responseData!);
    }

    private async Task<Result<UserDto>> SendPatchRequest(string url, string body, string? token)
    {
        SetToken(token);

        using var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await this.httpClient.PatchAsync(url, requestContent);

        return await HandleHttpResponse(response);
    }

    private async Task<Result<UserDto>> SendPostRequest(string url, string body, string? token)
    {
        SetToken(token);

        using var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await this.httpClient.PostAsync(url, requestContent);

        return await HandleHttpResponse(response);
    }

    private void SetToken(string? token)
    {
        if (token == null)
        {
            this.httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }

        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
