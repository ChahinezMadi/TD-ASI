namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public double Value { get; set; }

    // FK Etudiant
    public long EtudiantId { get; set; }
    public Etudiant Etudiant { get; set; } = null!;

    // FK Ue
    public long UeId { get; set; }
    public Ue Ue { get; set; } = null!;

    // (Optionnel) si tu veux lier au parcours
    public long? ParcoursId { get; set; }
    public Parcours? Parcours { get; set; }
}