using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.DAL.Models;
using Sulimov.MyChat.Server.Services;
using System.Data;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;
        private readonly IJwtService jwtService;

        public UsersController(UserManager<DbUser> userManager,
            IJwtService jwtService,
            SignInManager<DbUser> signInManager)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.signInManager = signInManager;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(string login)
        {
            DbUser user = await userManager.FindByNameAsync(login);

            if (user == null)
            {
                return NotFound();
            }

            return new User
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email
            };
        }

        // api/users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }

            var result = await userManager.CreateAsync(
                new DbUser()
                {
                    UserName = user.Name,
                    Email = user.Email,
                },
                user.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var DbUser = await userManager.FindByNameAsync(user.Name);
            result = await userManager.AddToRoleAsync(DbUser, "User");
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction("GetUser", new { login = user.Name }, user);
        }

        // api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await userManager.FindByNameAsync(request.Login);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest("Bad credentials");
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var token = jwtService.CreateToken(user, userRoles.Select(s => new Claim(ClaimTypes.Role, s)));

            return Ok(token);
        }
    }
}
