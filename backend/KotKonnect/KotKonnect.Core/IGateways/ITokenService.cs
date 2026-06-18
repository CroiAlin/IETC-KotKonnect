namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Models;

public interface ITokenService
{
    string GenerateAccessToken(Utilisateur utilisateur);
    string GenerateRefreshToken();
}
