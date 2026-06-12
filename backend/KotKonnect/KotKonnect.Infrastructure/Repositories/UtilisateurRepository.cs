
namespace KotKonnect.Infrastructure.Repositories;

using Dapper;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Interfaces;
using KotKonnect.Infrastructure.Data;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UtilisateurRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Utilisateur?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT UtilisateurID, Email, MotDePasseHash, Role, DateCreation
            FROM UTILISATEURS
            WHERE Email = @Email;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Utilisateur>(sql, new { Email = email });
    }

    public async Task<Utilisateur?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT UtilisateurID, Email, MotDePasseHash, Role, DateCreation
            FROM UTILISATEURS
            WHERE UtilisateurID = @ID;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Utilisateur>(sql, new { ID = id });
    }

    public async Task<int> CreateAsync(Utilisateur utilisateur)
    {
        const string sql = @"
            INSERT INTO UTILISATEURS (Email, MotDePasseHash, Role, DateCreation)
            VALUES (@Email, @MotDePasseHash, @Role, @DateCreation);
            SELECT LAST_INSERT_ID();";
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            utilisateur.Email,
            utilisateur.MotDePasseHash,
            Role = utilisateur.Role.ToString(),
            utilisateur.DateCreation
        });
    }
}


