using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Models.Responses;
using System.ComponentModel;

namespace Sulimov.MyChat.Server.Helpers
{
    /// <summary>
    /// Common helper for work with service result.
    /// </summary>
    public static class ResultHelper
    {
        /// <summary>
        /// Create HTTP response.
        /// </summary>
        /// <typeparam name="TIn">Type of result object.</typeparam>
        /// <typeparam name="TOut">Type of output object.</typeparam>
        /// <param name="controller">Controller.</param>
        /// <param name="result">Service result.</param>
        /// <returns>Instance of <see cref="ActionResult{TOut}"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Unknown result status.</exception>
        public static ActionResult<TOut> CreateHttpResult<TIn, TOut>(ControllerBase controller, Result<TIn> result) where TIn : class
        {
            controller.HttpContext.Response.ContentType = "application/json";

            switch (result.Status)
            {
                case ResultStatus.Success:
                    return controller.Ok(ConvertData<TOut>(result.Data));
                case ResultStatus.InconsistentData:
                    return controller.BadRequest(result.Message);
                case ResultStatus.ObjectNotFound:
                    return controller.NotFound(result.Message);
                case ResultStatus.AccessDenied:
                    return controller.Forbid(result.Message);
                case ResultStatus.UnhandledError:
                    return controller.StatusCode(500, result.Message);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Convert <see cref="User"/> instance to instance of <see cref="UserDto"/>.
        /// </summary>
        /// <param name="user"><see cref="User"/> instance.</param>
        /// <returns>Instance of <see cref="UserDto"/>.</returns>
        public static UserDto ConvertUser(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
        }

        /// <summary>
        /// Convert <see cref="Message"/> instance to instance of <see cref="MessageDto"/>.
        /// </summary>
        /// <param name="message"><see cref="Message"/> instance.</param>
        /// <returns>Instance of <see cref="MessageDto"/>.</returns>
        public static MessageDto ConvertMessage(Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                DateTime = message.DateTime,
                Sender = ConvertUser(message.Sender),
                Text = message.Text,
            };
        }

        /// <summary>
        /// Convert <see cref="Chat"/> instance to instance of <see cref="ChatDto"/>.
        /// </summary>
        /// <param name="chat"><see cref="Chat"/> instance.</param>
        /// <returns>Instance of <see cref="ChatDto"/>.</returns>
        public static ChatDto ConvertChat(Chat chat)
        {
            return new ChatDto
            {
                Id = chat.Id,
                Title = chat.Title,
                Users = chat.Users.Select(s => new ChatUserDto
                {
                    Id = s.Id,
                    User = ConvertUser(s.User),
                    Role = new ChatRoleDto
                    {
                        Id = s.Role.Id,
                        Name = s.Role.Name,
                    },
                }),
            };
        }

        private static object ConvertData<T>(object data)
        {
            if (data is T)
            {
                return data;
            }

            switch (data)
            {
                case User user:
                    return ConvertUser(user);
                case Chat chat:
                    return ConvertChat(chat);
                case Message message:
                    return ConvertMessage(message);
                case IEnumerable<Chat> chats:
                    return chats
                    .Select(s => ConvertChat(s))
                    .ToArray();
                case IEnumerable<Message> messages:
                    return messages
                    .Select(s => ConvertMessage(s))
                    .ToArray();
                default:
                    throw new ArgumentException("Unknown result type");
            }
        }
    }
}
