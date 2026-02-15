using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.Dtos.BulkNotes;

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
    public async Task<List<BulkNoteCsvRowDto>> GetCsvTemplateRowsForUeAsync(long idUe)
    {
        // On récupère l'UE
        var ue = await Context.Ues!.FirstOrDefaultAsync(u => u.Id == idUe);
        if (ue == null) throw new Exception("UE introuvable.");

        // Tous les étudiants dont le parcours enseigne l'UE
        var etudiants = await Context.Etudiants!
            .Include(e => e.ParcoursSuivi)
            .ThenInclude(p => p.UesEnseignees)
            .Where(e => e.ParcoursSuivi != null &&
                        e.ParcoursSuivi.UesEnseignees.Any(u => u.Id == idUe))
            .ToListAsync();

        // Notes existantes pour cette UE
        var notes = await Context.Notes!
            .Where(n => n.UeId == idUe)
            .ToListAsync();

        var noteByEtudId = notes.ToDictionary(n => n.EtudiantId, n => n.Value);

        return etudiants
            .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
            .Select(e => new BulkNoteCsvRowDto
            {
                NumeroUe = ue.NumeroUe,
                IntituleUe = ue.Intitule,
                NumEtud = e.NumEtud,
                Nom = e.Nom,
                Prenom = e.Prenom,
                Note = noteByEtudId.TryGetValue(e.Id, out var val) ? val.ToString() : null
            })
            .ToList();
    }
    public async Task ApplyNotesForUeAsync(long idUe, List<(long etudiantId, decimal? note)> notes)
    {
        using var trx = await Context.Database.BeginTransactionAsync();

        try
        {
            foreach (var (idEtud, noteVal) in notes)
            {
                var existing = await Context.Notes!
                    .FirstOrDefaultAsync(n => n.EtudiantId == idEtud && n.UeId == idUe);

                if (noteVal == null)
                {
                    // Choix métier : si vide, on supprime la note existante (option).
                    // Si tu préfères "ne rien changer", remplace par "continue;"
                    if (existing != null)
                        Context.Notes.Remove(existing);
                    continue;
                }

                if (existing == null)
                {
                    Context.Notes.Add(new Note
                    {
                        EtudiantId = idEtud,
                        UeId = idUe,
                        Value = (double)noteVal.Value
                    });
                }
                else
                {
                    existing.Value = (double)noteVal.Value;
                }
            }

            await Context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        catch
        {
            await trx.RollbackAsync();
            throw;
        }
    }
    public async Task<List<Etudiant>> GetEtudiantsByUeAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        return await Context.Etudiants
            .Include(e => e.Notes)
            .ThenInclude(n => n.Ue)
            .Include(e => e.Ues) // si ta relation Etudiant<->Ue existe déjà en base
            .Where(e => e.Ues.Any(u => u.Id == ueId))
            .ToListAsync();
    }

}