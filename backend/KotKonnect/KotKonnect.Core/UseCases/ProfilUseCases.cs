namespace KotKonnect.Core.UseCases;

using KotKonnect.Core.Exceptions;
using KotKonnect.Core.IGateways;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public class ProfilUseCases : IProfilUseCases
{
    private readonly IProfilGateway _profilGateway;
    private readonly ICandidatureGateway _candidatureGateway;

    public ProfilUseCases(IProfilGateway profilGateway, ICandidatureGateway candidatureGateway)
    {
        _profilGateway = profilGateway;
        _candidatureGateway = candidatureGateway;
    }

    public async Task<Profil> GetMonProfilAsync(int utilisateurId)
    {
        return await _profilGateway.GetByUtilisateurIdAsync(utilisateurId)
            ?? throw new KeyNotFoundException("Profil introuvable.");
    }

    public async Task<Profil> UpdateMonProfilAsync(UpdateProfilRequest request, int utilisateurId)
    {
        var profil = await _profilGateway.GetByUtilisateurIdAsync(utilisateurId)
            ?? throw new KeyNotFoundException("Profil introuvable.");

        profil.Nom = request.Nom;
        profil.Prenom = request.Prenom;
        profil.Telephone = request.Telephone;
        profil.Ville = request.Ville;
        profil.Ecole = request.Ecole;
        profil.BudgetMax = request.BudgetMax;

        await _profilGateway.UpdateAsync(profil);
        return profil;
    }

    // Un propriétaire ne voit que le profil d'un candidat ayant postulé chez lui (sinon 403).
    public async Task<Profil> GetProfilAsync(int utilisateurCibleId, int proprietaireId)
    {
        var aPostuleChezMoi =
            await _candidatureGateway.EtudiantAPostuleChezProprioAsync(utilisateurCibleId, proprietaireId);

        if (!aPostuleChezMoi)
            throw new ForbiddenException("Vous ne pouvez consulter que le profil d'un candidat ayant postulé chez vous.");

        return await _profilGateway.GetByUtilisateurIdAsync(utilisateurCibleId)
            ?? throw new KeyNotFoundException("Profil introuvable.");
    }
}
