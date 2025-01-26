namespace Sulimov.MyChat.Server.Core.Models.Responses;

/// <summary>
/// Response for authentification request.
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// JWT token.
    /// </summary>
    public string Token { get; }

    /// <summary>
    /// Token expiration date and time.
    /// </summary>
    public DateTime Expiration { get; }

    public AuthenticationResponse(string token, DateTime expirationDate)
    {
        Token = token;
        Expiration = expirationDate;
    }
}
