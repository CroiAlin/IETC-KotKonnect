namespace KotKonnect.Api.Extensions;

using System.Security.Claims;

// ID du connecté, lu dans le JWT (claim NameIdentifier).
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
