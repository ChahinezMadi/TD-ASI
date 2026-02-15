namespace UniversiteDomain.Dtos.BulkNotes;

public class BulkNoteCsvRowDto
{
    public string NumeroUe { get; set; } = string.Empty;
    public string IntituleUe { get; set; } = string.Empty;

    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;

    // Note texte : vide => null, sinon "12.5" etc.
    public string? Note { get; set; }
}