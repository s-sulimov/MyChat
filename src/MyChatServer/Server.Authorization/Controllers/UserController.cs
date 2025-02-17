using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Authorization.Services;
using Sulimov.MyChat.Server.Core.Helpers;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;

namespace Sulimov.MyChat.Server.Authorization.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UsersController(
        IUserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.userService = userService;
        this.httpContextAccessor = httpContextAccessor;
    }

    // api/users/user
    [HttpGet("user")]
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
