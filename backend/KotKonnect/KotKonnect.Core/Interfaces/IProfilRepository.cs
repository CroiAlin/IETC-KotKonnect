namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;

public interface IProfilRepository
{
    Task<Profil?> GetByUtilisateurIdAsync(int utilisateurId);
    Task<int> CreateAsync(Profil profil);
    Task UpdateAsync(Profil profil);
}
