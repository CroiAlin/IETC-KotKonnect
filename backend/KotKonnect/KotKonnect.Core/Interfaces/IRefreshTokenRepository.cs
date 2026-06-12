namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetTokenAsync(string token);
    Task<bool> RevokeTokenByIdAsync(int tokenID);
    Task<int> CreateTokenAsync(RefreshToken refreshToken);
}

