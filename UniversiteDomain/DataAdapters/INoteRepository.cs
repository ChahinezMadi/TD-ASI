using System.Linq.Expressions;
using UniversiteDomain.Entities;
using System.Threading.Tasks;

namespace UniversiteDomain.DataAdapters
{
    public interface INoteRepository
    {
        Task<Note> CreateAsync(Note note);
        Task<Note?> FindNoteByEtudiantUeAsync(long etudiantId, long ueId);
        Task<List<Note>> FindByConditionAsync(Expression<Func<Note, bool>> condition);
        Task<List<Note>> FindAllAsync();
        Task SaveChangesAsync();
    }
}