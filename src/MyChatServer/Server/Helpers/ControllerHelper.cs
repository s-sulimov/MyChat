using System.Security.Claims;

namespace Sulimov.MyChat.Server.Helpers
{
    /// <summary>
    /// Extensions for controllers
    /// </summary>
    public static class ControllerHelper
    {
        public static string GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
