namespace KotKonnect.Core.Services;

using KotKonnect.Core.DTOs;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Enums;
using KotKonnect.Core.Interfaces;

public class AuthService : IAuthService
{
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUtilisateurRepository utilisateurRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _utilisateurRepository = utilisateurRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existant = await _utilisateurRepository.GetByEmailAsync(request.Email);
        if (existant is not null)
            throw new InvalidOperationException("Cet email est déjà utilisé.");


        // Le rôle envoyé par le client doit être dans les enums
        if (!Enum.TryParse<RoleUtilisateur>(request.Role, out var role))
            throw new InvalidOperationException("Rôle invalide.");

        var utilisateur = new Utilisateur
        {
            Email = request.Email,
            MotDePasseHash = _passwordHasher.HashPassword(request.MotDePasse),
            Role = role,
            DateCreation = DateTime.UtcNow
        };

        utilisateur.UtilisateurID = await _utilisateurRepository.CreateAsync(utilisateur);

        return await BuildAuthResponseAsync(utilisateur);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var utilisateur = await _utilisateurRepository.GetByEmailAsync(request.Email);

        if (utilisateur is null || !_passwordHasher.VerifyPassword(request.MotDePasse, utilisateur.MotDePasseHash))
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

        return await BuildAuthResponseAsync(utilisateur);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var tokenEnBase = await _refreshTokenRepository.GetTokenAsync(refreshToken);

        if (tokenEnBase is null || tokenEnBase.Revoque || tokenEnBase.DateExpiration < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Token de rafraîchissement invalide ou expiré.");

        var utilisateur = await _utilisateurRepository.GetByIdAsync(tokenEnBase.UtilisateurID)
            ?? throw new UnauthorizedAccessException("Utilisateur introuvable");

        await _refreshTokenRepository.RevokeTokenByIdAsync(tokenEnBase.TokenID);

        return await BuildAuthResponseAsync(utilisateur);
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(Utilisateur utilisateur)
    {
        var accessToken = _tokenService.GenerateAccessToken(utilisateur);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.CreateTokenAsync(new RefreshToken
        {
            UtilisateurID = utilisateur.UtilisateurID,
            Token = refreshToken,
            DateExpiration = DateTime.UtcNow.AddDays(7),
            Revoque = false
        });

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = utilisateur.Email,
            Role = utilisateur.Role.ToString()
        };
    }
}