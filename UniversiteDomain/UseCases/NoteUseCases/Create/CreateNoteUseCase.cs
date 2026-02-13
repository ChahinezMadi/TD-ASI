using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // Règle métier : note entre 0 et 20
        if (valeur < 0 || valeur > 20)
            throw new ArgumentException("La note doit être comprise entre 0 et 20.");

        var etudiantRepo = repositoryFactory.EtudiantRepository();
        var ueRepo = repositoryFactory.UeRepository();
        var noteRepo = repositoryFactory.NoteRepository();

        // Charger Etudiant
        var etudiants = await etudiantRepo.FindByConditionAsync(e => e.Id == etudiantId);
        var etudiant = etudiants.FirstOrDefault()
            ?? throw new InvalidOperationException($"Etudiant {etudiantId} introuvable.");

        // Charger UE
        var ues = await ueRepo.FindByConditionAsync(u => u.Id == ueId);
        var ue = ues.FirstOrDefault()
            ?? throw new InvalidOperationException($"UE {ueId} introuvable.");

        // Vérifier qu'il n'existe pas déjà une note pour cet étudiant dans cette UE
        // (adapte le nom de méthode si ton repo est différent)
        var existingNote = await noteRepo.FindNoteByEtudiantUeAsync(etudiantId, ueId);
        if (existingNote != null)
            throw new InvalidOperationException("L'étudiant a déjà une note dans cette UE.");

        // Vérifier si l'étudiant est inscrit à l'UE (si la nav-prop est dispo)
        // -> si tes nav props ne sont pas chargées automatiquement, il faudra une méthode repo dédiée.
        if (etudiant.Ues != null && etudiant.Ues.Count > 0)
        {
            if (!etudiant.Ues.Any(u => u.Id == ueId))
                throw new InvalidOperationException("L'étudiant n'est pas inscrit dans cette UE.");
        }

        var noteToCreate = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Value = valeur
        };

        var created = await noteRepo.CreateAsync(noteToCreate);
        repositoryFactory.SaveChangesAsync().Wait();

        return created;
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
