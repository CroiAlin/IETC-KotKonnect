namespace KotKonnect.Infrastructure.Gateways;

using KotKonnect.Core.Enums;
using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using CoreModels = KotKonnect.Core.Models;
using InfraModels = KotKonnect.Infrastructure.Models;

public class UtilisateurGateway : IUtilisateurGateway
{
    private readonly IUtilisateurRepository _repository;

    public UtilisateurGateway(IUtilisateurRepository repository)
    {
        _repository = repository;
    }

    public async Task<CoreModels.Utilisateur?> GetByEmailAsync(string email)
    {
        var utilisateur = await _repository.GetByEmailAsync(email);
        return utilisateur is null ? null : ToCore(utilisateur);
    }

    public async Task<CoreModels.Utilisateur?> GetByIdAsync(int id)
    {
        var utilisateur = await _repository.GetByIdAsync(id);
        return utilisateur is null ? null : ToCore(utilisateur);
    }

    // Le hash exposé à part (jamais dans un modèle du Core).
    public async Task<string?> GetPasswordHashAsync(string email)
    {
        var utilisateur = await _repository.GetByEmailAsync(email);
        return utilisateur?.MotDePasseHash;
    }

    public async Task<int> CreateAsync(CoreModels.Utilisateur utilisateur, string passwordHash)
    {
        var infra = new InfraModels.Utilisateur
        {
            Email = utilisateur.Email,
            MotDePasseHash = passwordHash,
            Role = utilisateur.Role.ToString(),
            DateCreation = utilisateur.DateCreation
        };
        return await _repository.CreateAsync(infra);
    }

    // TryParse tolérant : Role absent quand l'étudiant est juste joint à une candidature.
    public static CoreModels.Utilisateur ToCore(InfraModels.Utilisateur u) => new()
    {
        UtilisateurID = u.UtilisateurID,
        Email = u.Email,
        Role = Enum.TryParse<RoleUtilisateur>(u.Role, out var role) ? role : default,
        DateCreation = u.DateCreation
    };
}
