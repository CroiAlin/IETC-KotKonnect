namespace KotKonnect.Core.UseCases.Abstractions;

using KotKonnect.Core.Models;

public interface IProfilUseCases
{
    Task<Profil> GetMonProfilAsync(int utilisateurId);
    Task<Profil> UpdateMonProfilAsync(UpdateProfilRequest request, int utilisateurId);
    Task<Profil> GetProfilAsync(int utilisateurCibleId, int proprietaireId);
}
