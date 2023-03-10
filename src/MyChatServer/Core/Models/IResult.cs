using Sulimov.MyChat.Server.Core.Enums;

namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Service result.
    /// </summary>
    /// <typeparam name="T">Type of result object.</typeparam>
    public interface IResult<T> where T : class
    {
        /// <summary>
        /// Result status.
        /// </summary>
        ResultStatus Status { get; set; }

        /// <summary>
        /// Result data.
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// Result message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Success result.
        /// </summary>
        bool IsSuccess { get; }
    }
}
