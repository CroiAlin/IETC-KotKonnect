namespace KotKonnect.Core.Entities;

public class Photo
{
    public int PhotoID { get; set; }
    public int BienID { get; set; }
    public string UrlImage { get; set; } = string.Empty;
    public int Ordre { get; set; }
}
