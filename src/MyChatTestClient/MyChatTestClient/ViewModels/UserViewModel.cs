using Sulimov.MyChat.Client.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sulimov.MyChat.Client.ViewModels
{
    public class UserViewModel
    {
        private readonly HttpClient client;
        private Credentials currentCredentials;
        private bool isLoggedIn;

        public Credentials CurrentCredentials { get { return currentCredentials; } }
        public bool IsLoggedIn { get { return isLoggedIn; } }

        public UserViewModel(HttpClient client)
        {
            this.client = client;
            this.currentCredentials = new Credentials();
            isLoggedIn = false;
        }

        public async Task<Result> Register(string userName, string password, string email)
        {
            var data = new
            {
                Name = userName,
                Email = email,
                Password = password,
            };

            var response = await client.PostAsJsonAsync(Constants.ApiUrl + "users/create", data);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = $"User {userName} already exists.",
                };
            }

            return new Result
            {
                IsSuccess = true,
            };
        }

        public async Task<Result> Login(string login, string password)
        {
            var data = new
            {
                Login = login,
                Password = password,
            };

            var response = await client.PostAsJsonAsync($"{Constants.ApiUrl}users/login", data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = "Bad credentials",
                };
            }

            var responseData = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            currentCredentials.Login = login;
            currentCredentials.Password = password;
            currentCredentials.Token = responseData.Token;

            isLoggedIn = true;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.currentCredentials.Token);
            var userResponse = await client.GetAsync($"{Constants.ApiUrl}users?name={data.Login}");

            if (!userResponse.IsSuccessStatusCode)
            {
                return new Result { IsSuccess = false, Message = "Problems with network." };
            }

            var user = await userResponse.Content.ReadFromJsonAsync<User>();
            currentCredentials.Id = user.Id;
            currentCredentials.Email = user.Email;

            return new Result { IsSuccess = true };
        }

        public async Task<User> GetUserInfo(string name)
        {
            var response = await client.GetAsync($"{Constants.ApiUrl}users?name={name}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<User>();
        }
    }
}
