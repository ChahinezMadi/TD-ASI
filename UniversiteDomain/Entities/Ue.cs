namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;

    public List<Parcours> EnseigneeDans { get; set; } = new();

    // One-to-many : une UE a plusieurs notes
    public ICollection<Note> Notes { get; set; } = new List<Note>();

    public override string ToString()
        => "ID " + Id + " : " + NumeroUe + " - " + Intitule;
}