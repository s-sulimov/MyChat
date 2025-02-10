using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.Helpers;
using Sulimov.MyChat.Server.Models;
using Sulimov.MyChat.Server.Models.Responses;
using Sulimov.MyChat.Server.Services;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IAuthorizationClient authorizationClient;
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersController(
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationClient authorizationClient)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
            this.authorizationClient = authorizationClient;
        }

        // api/users
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> GetUser(string name)
        {
            var result = await userService.GetUser(name);

            return ResultHelper.CreateHttpResult<User, UserDto>(this, result);
        }

        // api/users/create
        [HttpPost("create")]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await userService.CreateUser(request.Name, request.Email, request.Password);

            return ResultHelper.CreateHttpResult<User, UserDto>(this, result);
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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            var result = await userService.ChangeEmail(userId, request.Password, request.Email);

            return ResultHelper.CreateHttpResult<User, UserDto>(this, result);
        }

        // api/users/change-password
        [HttpPatch("change-password")]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> ChangePassword(ChangeUserPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            var result = await userService.ChangePassword(userId, request.CurrentPassword, request.NewPassword);

            return ResultHelper.CreateHttpResult<User, UserDto>(this, result);
        }
    }
}
