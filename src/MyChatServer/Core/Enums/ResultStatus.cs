namespace Sulimov.MyChat.Server.Core.Enums
{
    /// <summary>
    /// Service result statuses.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// Successful result.
        /// </summary>
        Success,

        /// <summary>
        /// Inconsistent data.
        /// </summary>
        InconsistentData,

        /// <summary>
        /// Object not found.
        /// </summary>
        ObjectNotFound,

        /// <summary>
        /// Access is denied.
        /// </summary>
        AccessDenied,

        /// <summary>
        /// Unhandled server error.
        /// </summary>
        UnhandledError,
    }
}
