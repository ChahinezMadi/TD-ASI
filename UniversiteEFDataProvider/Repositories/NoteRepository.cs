using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task AffecterNoteAsync(long idNote, long idEtudiant, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        var note = await Context.Notes.FindAsync(idNote)
                   ?? throw new InvalidOperationException($"Note {idNote} introuvable.");

        var etudiant = await Context.Etudiants.FindAsync(idEtudiant)
                      ?? throw new InvalidOperationException($"Etudiant {idEtudiant} introuvable.");

        var ue = await Context.Ues.FindAsync(idUe)
                 ?? throw new InvalidOperationException($"UE {idUe} introuvable.");

        //Nouveau modèle : 1 note -> 1 étudiant / 1 UE
        note.EtudiantId = etudiant.Id;
        note.Etudiant = etudiant;

        note.UeId = ue.Id;
        note.Ue = ue;

        await Context.SaveChangesAsync();
    }

    public async Task AffecterParcoursAsync(long idNote, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(Context.Parcours);

        var note = await Context.Notes.FindAsync(idNote)
                   ?? throw new InvalidOperationException($"Note {idNote} introuvable.");

        var parcours = await Context.Parcours.FindAsync(idParcours)
                      ?? throw new InvalidOperationException($"Parcours {idParcours} introuvable.");

        //Optionnel : si tu as ParcoursId/Parcours dans Note
        note.ParcoursId = parcours.Id;
        note.Parcours = parcours;

        await Context.SaveChangesAsync();
    }

    public async Task<Note?> FindNoteByEtudiantUeAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);

        return await Context.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
    }
}
