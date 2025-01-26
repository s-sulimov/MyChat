using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.Authorization.Models;

/// <summary>
/// Request for token creation.
/// </summary>
public class CreateTokenRequest
{
    /// <summary>
    /// User ID.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Login.
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// User email.
    /// </summary>
    [Required]
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>
    /// Claims.
    /// </summary>
    [Required]
    public string[] Claims { get; set; } = Array.Empty<string>();
}
