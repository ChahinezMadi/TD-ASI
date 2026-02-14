using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    
    public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants
            .Include(e => e.ParcoursSuivi)             
            .Include(e => e.Notes).ThenInclude(n => n.Ue)
            .FirstOrDefaultAsync(e => e.Id == idEtudiant);
    }
    public async Task AffecterParcoursAsync(long idEtudiant, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        e.ParcoursSuivi = p;
        await Context.SaveChangesAsync();
    }
    
    public async Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        await AffecterParcoursAsync(etudiant.Id, parcours.Id); 
    }
    
    public async Task<Etudiant> AddNoteAsync(long idEtudiant, long idNote)
    {
        var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        var note = await Context.Notes.FindAsync(idNote);
        if (etudiant == null || note == null) throw new Exception("Étudiant ou Note introuvable.");
    
        etudiant.Notes.Add(note);
        await Context.SaveChangesAsync();
        return etudiant;
    }

    public async Task<Etudiant> AddNoteAsync(Etudiant etudiant, Note note)
    {
        return await AddNoteAsync(etudiant.Id, note.Id);
    }

    public async Task<Etudiant> AddNoteAsync(Etudiant? etudiant, List<Note> notes)
    {
        if (etudiant == null) throw new ArgumentNullException(nameof(etudiant));
    
        foreach (var note in notes)
            etudiant.Notes.Add(note);

        await Context.SaveChangesAsync();
        return etudiant;
    }

    public async Task<Etudiant> AddNoteAsync(long idEtudiant, long[] idNotes)
    {
        var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        var notes = await Context.Notes.Where(n => idNotes.Contains(n.Id)).ToListAsync();

        if (etudiant == null) throw new Exception("Étudiant introuvable.");

        foreach (var note in notes)
        {
            etudiant.Notes.Add(note);
        }

        await Context.SaveChangesAsync();
        return etudiant;
    }
    public async Task<Etudiant> AddUeAsync(long idEtudiant, long idUe)
    {
        var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        var ue = await Context.Ues.FindAsync(idUe);

        if (etudiant == null || ue == null) throw new Exception("Étudiant ou UE introuvable.");

        etudiant.Ues.Add(ue);
        await Context.SaveChangesAsync();
        return etudiant;
    }
    public async Task<Etudiant> AddUeAsync(Etudiant etudiant, Ue ue)
    {
        return await AddUeAsync(etudiant.Id, ue.Id);
    }
    public async Task<Etudiant> AddUeAsync(Etudiant? etudiant, List<Ue> ues)
    {
        if (etudiant == null) throw new ArgumentNullException(nameof(etudiant));

        foreach (var ue in ues)
        {
            etudiant.Ues.Add(ue);
        }

        await Context.SaveChangesAsync();
        return etudiant;
    }
    public async Task<Etudiant> AddUeAsync(long idEtudiant, long[] idUes)
    {
        var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        var ues = await Context.Ues.Where(u => idUes.Contains(u.Id)).ToListAsync();

        if (etudiant == null) throw new Exception("Étudiant introuvable.");

        etudiant.Ues.AddRange(ues);
        await Context.SaveChangesAsync();
        return etudiant;
    }
    public async Task<Etudiant?> GetByNumEtudAsync(string numEtud)
    {
        return await Context.Etudiants
            .FirstOrDefaultAsync(e => e.NumEtud == numEtud);
    }
    public async Task<IEnumerable<Etudiant>> GetByParcoursAsync(long idParcours)
    {
        return await Context.Etudiants
            .Where(e => e.ParcoursSuivi!.Id == idParcours)
            .ToListAsync();
    }
}