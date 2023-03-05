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

        // api/messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int chatId)
        {
            var result = await messageService.GetAllChatMessages(chatId);
            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/messages
        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(SendMessageRequest message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await messageService.SaveMessage(userId, message.ChatId, message.Message);

            return ResultHelper.CreateHttpResult(result, this);
        }
    }
}
