using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Authorization.Services;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Authorization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtController : ControllerBase
{
    private readonly IJwtService jwtService;

    public JwtController(IJwtService jwtService)
    {
        this.jwtService = jwtService;
    }

    // api/jwt/create
    [HttpPost("login")]
    [AllowAnonymous]
    [Produces("application/json")]
    public ActionResult<AuthenticationResponse> CreateToken([FromBody] CreateTokenRequest request)
    {
        return this.jwtService.CreateToken(request.UserId, request.UserName, request.UserEmail, request.Claims.Select(s => new Claim(ClaimTypes.Role, s)).ToList());
    }
}
