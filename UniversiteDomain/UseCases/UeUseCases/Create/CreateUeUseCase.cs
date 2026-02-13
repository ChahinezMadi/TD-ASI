using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue { NumeroUe = numeroUe, Intitule = intitule };
        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);

        var repo = repositoryFactory.UeRepository();

        Ue created = await repo.CreateAsync(ue);
        repositoryFactory.SaveChangesAsync().Wait(); // même style que CreateParcoursUseCase

        return created;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);

        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());

        var repo = repositoryFactory.UeRepository();

        var existing = await repo.FindByConditionAsync(u => u.NumeroUe == ue.NumeroUe);
        if (existing is { Count: > 0 })
            throw new DuplicateUeDansParcoursException("Unité d'enseignement avec ce numéro existe déjà.");

        if (ue.Intitule.Length < 3)
            throw new UeNotFoundException("L'intitulé de l'UE doit contenir au moins 3 caractères.");
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}