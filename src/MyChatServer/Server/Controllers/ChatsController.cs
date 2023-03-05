using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Models.Requests;
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

            var userId = this.httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await this.chateService.CreateChat(request, userId);

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

            return ResultHelper.CreateHttpResult(result, this);
        }
    }
}
