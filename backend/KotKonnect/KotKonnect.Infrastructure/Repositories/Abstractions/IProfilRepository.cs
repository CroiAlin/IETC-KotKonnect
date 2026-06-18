namespace KotKonnect.Infrastructure.Repositories.Abstractions;

using KotKonnect.Infrastructure.Models;

public interface IProfilRepository
{
    Task<Profil?> GetByUtilisateurIdAsync(int utilisateurId);
    Task<int> CreateAsync(Profil profil);
    Task UpdateAsync(Profil profil);
}
