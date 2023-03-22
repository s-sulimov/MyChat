using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.Helpers;
using Sulimov.MyChat.Server.Hubs;
using Sulimov.MyChat.Server.Models;
using Sulimov.MyChat.Server.Models.Responses;
using System.Collections.Generic;

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
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetChats()
        {
            var userId = ControllerHelper.GetCurrentUserId(httpContextAccessor);
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(500, Constants.UnknownErrorMessage);
            }

            var result = await chatService.GetUserChats(userId);

            return ResultHelper.CreateHttpResult<IEnumerable<Chat>, IEnumerable<ChatDto>>(this, result);
        }

        // api/chats
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<ChatDto>> CreateChat(CreateChatRequest request)
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

            await SendResult(result);

            return ResultHelper.CreateHttpResult<Chat, ChatDto>(this, result);
        }

        // api/chats/add-user
        [HttpPut("add-user")]
        [Produces("application/json")]
        public async Task<ActionResult<ChatDto>> AddUserToChat(UpdateChatUserRequest request)
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

            await SendResult(result);

            return ResultHelper.CreateHttpResult<Chat, ChatDto>(this, result);
        }

        // api/chats/remove-user
        [HttpPut("remove-user")]
        [Produces("application/json")]
        public async Task<ActionResult<ChatDto>> RemoveUserFromChat(UpdateChatUserRequest request)
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

            if (result.IsSuccess)
            {
                await chatHubContext.Clients.Users(request.UserId).SendAsync("remove-user-from-chat", result.Data.Id);
            }

            return ResultHelper.CreateHttpResult<Chat, ChatDto>(this, result);
        }

        // api/chats/set-admin
        [HttpPut("set-admin")]
        [Produces("application/json")]
        public async Task<ActionResult<ChatDto>> SetChatAdmin(UpdateChatUserRequest request)
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

            await SendResult(result);

            return ResultHelper.CreateHttpResult<Chat, ChatDto>(this, result);
        }

        // api/chats/remove-admin
        [HttpPut("remove-admin")]
        [Produces("application/json")]
        public async Task<ActionResult<ChatDto>> RemoveChatAdmin(UpdateChatUserRequest request)
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

            await SendResult(result);

            return ResultHelper.CreateHttpResult<Chat, ChatDto>(this, result);
        }

        private async Task SendResult(Result<Chat> result)
        {
            if (result.IsSuccess)
            {
                var users = result.Data.Users
                    .Select(s => s.User.Id)
                    .ToArray();

                await chatHubContext.Clients.Users(users).SendAsync("chat", ResultHelper.ConvertChat(result.Data));
            }
        }
    }
}
