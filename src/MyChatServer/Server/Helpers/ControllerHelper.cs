using System.Security.Claims;

namespace Sulimov.MyChat.Server.Helpers
{
    /// <summary>
    /// Extensions for controllers
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// Get current user ID.
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor.</param>
        /// <returns>User ID.</returns>
        /// <exception cref="InvalidDataException">If current user ID is empty.</exception>
        public static string GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidDataException("Current usr id is empty.");
            }

            return userId;
        }
    }
}
