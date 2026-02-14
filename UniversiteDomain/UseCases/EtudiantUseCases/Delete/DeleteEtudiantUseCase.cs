using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    // Suppression par Id
    public async Task ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules(idEtudiant);

        var repo = repositoryFactory.EtudiantRepository();

        // On supprime directement par id (si ton repo le supporte)
        await repo.DeleteAsync(idEtudiant);

        await repositoryFactory.SaveChangesAsync();
    }

    // Suppression par NumEtud (pratique pour REST)
    public async Task ExecuteByNumEtudAsync(string numEtud)
    {
        ArgumentNullException.ThrowIfNull(numEtud);

        var repo = repositoryFactory.EtudiantRepository();

        var found = await repo.FindByConditionAsync(e => e.NumEtud == numEtud);
        if (found.Count == 0)
            throw new InvalidOperationException($"{numEtud} - étudiant introuvable");

        // Si jamais il y a plusieurs (normalement non), on prend le 1er
        await repo.DeleteAsync(found[0]);

        await repositoryFactory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(long idEtudiant)
    {
        if (idEtudiant <= 0)
            throw new ArgumentException("Id étudiant invalide", nameof(idEtudiant));

        var repo = repositoryFactory.EtudiantRepository();

        var etu = await repo.FindAsync(idEtudiant);
        if (etu == null)
            throw new InvalidOperationException($"{idEtudiant} - étudiant introuvable");

        // Si tu veux bloquer la suppression si l’étudiant a des notes, tu peux faire :
        // if (etu.Notes != null && etu.Notes.Count > 0)
        //     throw new InvalidOperationException("Impossible de supprimer : l'étudiant possède des notes.");
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}