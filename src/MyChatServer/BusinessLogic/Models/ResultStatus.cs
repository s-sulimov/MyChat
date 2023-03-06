namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Service result statuses.
    /// </summary>
    public enum ResultStatus : byte
    {
        /// <summary>
        /// Successful result.
        /// </summary>
        Success,

        /// <summary>
        /// Bad data.
        /// </summary>
        BadData,

        /// <summary>
        /// Object not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Resource is forbidden.
        /// </summary>
        Forbidden,

        /// <summary>
        /// Internal server error.
        /// </summary>
        InternalError,
    }
}
