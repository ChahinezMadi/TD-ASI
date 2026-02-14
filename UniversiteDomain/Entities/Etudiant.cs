namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long Id { get; set; }
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Many-to-one : l'étudiant est inscrit dans un parcours
    public long? ParcoursId { get; set; }
    public Parcours? ParcoursSuivi { get; set; }

    // One-to-many : un étudiant a plusieurs notes
    public List<Note> Notes { get; set; } = new List<Note>();

    public List<Ue> Ues { get; set; } = new List<Ue>();

    public override string ToString()
        => $"ID {Id} : {NumEtud} - {Nom} {Prenom} inscrit en " + ParcoursSuivi;
}