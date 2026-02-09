using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters
{
    public interface IUeRepository : IRepository<Ue>
    {
        // Associer un parcours à une UE (ManyToMany)
        Task<Ue> AddParcoursAsync(Ue ue, Parcours parcours);
        Task<Ue> AddParcoursAsync(long idUe, long idParcours);
        Task<Ue> AddParcoursAsync(Ue? ue, List<Parcours> parcours);
        Task<Ue> AddParcoursAsync(long idUe, long[] idParcours);
        
        Task<Ue> AddNoteAsync(long idUe, long idNote);
        Task<Ue> AddNoteAsync(Ue ue, Note note);
        Task<Ue> AddNoteAsync(Ue? ue, List<Note> notes);
        Task<Ue> AddNoteAsync(long idUe, long[] idNotes);
    }
}