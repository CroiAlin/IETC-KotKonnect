namespace KotKonnect.Core.Entities;

public class Etudiant : Utilisateur
{
    public string? Ecole { get; set; }
    public decimal? BudgetMax { get; set; }

}