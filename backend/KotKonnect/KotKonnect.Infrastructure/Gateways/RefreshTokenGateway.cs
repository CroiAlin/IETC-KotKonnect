namespace KotKonnect.Infrastructure.Gateways;

using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using CoreModels = KotKonnect.Core.Models;
using InfraModels = KotKonnect.Infrastructure.Models;

public class RefreshTokenGateway : IRefreshTokenGateway
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenGateway(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    public async Task<CoreModels.RefreshToken?> GetTokenAsync(string token)
    {
        var rt = await _repository.GetTokenAsync(token);
        return rt is null ? null : ToCore(rt);
    }

    public Task<bool> RevokeTokenByIdAsync(int tokenId) => _repository.RevokeTokenByIdAsync(tokenId);

    public Task<int> CreateTokenAsync(CoreModels.RefreshToken refreshToken) =>
        _repository.CreateTokenAsync(ToInfra(refreshToken));

    private static CoreModels.RefreshToken ToCore(InfraModels.RefreshToken rt) => new()
    {
        TokenID = rt.TokenID,
        UtilisateurID = rt.UtilisateurID,
        Token = rt.Token,
        DateExpiration = rt.DateExpiration,
        Revoque = rt.Revoque
    };

    private static InfraModels.RefreshToken ToInfra(CoreModels.RefreshToken rt) => new()
    {
        TokenID = rt.TokenID,
        UtilisateurID = rt.UtilisateurID,
        Token = rt.Token,
        DateExpiration = rt.DateExpiration,
        Revoque = rt.Revoque
    };
}
