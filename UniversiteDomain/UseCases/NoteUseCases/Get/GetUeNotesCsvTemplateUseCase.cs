using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.BulkNotes;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetUeNotesCsvTemplateUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);

    public async Task<List<BulkNoteCsvRowDto>> ExecuteAsync(long idUe)
    {
        ArgumentNullException.ThrowIfNull(factory);
        var repo = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(repo);

        return await repo.GetCsvTemplateRowsForUeAsync(idUe);
    }
}