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
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MessagesController(IMessageService messageService, IHttpContextAccessor httpContextAccessor)
        {
            this.messageService = messageService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            return Ok(await messageService.GetAllChatMessages(chatId));
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await messageService.SaveMessage(message, userId);

            if (result == null)
            {
                return BadRequest();
            }


            return Ok(result);
        }
    }
}
