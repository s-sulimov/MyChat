using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sulimov.MyChat.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
    }
}
