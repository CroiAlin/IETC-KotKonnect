namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;

public interface ITokenService
{
    public string GenerateAccessToken(Utilisateur utilisateur);
    public string GenerateRefreshToken();
}

