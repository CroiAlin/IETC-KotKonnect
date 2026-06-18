namespace KotKonnect.Infrastructure.Repositories.Abstractions;

using KotKonnect.Infrastructure.Models;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetTokenAsync(string token);
    Task<bool> RevokeTokenByIdAsync(int tokenId);
    Task<int> CreateTokenAsync(RefreshToken refreshToken);
}
