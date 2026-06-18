namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Models;

public interface IRefreshTokenGateway
{
    Task<RefreshToken?> GetTokenAsync(string token);
    Task<bool> RevokeTokenByIdAsync(int tokenId);
    Task<int> CreateTokenAsync(RefreshToken refreshToken);
}
