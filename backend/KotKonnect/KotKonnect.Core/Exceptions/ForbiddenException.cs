namespace KotKonnect.Core.Exceptions;

// Ownership : authentifié mais pas le droit sur CETTE ressource (-> 403).
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
