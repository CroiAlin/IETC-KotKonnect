namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Models;

public interface IProfilGateway
{
    Task<Profil?> GetByUtilisateurIdAsync(int utilisateurId);
    Task<int> CreateAsync(Profil profil);
    Task UpdateAsync(Profil profil);
}
