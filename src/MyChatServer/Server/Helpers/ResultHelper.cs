using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.BL.Models;
using System.ComponentModel;

namespace Sulimov.MyChat.Server
{
    /// <summary>
    /// Common helper for work with service result.
    /// </summary>
    public static class ResultHelper
    {
        /// <summary>
        /// Create HTTP response from result.
        /// </summary>
        /// <typeparam name="T">Type of result object.</typeparam>
        /// <param name="serviceResult">Service result.</param>
        /// <param name="controller">Controller.</param>
        /// <returns>Instance of <see cref="IActionResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static ActionResult<T> CreateHttpResult<T>(Result<T> serviceResult, ControllerBase controller)
        {
            controller.HttpContext.Response.ContentType = "application/json";
            
            switch (serviceResult.Status)
            {
                case ResultStatus.Success:
                    return controller.Ok(serviceResult.Data);
                case ResultStatus.BadData:
                    return controller.BadRequest(serviceResult.Message);
                case ResultStatus.NotFound:
                    return controller.NotFound(serviceResult.Message);
                case ResultStatus.Forbidden:
                    return controller.Forbid(serviceResult.Message);
                case ResultStatus.InternalError:
                    return controller.StatusCode(500, serviceResult.Message);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
