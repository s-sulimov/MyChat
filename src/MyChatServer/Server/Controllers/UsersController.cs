using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Helpers;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using Sulimov.MyChat.Server.Services;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IAuthorizationClient authorizationClient;
        private readonly IUserClient userClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        private const string TOKEN_NAME = "access_token";

        public UsersController(
            IUserClient userClient,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationClient authorizationClient)
        {
            this.userClient = userClient;
            this.httpContextAccessor = httpContextAccessor;
            this.authorizationClient = authorizationClient;
        }

        // api/users
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> GetUser(string name)
        {
            var token = await HttpContext.GetTokenAsync(TOKEN_NAME);
            var result = await userClient.GetUser(name, token);

            return ResultHelper.CreateHttpResult<UserDto, UserDto>(this, result);
        }

        // api/users/create
        [HttpPost("create")]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
        {
            var result = await userClient.CreateUser(request.Name, request.Email, request.Password);

            return ResultHelper.CreateHttpResult<UserDto, UserDto>(this, result);
        }

        // api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            var result = await authorizationClient.Login(request);

            if (!result.IsSuccess)
            {
                if (result.Status == ResultStatus.AccessDenied)
                {
                    return Forbid();
                }

                if (result.Status == ResultStatus.InconsistentData)
                {
                    return BadRequest();
                }

                return Problem(statusCode: 500);
            }

            return ResultHelper.CreateHttpResult<AuthenticationResponse, AuthenticationResponse>(this, result);
        }

        // api/users/change-email
        [HttpPatch("change-email")]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> ChangeEmail(ChangeUserEmailRequest request)
        {
            var token = await HttpContext.GetTokenAsync(TOKEN_NAME);
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            var result = await userClient.ChangeEmail(userId, request.Password, request.Email, token);

            return ResultHelper.CreateHttpResult<UserDto, UserDto>(this, result);
        }

        // api/users/change-password
        [HttpPatch("change-password")]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> ChangePassword(ChangeUserPasswordRequest request)
        {
            var token = await HttpContext.GetTokenAsync(TOKEN_NAME);
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            var result = await userClient.ChangePassword(userId, request.CurrentPassword, request.NewPassword, token);

            return ResultHelper.CreateHttpResult<UserDto, UserDto>(this, result);
        }
    }
}
