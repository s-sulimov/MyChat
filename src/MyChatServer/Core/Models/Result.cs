using Sulimov.MyChat.Server.Core.Enums;

namespace Sulimov.MyChat.Server.Core.Models
{
    /// <inheritdoc/>
    public class Result<T> : IResult<T> where T : class
    {
        /// <inheritdoc/>
        public ResultStatus Status { get; set; }

        /// <inheritdoc/>
        public T Data { get; set; }

        /// <inheritdoc/>
        public string Message { get; set; }

        /// <inheritdoc/>
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
