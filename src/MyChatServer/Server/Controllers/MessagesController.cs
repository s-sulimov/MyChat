using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Hubs;
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
        private readonly IHubContext<ChatHub> chatHubContext;
        private readonly IChateService chateService;

        public MessagesController(
            IMessageService messageService,
            IHttpContextAccessor httpContextAccessor,
            IHubContext<ChatHub> chatHubContext,
            IChateService chatService)
        {
            this.messageService = messageService;
            this.httpContextAccessor = httpContextAccessor;
            this.chatHubContext = chatHubContext;
            this.chateService = chatService;
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

            if (result.IsSuccess)
            {
                var users = await chateService.GetChatUsers(message.ChatId, userId);
                await chatHubContext.Clients.Users(users).SendAsync("message", result.Data);
            }

            return ResultHelper.CreateHttpResult(result, this);
        }
    }
}
