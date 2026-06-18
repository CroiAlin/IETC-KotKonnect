namespace KotKonnect.Core;

using KotKonnect.Core.UseCases;
using KotKonnect.Core.UseCases.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthUseCases, AuthUseCases>();
        services.AddScoped<IBienUseCases, BienUseCases>();
        services.AddScoped<ICandidatureUseCases, CandidatureUseCases>();
        services.AddScoped<IProfilUseCases, ProfilUseCases>();

        return services;
    }
}
