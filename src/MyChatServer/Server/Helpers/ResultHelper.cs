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
        /// <typeparam name="T">Type of result object.</typeparam>
        /// <param name="controller">Controller.</param>
        /// <param name="status">Result status.</param>
        /// <param name="message">Result message</param>
        /// <param name="data">Result data.</param>
        /// <returns>Instance of <see cref="ActionResult{TValue}"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static ActionResult<T> CreateHttpResultFromData<T>(ControllerBase controller, ResultStatus status, string message, object data) where T : class
        {
            controller.HttpContext.Response.ContentType = "application/json";

            switch (status)
            {
                case ResultStatus.Success:
                    return controller.Ok(ConvertData<T>(data));
                case ResultStatus.InconsistentData:
                    return controller.BadRequest(message);
                case ResultStatus.ObjectNotFound:
                    return controller.NotFound(message);
                case ResultStatus.AccessDenied:
                    return controller.Forbid(message);
                case ResultStatus.UnhandledError:
                    return controller.StatusCode(500, message);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Convert <see cref="IUser"/> instance to instance of <see cref="UserDto"/>.
        /// </summary>
        /// <param name="user"><see cref="IUser"/> instance.</param>
        /// <returns>Instance of <see cref="UserDto"/>.</returns>
        public static UserDto ConvertUser(IUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
        }

        /// <summary>
        /// Convert <see cref="IMessage"/> instance to instance of <see cref="MessageDto"/>.
        /// </summary>
        /// <param name="message"><see cref="IMessage"/> instance.</param>
        /// <returns>Instance of <see cref="MessageDto"/>.</returns>
        public static MessageDto ConvertMessage(IMessage message)
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
        /// Convert <see cref="IChat"/> instance to instance of <see cref="ChatDto"/>.
        /// </summary>
        /// <param name="chat"><see cref="IChat"/> instance.</param>
        /// <returns>Instance of <see cref="ChatDto"/>.</returns>
        public static ChatDto ConvertChat(IChat chat)
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

            if (data is IUser userData)
            {
                return ConvertUser(userData);
            }
            else if (data is IChat chatData)
            {
                return ConvertChat(chatData);
            }
            else if (data is IMessage messageData)
            {
                return ConvertMessage(messageData);
            }
            else if (data is IEnumerable<IMessage> messageDataCollection)
            {
                return messageDataCollection
                    .Select(s => ConvertMessage(s))
                    .ToArray();
            }
            else if (data is IEnumerable<IChat> chatDataCollection)
            {
                return chatDataCollection
                    .Select(s => ConvertChat(s))
                    .ToArray();
            }
            else
            {
                throw new ArgumentException("Unknown result type");
            }
        }
    }
}
