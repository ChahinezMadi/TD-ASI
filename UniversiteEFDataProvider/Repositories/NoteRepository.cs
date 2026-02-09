using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task AffecterNoteAsync(long idNote, long idEtudiant, long idUE)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        Note n = (await Context.Notes.FindAsync(idNote))!;
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Ue ue = (await Context.Ues.FindAsync(idUE))!;

        if (n == null || e == null || ue == null)
        {
            throw new Exception("Données invalides pour l'affectation.");
        }

        n.recevoirNote?.Add(e);
        n.notePourUe = ue;
        await Context.SaveChangesAsync();
    }

    public async Task AffecterParcoursAsync(Note note, Parcours parcours)
    {
        if (note == null)
        {
            throw new ArgumentNullException(nameof(note), "La note ne peut pas être nulle.");
        }

        if (parcours == null)
        {
            throw new ArgumentNullException(nameof(parcours), "Le parcours ne peut pas être nul.");
        }

        note.parcours = parcours;

        await Context.SaveChangesAsync();
    }

    public async Task<Note?> FindNoteByEtudiantUeAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);

        var note = await Context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

        return note;
    }
}