namespace KotKonnect.Core.UseCases;

using System.Text.RegularExpressions;
using KotKonnect.Core.Enums;
using KotKonnect.Core.IGateways;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public class AuthUseCases : IAuthUseCases
{
    private readonly IUtilisateurGateway _utilisateurGateway;
    private readonly IRefreshTokenGateway _refreshTokenGateway;
    private readonly IProfilGateway _profilGateway;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthUseCases(
        IUtilisateurGateway utilisateurGateway,
        IRefreshTokenGateway refreshTokenGateway,
        IProfilGateway profilGateway,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _utilisateurGateway = utilisateurGateway;
        _refreshTokenGateway = refreshTokenGateway;
        _profilGateway = profilGateway;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    // 6 caractères min, 1 minuscule, 1 majuscule, 1 chiffre.
    private static bool MotDePasseEstValide(string motDePasse) =>
        Regex.IsMatch(motDePasse, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (!MotDePasseEstValide(request.MotDePasse))
            throw new ArgumentException(
                "Le mot de passe doit contenir au moins 6 caractères, une minuscule, une majuscule et un chiffre.");

        var existant = await _utilisateurGateway.GetByEmailAsync(request.Email);
        if (existant is not null)
            throw new InvalidOperationException("Cet email est déjà utilisé.");

        if (!Enum.TryParse<RoleUtilisateur>(request.Role, out var role))
            throw new InvalidOperationException("Rôle invalide.");

        var utilisateur = new Utilisateur
        {
            Email = request.Email,
            Role = role,
            DateCreation = DateTime.UtcNow
        };
        var hash = _passwordHasher.HashPassword(request.MotDePasse);
        utilisateur.UtilisateurID = await _utilisateurGateway.CreateAsync(utilisateur, hash);

        // Profil créé après l'utilisateur (besoin de son ID auto-incrémenté).
        var profil = new Profil
        {
            UtilisateurID = utilisateur.UtilisateurID,
            Nom = request.Nom,
            Prenom = request.Prenom
        };
        await _profilGateway.CreateAsync(profil);

        return await BuildAuthResponseAsync(utilisateur);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var utilisateur = await _utilisateurGateway.GetByEmailAsync(request.Email);
        var hash = utilisateur is null ? null : await _utilisateurGateway.GetPasswordHashAsync(request.Email);

        if (utilisateur is null || hash is null || !_passwordHasher.VerifyPassword(request.MotDePasse, hash))
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

        return await BuildAuthResponseAsync(utilisateur);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var tokenEnBase = await _refreshTokenGateway.GetTokenAsync(refreshToken);

        if (tokenEnBase is null || tokenEnBase.Revoque || tokenEnBase.DateExpiration < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Token de rafraîchissement invalide ou expiré.");

        var utilisateur = await _utilisateurGateway.GetByIdAsync(tokenEnBase.UtilisateurID)
            ?? throw new UnauthorizedAccessException("Utilisateur introuvable.");

        // Rotation : on révoque l'ancien refresh token.
        await _refreshTokenGateway.RevokeTokenByIdAsync(tokenEnBase.TokenID);

        return await BuildAuthResponseAsync(utilisateur);
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(Utilisateur utilisateur)
    {
        var accessToken = _tokenService.GenerateAccessToken(utilisateur);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenGateway.CreateTokenAsync(new RefreshToken
        {
            UtilisateurID = utilisateur.UtilisateurID,
            Token = refreshToken,
            DateExpiration = DateTime.UtcNow.AddDays(7),
            Revoque = false
        });

        // Nom/prénom viennent du profil (pas de la table UTILISATEURS).
        var profil = await _profilGateway.GetByUtilisateurIdAsync(utilisateur.UtilisateurID);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = utilisateur.Email,
            Role = utilisateur.Role.ToString(),
            Nom = profil?.Nom ?? string.Empty,
            Prenom = profil?.Prenom ?? string.Empty
        };
    }
}
