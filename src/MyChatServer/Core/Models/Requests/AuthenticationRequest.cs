using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.Core.Models.Requests;

/// <summary>
/// DTO for authentification request.
/// </summary>
public class AuthenticationRequest
{
    /// <summary>
    /// User login or email.
    /// </summary>
    [Required]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
