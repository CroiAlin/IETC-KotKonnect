namespace KotKonnect.Infrastructure.Repositories;

using Dapper;
using KotKonnect.Infrastructure.Data;
using KotKonnect.Infrastructure.Models;
using KotKonnect.Infrastructure.Repositories.Abstractions;

public class BienRepository : IBienRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BienRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<BienImmobilier?> GetByIdAsync(int bienId)
    {
        const string sql = @"
            SELECT b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut,
                   p.PhotoID, p.BienID, p.UrlImage, p.Ordre
            FROM BIENS b
            LEFT JOIN PHOTOS_BIEN p ON p.BienID = b.BienID
            WHERE b.BienID = @BienId
            ORDER BY p.Ordre;";

        using var connection = _connectionFactory.CreateConnection();

        var biens = new Dictionary<int, BienImmobilier>();

        await connection.QueryAsync<BienImmobilier, Photo, BienImmobilier>(
            sql,
            (bien, photo) =>
            {
                // 1re fois qu'on rencontre ce bien -> on le mémorise
                if (!biens.TryGetValue(bien.BienID, out var courant))
                {
                    courant = bien;
                    biens.Add(courant.BienID, courant);
                }
                // LEFT JOIN : un bien sans photo donne photo = null
                if (photo is not null)
                    courant.Photos.Add(photo);
                return courant;
            },
            new { BienId = bienId },
            splitOn: "PhotoID");

        return biens.Values.FirstOrDefault();
    }

    public async Task<List<BienImmobilier>> GetAllPubliesAsync()
    {
        const string sql = @"
            SELECT b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut,
                   p.PhotoID, p.BienID, p.UrlImage, p.Ordre
            FROM BIENS b
            LEFT JOIN PHOTOS_BIEN p ON p.BienID = b.BienID
            WHERE b.Statut = 'PUBLIE'
            ORDER BY b.BienID, p.Ordre;";

        using var connection = _connectionFactory.CreateConnection();

        var biensPublies = new Dictionary<int, BienImmobilier>();

        await connection.QueryAsync<BienImmobilier, Photo, BienImmobilier>(
            sql,
            (bien, photo) =>
            {
                if (!biensPublies.TryGetValue(bien.BienID, out var courant))
                {
                    courant = bien;
                    biensPublies.Add(courant.BienID, courant);
                }
                if (photo is not null)
                    courant.Photos.Add(photo);
                return courant;
            },
            splitOn: "PhotoID");

        return biensPublies.Values.ToList();
    }

    public async Task<List<BienImmobilier>> GetByProprietaireAsync(int proprietaireId)
    {
        const string sql = @"
            SELECT b.BienID, b.ProprietaireID, b.Titre, b.Description, b.Adresse, b.Ville,
                   b.CodePostal, b.Surface, b.NombrePieces, b.LoyerBase, b.Charges, b.Statut,
                   p.PhotoID, p.BienID, p.UrlImage, p.Ordre
            FROM BIENS b
            LEFT JOIN PHOTOS_BIEN p ON p.BienID = b.BienID
            WHERE b.ProprietaireID = @ProprietaireId AND b.Statut <> 'SUPPRIME'
            ORDER BY b.BienID, p.Ordre;";

        using var connection = _connectionFactory.CreateConnection();

        var biensProprietaire = new Dictionary<int, BienImmobilier>();

        await connection.QueryAsync<BienImmobilier, Photo, BienImmobilier>(
            sql,
            (bien, photo) =>
            {
                if (!biensProprietaire.TryGetValue(bien.BienID, out var courant))
                {
                    courant = bien;
                    biensProprietaire.Add(courant.BienID, courant);
                }
                if (photo is not null)
                    courant.Photos.Add(photo);
                return courant;
            },
            new { ProprietaireId = proprietaireId },
            splitOn: "PhotoID");

        return biensProprietaire.Values.ToList();
    }

    public async Task<int> CreateAsync(BienImmobilier bien)
    {
        const string sql = @"
            INSERT INTO BIENS (ProprietaireID,Titre,Description,Adresse,Ville,CodePostal,Surface,NombrePieces,LoyerBase,Charges,Statut)
            VALUES (@ProprietaireID,@Titre,@Description,@Adresse,@Ville,@CodePostal,@Surface,@NombrePieces,@LoyerBase,@Charges,@Statut);
            SELECT LAST_INSERT_ID();";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            bien.ProprietaireID,
            bien.Titre,
            bien.Description,
            bien.Adresse,
            bien.Ville,
            bien.CodePostal,
            bien.Surface,
            bien.NombrePieces,
            bien.LoyerBase,
            bien.Charges,
            bien.Statut
        });
    }

    public async Task<bool> UpdateAsync(BienImmobilier bien)
    {
        const string sql = @"
            UPDATE BIENS SET Titre=@Titre,Description=@Description,Adresse=@Adresse,
                            Ville=@Ville,CodePostal=@CodePostal,Surface=@Surface,NombrePieces=@NombrePieces,LoyerBase=@LoyerBase,
                            Charges=@Charges,Statut=@Statut
            WHERE BienID=@BienID";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new
        {
            bien.BienID,
            bien.ProprietaireID,
            bien.Titre,
            bien.Description,
            bien.Adresse,
            bien.Ville,
            bien.CodePostal,
            bien.Surface,
            bien.NombrePieces,
            bien.LoyerBase,
            bien.Charges,
            bien.Statut
        }) > 0;
    }

    public async Task<bool> SoftDeleteAsync(int bienId)
    {
        const string sql = @"
           UPDATE BIENS SET Statut='SUPPRIME'
            WHERE BienID=@BienID";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { BienID = bienId }) > 0;
    }

    public async Task<int> AddPhotoAsync(Photo photo)
    {
        const string sql = @"
            INSERT INTO PHOTOS_BIEN (BienID, UrlImage, Ordre)
            VALUES (@BienID, @UrlImage, @Ordre);
            SELECT LAST_INSERT_ID();";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new { photo.BienID, photo.UrlImage, photo.Ordre });
    }

    public async Task<bool> DeletePhotoAsync(int photoId, int bienId)
    {
        const string sql = @"
            DELETE FROM PHOTOS_BIEN
            WHERE PhotoID = @PhotoId AND BienID = @BienId;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { PhotoId = photoId, BienId = bienId }) > 0;
    }
}
