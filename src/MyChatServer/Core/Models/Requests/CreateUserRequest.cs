using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.Core.Models.Requests;

/// <summary>
/// DTO for user creation.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// User name (login).
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// User e-mail.
    /// </summary>
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
