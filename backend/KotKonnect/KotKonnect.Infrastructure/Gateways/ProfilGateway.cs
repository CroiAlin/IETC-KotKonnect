namespace KotKonnect.Infrastructure.Gateways;

using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using CoreModels = KotKonnect.Core.Models;
using InfraModels = KotKonnect.Infrastructure.Models;

public class ProfilGateway : IProfilGateway
{
    private readonly IProfilRepository _repository;

    public ProfilGateway(IProfilRepository repository)
    {
        _repository = repository;
    }

    public async Task<CoreModels.Profil?> GetByUtilisateurIdAsync(int utilisateurId)
    {
        var profil = await _repository.GetByUtilisateurIdAsync(utilisateurId);
        return profil is null ? null : ToCore(profil);
    }

    public Task<int> CreateAsync(CoreModels.Profil profil) => _repository.CreateAsync(ToInfra(profil));

    public Task UpdateAsync(CoreModels.Profil profil) => _repository.UpdateAsync(ToInfra(profil));

    public static CoreModels.Profil ToCore(InfraModels.Profil p) => new()
    {
        ProfilID = p.ProfilID,
        UtilisateurID = p.UtilisateurID,
        Nom = p.Nom,
        Prenom = p.Prenom,
        Telephone = p.Telephone,
        Ville = p.Ville,
        Ecole = p.Ecole,
        BudgetMax = p.BudgetMax
    };

    private static InfraModels.Profil ToInfra(CoreModels.Profil p) => new()
    {
        ProfilID = p.ProfilID,
        UtilisateurID = p.UtilisateurID,
        Nom = p.Nom,
        Prenom = p.Prenom,
        Telephone = p.Telephone,
        Ville = p.Ville,
        Ecole = p.Ecole,
        BudgetMax = p.BudgetMax
    };
}
