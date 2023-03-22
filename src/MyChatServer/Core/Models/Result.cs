using Sulimov.MyChat.Server.Core.Enums;

namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Service result.
    /// </summary>
    /// <typeparam name="T">Type of result object.</typeparam>
    public class Result<T> where T : class
    {
        /// <summary>
        /// Result status.
        /// </summary>
        public ResultStatus Status { get; set; }

        /// <summary>
        /// Result data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Result message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Success result.
        /// </summary>
        public bool IsSuccess => Status == ResultStatus.Success;

        public Result(ResultStatus status, T data)
        {
            Status = status;
            Data = data;
            Message = string.Empty;
        }

        public Result(ResultStatus status, T data, string message)
        {
            Status = status;
            Data = data;
            Message = message;
        }
    }
}
