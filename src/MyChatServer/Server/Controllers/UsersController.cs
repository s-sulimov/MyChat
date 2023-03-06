using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.Helpers;
using Sulimov.MyChat.Server.Models;
using Sulimov.MyChat.Server.Services;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(string name)
        {
            var result = await userService.GetUser(name);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/users/create
        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await userService.CreateUser(request.Name, request.Email, request.Password);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await userService.Login(request.Login, request.Password);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/users/change-email
        [HttpPatch("change-email")]
        public async Task<ActionResult<User>> ChangeEmail(ChangeUserEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await userService.ChangeEmail(userId, request.Password, request.Email);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/users/change-password
        [HttpPatch("change-password")]
        public async Task<ActionResult<User>> ChangePassword(ChangeUserPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await userService.ChangePassword(userId, request.CurrentPassword, request.NewPassword);

            return ResultHelper.CreateHttpResult(result, this);
        }
    }
}
