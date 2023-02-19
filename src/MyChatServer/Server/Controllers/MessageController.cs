using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Services;

namespace Sulimov.MyChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        
        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            return Ok(await messageService.GetAllChatMessages(chatId));
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            var result = await messageService.SaveMessage(message);

            if (result == null)
            {
                return BadRequest();
            }


            return Ok(result);
        }
    }
}
