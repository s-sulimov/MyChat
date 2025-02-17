using Microsoft.AspNetCore.Mvc;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Responses;
using System.ComponentModel;

namespace Sulimov.MyChat.Server.Core.Helpers;

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
    public static UserDto ConvertUser(User? user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        return new UserDto(id: user.Id, name: user.Name, email: user.Email);
    }

    /// <summary>
    /// Convert <see cref="Message"/> instance to instance of <see cref="MessageDto"/>.
    /// </summary>
    /// <param name="message"><see cref="Message"/> instance.</param>
    /// <returns>Instance of <see cref="MessageDto"/>.</returns>
    public static MessageDto ConvertMessage(Message? message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        return new MessageDto(
            id: message.Id,
            chatId: message.ChatId,
            dateTime: message.DateTime,
            sender: ConvertUser(message.Sender),
            text: message.Text);
    }

    /// <summary>
    /// Convert <see cref="Chat"/> instance to instance of <see cref="ChatDto"/>.
    /// </summary>
    /// <param name="chat"><see cref="Chat"/> instance.</param>
    /// <returns>Instance of <see cref="ChatDto"/>.</returns>
    public static ChatDto ConvertChat(Chat? chat)
    {
        ArgumentNullException.ThrowIfNull(chat, nameof(chat));

        return new ChatDto(
            id: chat.Id,
            title: chat.Title,
            users: chat.Users
                .Select(s => new ChatUserDto(
                    id: s.Id,
                    user: ConvertUser(s.User),
                    role: new ChatRoleDto(s.Role.Id, s.Role.Name)))
                .ToList());
    }

    private static object ConvertData<T>(object? data)
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
            case IReadOnlyCollection<Chat> chats:
                return chats
                .Select(s => ConvertChat(s))
                .ToList();
            case IReadOnlyCollection<Message> messages:
                return messages
                .Select(s => ConvertMessage(s))
                .ToList();
            default:
                throw new ArgumentException("Unknown result type");
        }
    }
}
