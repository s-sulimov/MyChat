using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using Sulimov.MyChat.Server.DAL.Models;
using System.Text;

namespace Sulimov.MyChat.Server.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public AuthentificationService(
            UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<Result<AuthenticationResponse>> Login(string userName, string password)
        {
            DbUser? user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return new Result<AuthenticationResponse>(ResultStatus.ObjectNotFound, $"User {userName} not found.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            var userRoles = await userManager.GetRolesAsync(user);

            var requestObject = new CreateTokenRequest
            {
                UserId = user.Id,
                UserName = user.UserName!,
                UserEmail = user.Email!,
                Claims = userRoles.ToArray(),
            };

            using var requestContent = new StringContent(
                JsonConvert.SerializeObject(requestObject),
                Encoding.UTF8,
                "application/json");

            if (!result.Succeeded)
            {
                return new Result<AuthenticationResponse>(ResultStatus.InconsistentData, "Bad credentials");
            }

            string? authServiceUri = configuration.GetSection("Services").GetValue<string>("Authorization");
            using var response = await this.httpClient.PostAsync($"{authServiceUri}/api/jwt/login", requestContent);

            if (response == null || !response.IsSuccessStatusCode)
            {
                return new Result<AuthenticationResponse>(ResultStatus.UnhandledError, "Can't create token");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<AuthenticationResponse>(responseContent);

            return new Result<AuthenticationResponse>(ResultStatus.Success, responseData!);
        }
    }
}
