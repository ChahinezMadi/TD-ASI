using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    public async Task AffecterUeAsync(long idUe, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
    
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
    
        ue.EnseigneeDans?.Add(p);
    
        await Context.SaveChangesAsync();
    }
    
    public async Task AffecterParcoursAsync(Ue Ue, Parcours parcours)
    {
        await AffecterUeAsync(Ue.Id, parcours.Id); 
    }

    public async Task<Ue?> GetByNumeroUeAsync(string numeroUe)
    {
        var ue = await Context.Ues.FirstOrDefaultAsync(u => u.NumeroUe == numeroUe);
        return ue;
    }
    public async Task<IEnumerable<Ue>> GetByParcoursAsync(long idParcours)
    {
        var parcours = await Context.Parcours.FindAsync(idParcours);
    
        if (parcours == null) throw new Exception("Parcours non trouvé.");

        return parcours.UesEnseignees;
    }
    public async Task<Ue> AddParcoursAsync(long idUe, long idParcours)
    {
        var ue = await Context.Ues.FindAsync(idUe);
        var parcours = await Context.Parcours.FindAsync(idParcours);

        if (ue == null) throw new Exception("UE non trouvée.");
        if (parcours == null) throw new Exception("Parcours non trouvé.");

        ue.EnseigneeDans.Add(parcours);
        await Context.SaveChangesAsync();
        return ue;
    }
    public async Task<Ue> AddParcoursAsync(Ue ue, Parcours parcours)
    {
        if (ue == null) throw new ArgumentNullException(nameof(ue), "UE ne peut pas être nulle.");
        if (parcours == null) throw new ArgumentNullException(nameof(parcours), "Parcours ne peut pas être nul.");

        ue.EnseigneeDans.Add(parcours);
        await Context.SaveChangesAsync();
        return ue;
    }
    public async Task<Ue> AddParcoursAsync(Ue? ue, List<Parcours> parcours)
    {
        if (ue == null) throw new ArgumentNullException(nameof(ue), "UE ne peut pas être nulle.");
        if (parcours == null || !parcours.Any()) throw new ArgumentNullException(nameof(parcours), "Liste des parcours ne peut pas être vide.");

        foreach (var p in parcours)
        {
            ue.EnseigneeDans.Add(p);
        }
        await Context.SaveChangesAsync();
        return ue;
    }
    public async Task<Ue> AddParcoursAsync(long idUe, long[] idParcours)
    {
        var ue = await Context.Ues.FindAsync(idUe);
        var parcoursList = await Context.Parcours.Where(p => idParcours.Contains(p.Id)).ToListAsync();

        if (ue == null) throw new Exception("UE non trouvée.");
        if (parcoursList == null || !parcoursList.Any()) throw new Exception("Parcours non trouvés.");

        foreach (var p in parcoursList)
        {
            ue.EnseigneeDans.Add(p);
        }
        await Context.SaveChangesAsync();
        return ue;
    }
    public async Task<Ue> AddNoteAsync(long idUe, long idNote)
    {
        var ue = await Context.Ues.FindAsync(idUe);
        var note = await Context.Notes.FindAsync(idNote);

        if (ue == null) throw new Exception("UE non trouvée.");
        if (note == null) throw new Exception("Note non trouvée.");

        ue.Notes.Add(note);
        await Context.SaveChangesAsync();
        return ue;
    }
    
    public async Task<Ue> AddNoteAsync(Ue ue, Note note)
    {
        if (ue == null) throw new ArgumentNullException(nameof(ue), "UE ne peut pas être nulle.");
        if (note == null) throw new ArgumentNullException(nameof(note), "Note ne peut pas être nulle.");

        ue.Notes.Add(note);
        await Context.SaveChangesAsync();
        return ue;
    }
    
    public async Task<Ue> AddNoteAsync(Ue? ue, List<Note> notes)
    {
        if (ue == null) throw new ArgumentNullException(nameof(ue), "UE ne peut pas être nulle.");
        if (notes == null || !notes.Any()) throw new ArgumentNullException(nameof(notes), "Liste des notes ne peut pas être vide.");

        foreach (var note in notes)
        {
            ue.Notes.Add(note);
        }
        await Context.SaveChangesAsync();
        return ue;
    }
    
    public async Task<Ue> AddNoteAsync(long idUe, long[] idNotes)
    {
        var ue = await Context.Ues.FindAsync(idUe);
        var notes = await Context.Notes.Where(n => idNotes.Contains(n.Id)).ToListAsync();

        if (ue == null) throw new Exception("UE non trouvée.");
        if (notes == null || !notes.Any()) throw new Exception("Notes non trouvées.");

        foreach (var note in notes)
        {
            ue.Notes.Add(note);
        }
        await Context.SaveChangesAsync();
        return ue;
    }
}