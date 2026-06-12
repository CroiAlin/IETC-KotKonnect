namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByEmailAsync(string email);
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(Utilisateur utilisateur);
}

