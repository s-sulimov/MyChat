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
    public class ChatsController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHubContext<ChatHub> chatHubContext;

        public ChatsController(IChatService chatService, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> chatHubContext)
        {
            this.chatService = chatService;
            this.httpContextAccessor = httpContextAccessor;
            this.chatHubContext = chatHubContext;
        }

        // api/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IChat>>> GetChats()
        {
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await chatService.GetUserChats(userId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/chats
        [HttpPost]
        public async Task<ActionResult<IChat>> CreateChat(CreateChatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await this.chatService.CreateChat(request.Title, request.ChatUserIds, currentUserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/chats/add-user
        [HttpPut("add-user")]
        public async Task<ActionResult<IChat>> AddUserToChat(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await this.chatService.AddUserToChat(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/chats/remove-user
        [HttpPut("remove-user")]
        public async Task<ActionResult<IChat>> RemoveUserFromChat(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await this.chatService.RemoveUserFromChat(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/chats/set-admin
        [HttpPut("set-admin")]
        public async Task<ActionResult<IChat>> SetChatAdmin(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await this.chatService.SetChatAdmin(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        // api/chats/remove-admin
        [HttpPut("remove-admin")]
        public async Task<ActionResult<IChat>> RemoveChatAdmin(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await this.chatService.RemoveChatAdmin(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(this, result);
        }

        private async Task SendResult(IResult<IChat> result, string currentUserId)
        {
            if (result.IsSuccess)
            {
                var users = result.Data.Users
                    .Where(w => w.User.Id != currentUserId)
                    .Select(s => s.User.Id)
                    .ToArray();

                await chatHubContext.Clients.Clients(users).SendAsync("chat", result.Data);
            }
        }
    }
}
