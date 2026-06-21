namespace KotKonnect.Infrastructure;

using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Data;
using KotKonnect.Infrastructure.Gateways;
using KotKonnect.Infrastructure.Repositories;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using KotKonnect.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MySQL (fabrique de connexion)
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Chaîne de connexion 'Default' manquante.");
        services.AddSingleton<IDbConnectionFactory>(new MySqlConnectionFactory(connectionString));

        // Repositories
        services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
        services.AddScoped<IProfilRepository, ProfilRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBienRepository, BienRepository>();
        services.AddScoped<ICandidatureRepository, CandidatureRepository>();

        // Gateways (implémentent les ports du Core)
        services.AddScoped<IUtilisateurGateway, UtilisateurGateway>();
        services.AddScoped<IProfilGateway, ProfilGateway>();
        services.AddScoped<IRefreshTokenGateway, RefreshTokenGateway>();
        services.AddScoped<IBienGateway, BienGateway>();
        services.AddScoped<ICandidatureGateway, CandidatureGateway>();

        // Sécurité (hachage de mot de passe ; le JWT est géré côté Api)
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}
