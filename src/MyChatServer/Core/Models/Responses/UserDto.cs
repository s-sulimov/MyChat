namespace Sulimov.MyChat.Server.Core.Models.Responses;

/// <summary>
/// User.
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// User name (login).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// User e-mail.
    /// </summary>
    public string Email { get; }

    public UserDto(string id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}
