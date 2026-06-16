namespace KotKonnect.Infrastructure.Repositories;

using Dapper;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Enums;
using KotKonnect.Core.Interfaces;
using KotKonnect.Infrastructure.Data;

public class CandidatureRepository : ICandidatureRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CandidatureRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsAsync(int bienId, int etudiantId)
    {
        const string sql = @"SELECT COUNT(*) FROM CANDIDATURES WHERE BienID=@BienId AND EtudiantID=@EtudiantId;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(sql, new { BienId = bienId, EtudiantId = etudiantId }) > 0;
    }

    public async Task<int> CreateAsync(Candidature candidature)
    {
        const string sql = @"
            INSERT INTO CANDIDATURES (BienID, EtudiantID, MessageEtudiant, Statut)
            VALUES (@BienId, @EtudiantId, @MessageEtudiant, @Statut);
            SELECT LAST_INSERT_ID();";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            BienId = candidature.BienID,
            EtudiantId = candidature.EtudiantID,
            MessageEtudiant = candidature.MessageEtudiant,
            Statut = candidature.Statut.ToString()
        });

    }

    public async Task<List<Candidature>> GetByEtudiantAsync(int etudiantId)
    {
        const string sql = @"
            SELECT c.CandidatureID, c.EtudiantID, c.MessageEtudiant, c.Statut, c.DateCandidature,
                   b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut
            FROM CANDIDATURES c
            JOIN BIENS b ON b.BienID = c.BienID
            WHERE c.EtudiantID = @EtudiantId;";
        
        using var connection = _connectionFactory.CreateConnection();

        var candidatureDict = new Dictionary<int, Candidature>();

        await connection.QueryAsync<Candidature, BienImmobilier, Candidature>(
            sql,
            (candidature, bien) =>
            {
                if (!candidatureDict.TryGetValue(candidature.CandidatureID, out var currentCandidature))
                {
                    currentCandidature = candidature;
                    candidatureDict.Add(currentCandidature.CandidatureID, currentCandidature);
                }
                currentCandidature.Bien = bien;
                currentCandidature.BienID = bien.BienID;
                return currentCandidature;
            },
            new { EtudiantId = etudiantId },
            splitOn: "BienID"
        );

        return candidatureDict.Values.ToList();
    }

    public async Task<List<Candidature>> GetByProprietaireAsync(int proprietaireId)
    {
        const string sql = @"
            SELECT c.CandidatureID, c.EtudiantID, c.MessageEtudiant, c.Statut, c.DateCandidature,
                   b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut,
                   u.UtilisateurID, u.Email
            FROM CANDIDATURES c
            JOIN BIENS b ON b.BienID = c.BienID
            JOIN UTILISATEURS u ON u.UtilisateurID = c.EtudiantID
            WHERE b.ProprietaireID = @ProprietaireId;";

        using var connections = _connectionFactory.CreateConnection();

        var candidatureDict = new Dictionary<int, Candidature>();

        await connections.QueryAsync<Candidature, BienImmobilier, Utilisateur, Candidature>(
            sql,
            (candidature, bien, etudiant) =>
            {
                // inutile de faire un check ici car une candidature ne peut pas être liée à plusieurs biens ou étudiants, mais on suit le pattern Dapper pour éviter les doublons
                if (!candidatureDict.TryGetValue(candidature.CandidatureID, out var currentCandidature))
                {
                    currentCandidature = candidature;
                    candidatureDict.Add(currentCandidature.CandidatureID, currentCandidature);
                }
                currentCandidature.Bien = bien;
                currentCandidature.BienID = bien.BienID;
                currentCandidature.Etudiant = etudiant;
                return currentCandidature;
            },
            new { ProprietaireId = proprietaireId },
            splitOn: "BienID,UtilisateurID"
        );

        return candidatureDict.Values.ToList();
    }

    public async Task<Candidature?> GetByIdAsync(int candidatureId)
    {
        const string sql = @"
            SELECT c.CandidatureID, c.EtudiantID, c.MessageEtudiant, c.Statut, c.DateCandidature,
                   b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut
            FROM CANDIDATURES c
            JOIN BIENS b ON b.BienID = c.BienID
            WHERE c.CandidatureID = @CandidatureId;";

        using var connection = _connectionFactory.CreateConnection();
        
        var candidatureDict = new Dictionary<int, Candidature>();

        await connection.QueryAsync<Candidature, BienImmobilier, Candidature>(
            sql,
            (candidature, bien) =>
            {
                if (!candidatureDict.TryGetValue(candidature.CandidatureID, out var currentCandidature))
                {
                    currentCandidature = candidature;
                    candidatureDict.Add(currentCandidature.CandidatureID, currentCandidature);
                }
                currentCandidature.Bien = bien;
                currentCandidature.BienID = bien.BienID;
                return currentCandidature;
            },
            new { CandidatureId = candidatureId },
            splitOn: "BienID"
        );

        return candidatureDict.Values.FirstOrDefault();
    }
    public async Task<bool> UpdateStatutAsync(int candidatureId, StatutCandidature statut)
    {
        const string sql = @"UPDATE CANDIDATURES SET Statut=@Statut WHERE CandidatureID=@CandidatureId;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { CandidatureId = candidatureId, Statut = statut.ToString() }) > 0;
    }
}
