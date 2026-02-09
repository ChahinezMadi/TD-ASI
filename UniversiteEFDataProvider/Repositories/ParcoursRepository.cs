using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        if (parcours == null || etudiant == null) throw new ArgumentNullException("Le parcours ou l'étudiant est nul.");

        if (parcours.Inscrits == null) parcours.Inscrits = new List<Etudiant>();
        parcours.Inscrits.Add(etudiant);

        await Context.SaveChangesAsync();
        return parcours;
    }
    
        public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var etudiant = await Context.Etudiants.FindAsync(idEtudiant);

            if (parcours == null || etudiant == null) throw new Exception("Parcours ou étudiant introuvable.");

            if (parcours.Inscrits == null) parcours.Inscrits = new List<Etudiant>();
            parcours.Inscrits.Add(etudiant);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
        {
            if (parcours == null || etudiants == null || !etudiants.Any()) throw new ArgumentNullException("Le parcours ou la liste d'étudiants est nulle ou vide.");

            if (parcours.Inscrits == null) parcours.Inscrits = new List<Etudiant>();
            parcours.Inscrits.AddRange(etudiants);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var etudiants = await Context.Etudiants.Where(e => idEtudiants.Contains(e.Id)).ToListAsync();

            if (parcours == null || etudiants == null || !etudiants.Any()) throw new Exception("Parcours ou étudiants introuvables.");

            if (parcours.Inscrits == null) parcours.Inscrits = new List<Etudiant>();
            parcours.Inscrits.AddRange(etudiants);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var ue = await Context.Ues.FindAsync(idUe);

            if (parcours == null || ue == null) throw new Exception("Parcours ou UE introuvable.");

            if (parcours.UesEnseignees == null) parcours.UesEnseignees = new List<Ue>();
            parcours.UesEnseignees.Add(ue);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
        {
            if (parcours == null || ue == null) throw new ArgumentNullException("Le parcours ou l'UE est nul.");

            if (parcours.UesEnseignees == null) parcours.UesEnseignees = new List<Ue>();
            parcours.UesEnseignees.Add(ue);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
        {
            if (parcours == null || ues == null || !ues.Any()) throw new ArgumentNullException("Le parcours ou la liste des UEs est nulle ou vide.");

            if (parcours.UesEnseignees == null) parcours.UesEnseignees = new List<Ue>();
            parcours.UesEnseignees.AddRange(ues);

            await Context.SaveChangesAsync();
            return parcours;
        }

        public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
        {
            var parcours = await Context.Parcours.FindAsync(idParcours);
            var ues = await Context.Ues.Where(u => idUes.Contains(u.Id)).ToListAsync();

            if (parcours == null || ues == null || !ues.Any()) throw new Exception("Parcours ou UEs introuvables.");

            if (parcours.UesEnseignees == null) parcours.UesEnseignees = new List<Ue>();
            parcours.UesEnseignees.AddRange(ues);

            await Context.SaveChangesAsync();
            return parcours;
        }
}
