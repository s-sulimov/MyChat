using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Services;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class ChatsController : ControllerBase
    {
        private readonly IChateService chateService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ChatsController(IChateService chateService, IHttpContextAccessor httpContextAccessor)
        {
            this.chateService = chateService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Ok(await this.chateService.GetUserChats(userId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] Chat chat)
        {
            if (chat.Users != null && chat.Users.Count() <= 1)
            {
                return BadRequest();
            }

            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Chat newChat = await this.chateService.CreateChat(chat, userId);

            if (newChat == null)
            {
                return BadRequest();
            }

            return Ok(newChat);
        }

        [HttpPut("add-user")]
        public async Task<IActionResult> AddUserToChat(int chatId, string userId)
        {
            var actualUserId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chat = await this.chateService.AddUserToChat(chatId, actualUserId, userId);

            if (chat == null)
            {
                return BadRequest();
            }

            return Ok(chat);
        }

        [HttpPut("remove-user")]
        public async Task<IActionResult> RemoveUserFromChat(int chatId, string userId)
        {
            var actualUserId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chat = await this.chateService.RemoveUserFromChat(chatId, actualUserId, userId);

            if (chat == null)
            {
                return BadRequest();
            }

            return Ok(chat);
        }

        [HttpPut("set-admin")]
        public async Task<IActionResult> SetChatAdmin(int chatId, string userId)
        {
            var actualUserId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chat = await this.chateService.SetChatAdmin(chatId, actualUserId, userId);

            if (chat == null)
            {
                return BadRequest();
            }

            return Ok(chat);
        }

        [HttpPut("remove-admin")]
        public async Task<IActionResult> RemoveChatAdmin(int chatId, string userId)
        {
            var actualUserId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chat = await this.chateService.RemoveChatAdmin(chatId, actualUserId, userId);

            if (chat == null)
            {
                return BadRequest();
            }

            return Ok(chat);
        }
    }
}
