namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public double Value { get; set; }
    public Parcours parcours { get; set; }

    // OneToMany
    // Une note peut être attribuée à plusieurs étudiants
    public long EtudiantId { get; set; } // Clé étrangère vers Etudiant
    public Etudiant Etudiant { get; set; } // Propriété de navigation vers l'étudiant
    public List<Etudiant> recevoirNote { get; set; } = new(); // Liste des étudiants qui reçoivent cette note

    // ManyToOne
    // Les notes sont attribuées à une seule Ue
    public long UeId { get; set; } // Clé étrangère vers Ue
    public Ue notePourUe { get; set; } // Propriété de navigation vers Ue
}