namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Service result entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
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

        public Result(ResultStatus status, T data, string message)
        {
            Status = status;
            Data = data;
            Message = message;
        }
    }
}
