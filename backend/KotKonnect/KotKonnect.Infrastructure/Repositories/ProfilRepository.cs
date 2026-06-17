namespace KotKonnect.Infrastructure.Repositories;

using Dapper;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Interfaces;
using KotKonnect.Infrastructure.Data;

public class ProfilRepository : IProfilRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProfilRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Lecture du profil d'un utilisateur (relation 1-1 -> au plus une ligne).
    public async Task<Profil?> GetByUtilisateurIdAsync(int utilisateurId)
    {
        const string sql = @"
            SELECT ProfilID, UtilisateurID, Nom, Prenom, Telephone, Ville, Ecole, BudgetMax
            FROM PROFILS
            WHERE UtilisateurID = @UtilisateurID;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Profil>(sql, new { UtilisateurID = utilisateurId });
    }

    // Insertion d'un nouveau profil. Renvoie l'ID auto-incrémenté généré par MySQL.
    public async Task<int> CreateAsync(Profil profil)
    {
        const string sql = @"
            INSERT INTO PROFILS (UtilisateurID, Nom, Prenom, Telephone, Ville, Ecole, BudgetMax)
            VALUES (@UtilisateurID, @Nom, @Prenom, @Telephone, @Ville, @Ecole, @BudgetMax);
            SELECT LAST_INSERT_ID();";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            profil.UtilisateurID,
            profil.Nom,
            profil.Prenom,
            profil.Telephone,
            profil.Ville,
            profil.Ecole,
            profil.BudgetMax
        });
    }

    // Mise à jour des champs modifiables, ciblée sur l'UtilisateurID du connecté.
    public async Task UpdateAsync(Profil profil)
    {
        const string sql = @"
            UPDATE PROFILS
            SET Nom = @Nom,
                Prenom = @Prenom,
                Telephone = @Telephone,
                Ville = @Ville,
                Ecole = @Ecole,
                BudgetMax = @BudgetMax
            WHERE UtilisateurID = @UtilisateurID;";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            profil.Nom,
            profil.Prenom,
            profil.Telephone,
            profil.Ville,
            profil.Ecole,
            profil.BudgetMax,
            profil.UtilisateurID
        });
    }
}
