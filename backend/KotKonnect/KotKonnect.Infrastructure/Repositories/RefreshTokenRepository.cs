namespace KotKonnect.Infrastructure.Repositories;

using Dapper;
using KotKonnect.Infrastructure.Data;
using KotKonnect.Infrastructure.Models;
using KotKonnect.Infrastructure.Repositories.Abstractions;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RefreshToken?> GetTokenAsync(string token)
    {
        const string sql = @"
            SELECT TokenID, UtilisateurID, Token, DateExpiration, Revoque
            FROM REFRESH_TOKENS
            WHERE Token = @Token";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Token = token });
    }

    public async Task<bool> RevokeTokenByIdAsync(int tokenId)
    {
        const string sql = @"
            UPDATE REFRESH_TOKENS
            SET Revoque = 1
            WHERE TokenID = @TokenID";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { TokenID = tokenId }) > 0;
    }

    public async Task<int> CreateTokenAsync(RefreshToken refreshToken)
    {
        const string sql = @"
            INSERT INTO REFRESH_TOKENS (UtilisateurID, Token, DateExpiration, Revoque)
            VALUES (@UtilisateurID, @Token, @DateExpiration, @Revoque);
            SELECT LAST_INSERT_ID()";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, refreshToken);
    }
}
