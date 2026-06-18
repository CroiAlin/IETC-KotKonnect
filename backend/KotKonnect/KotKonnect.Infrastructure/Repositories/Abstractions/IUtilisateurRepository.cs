namespace KotKonnect.Infrastructure.Repositories.Abstractions;

using KotKonnect.Infrastructure.Models;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByEmailAsync(string email);
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(Utilisateur utilisateur);
}
