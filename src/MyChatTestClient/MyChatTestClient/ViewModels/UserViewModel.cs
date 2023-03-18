using Microsoft.AspNetCore.SignalR.Client;
using Sulimov.MyChat.Client.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Sulimov.MyChat.Client.ViewModels
{
    public class UserViewModel
    {
        private readonly HttpClient client;
        private Credentials currentCredentials;

        public Credentials CurrentCredentials { get { return currentCredentials; } }

        public UserViewModel(HttpClient client)
        {
            this.client = client;
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

            this.currentCredentials = new Credentials
            {
                Login = login,
                Password = password,
                Token = responseData.Token,
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.currentCredentials.Token);
            var userResponse = await client.GetAsync($"{Constants.ApiUrl}users?name={data.Login}");

            if (!userResponse.IsSuccessStatusCode)
            {
                return new Result { IsSuccess = false, Message = "Problems with network." };
            }

            var user = await userResponse.Content.ReadFromJsonAsync<User>();
            this.currentCredentials.Id = user.Id;
            this.currentCredentials.Email = user.Email;

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
