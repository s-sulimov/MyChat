using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Models.Requests;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Hubs;
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
        private readonly IHubContext<ChatHub> chatHubContext;

        public ChatsController(IChateService chateService, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> chatHubContext)
        {
            this.chateService = chateService;
            this.httpContextAccessor = httpContextAccessor;
            this.chatHubContext = chatHubContext;
        }

        // api/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await chateService.GetUserChats(userId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/chats
        [HttpPost]
        public async Task<ActionResult<Chat>> CreateChat(CreateChatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.CreateChat(request, currentUserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/chats/add-user
        [HttpPut("add-user")]
        public async Task<ActionResult<Chat>> AddUserToChat(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.AddUserToChat(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/chats/remove-user
        [HttpPut("remove-user")]
        public async Task<ActionResult<Chat>> RemoveUserFromChat(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.RemoveUserFromChat(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/chats/set-admin
        [HttpPut("set-admin")]
        public async Task<ActionResult<Chat>> SetChatAdmin(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.SetChatAdmin(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        // api/chats/remove-admin
        [HttpPut("remove-admin")]
        public async Task<ActionResult<Chat>> RemoveChatAdmin(UpdateChatUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.RemoveChatAdmin(request.ChatId, currentUserId, request.UserId);

            await SendResult(result, currentUserId);

            return ResultHelper.CreateHttpResult(result, this);
        }

        private async Task SendResult(Result<Chat> result, string currentUserId)
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
