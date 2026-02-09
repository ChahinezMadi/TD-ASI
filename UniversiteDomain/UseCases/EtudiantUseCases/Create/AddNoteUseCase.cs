using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteApplication.UseCases.EtudiantUseCases.Create;

public class AddNoteUseCase(IEtudiantRepository etudiantRepository)
{
    private readonly IEtudiantRepository _etudiantRepository = etudiantRepository;

    public async Task<Etudiant> ExecuteAsync(long idEtudiant, long idNote)
    {
        return await _etudiantRepository.AddNoteAsync(idEtudiant, idNote);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, Note note)
    {
        return await _etudiantRepository.AddNoteAsync(etudiant, note);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, List<Note> notes)
    {
        return await _etudiantRepository.AddNoteAsync(etudiant, notes);
    }

    public async Task<Etudiant> ExecuteAsync(long idEtudiant, long[] idNotes)
    {
        return await _etudiantRepository.AddNoteAsync(idEtudiant, idNotes);
    }
}