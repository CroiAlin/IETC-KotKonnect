namespace KotKonnect.Core.DTOs;

// Entrée : le propriétaire fait passer une candidature à VU / ACCEPTE / REFUSE
public class UpdateStatutRequest
{
    public string Statut { get; set; } = string.Empty;
}
