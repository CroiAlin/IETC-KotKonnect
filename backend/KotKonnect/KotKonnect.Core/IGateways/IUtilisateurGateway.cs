namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Models;

public interface IUtilisateurGateway
{
    Task<Utilisateur?> GetByEmailAsync(string email);
    Task<Utilisateur?> GetByIdAsync(int id);
    // Hash récupéré à part : jamais exposé dans un modèle du Core.
    Task<string?> GetPasswordHashAsync(string email);
    Task<int> CreateAsync(Utilisateur utilisateur, string passwordHash);
}
