namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long Id { get; set; }
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // ManyToOne : l'étudiant est inscrit dans un parcours
    public Parcours? ParcoursSuivi { get; set; } = null;
    
    // ManyToMany : un Etudiant a plusieurs notes.  
    public List<Note>? NotesEt { get; set; } = new();  

    public override string ToString()
    {
        return $"ID {Id} : {NumEtud} - {Nom} {Prenom} inscrit en "+ParcoursSuivi;
    }    
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<Note> NotesObtenues { get; set; } = new List<Note>();
    public List<Ue> Ues { get; set; } = new List<Ue>();
}