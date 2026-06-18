namespace KotKonnect.Core.UseCases.Abstractions;

using KotKonnect.Core.Models;

public interface IAuthUseCases
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(string refreshToken);
}
