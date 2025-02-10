using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Authorization.Services;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;

namespace Sulimov.MyChat.Server.Authorization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthentificationService authentificationService;

    public AuthorizationController(IAuthentificationService authentificationService)
    {
        this.authentificationService = authentificationService;
    }

    // api/authorization/login
    [HttpPost("login")]
    [AllowAnonymous]
    [Produces("application/json")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] AuthenticationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await this.authentificationService.Login(request.Login, request.Password);

        if (!result.IsSuccess && result.Status == ResultStatus.AccessDenied)
        {
            return Forbid();
        }
        else if (!result.IsSuccess)
        {
            return Forbid();
        }

        return Ok(result.Data);
    }
}
