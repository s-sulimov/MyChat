using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.Helpers;
using Sulimov.MyChat.Server.Hubs;
using Sulimov.MyChat.Server.Models;

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

        // api/messages/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<IMessage>>> GetAllMessages(int chatId)
        {
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await messageService.GetAllChatMessages(chatId, userId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/messages/last
        [HttpGet("last")]
        public async Task<ActionResult<IEnumerable<IMessage>>> GetLastMessages(int chatId, DateTime fromDateTime)
        {
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await messageService.GetLastChatMessages(chatId, userId, fromDateTime);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/messages
        [HttpPost]
        public async Task<ActionResult<IMessage>> SendMessage(SendMessageRequest message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (userId == null)
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await messageService.SaveMessage(userId, message.ChatId, message.Message);

            if (result.IsSuccess)
            {
                var users = await chateService.GetChatUsers(message.ChatId, userId);
                await chatHubContext.Clients.Users(users).SendAsync("message", result.Data);
            }

            return ResultHelper.CreateHttpResult(this, result);
        }
    }
}
