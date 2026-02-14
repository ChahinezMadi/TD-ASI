using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);

        var repo = repositoryFactory.ParcoursRepository();

        Parcours p = await repo.CreateAsync(parcours);
        repositoryFactory.SaveChangesAsync().Wait(); // même style que ton code initial

        return p;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());

        var repo = repositoryFactory.ParcoursRepository();

        // Vérifier si un parcours avec le même nom et la même année existe déjà
        List<Parcours> existe = await repo.FindByConditionAsync(
            p => p.NomParcours.Equals(parcours.NomParcours) &&
                 p.AnneeFormation == parcours.AnneeFormation);

        if (existe is { Count: > 0 })
            throw new DuplicateParcoursException(
                parcours.NomParcours + " (" + parcours.AnneeFormation + ") - ce parcours existe déjà");

        // Règle métier : le nom du parcours doit contenir plus de 3 caractères
        if (parcours.NomParcours.Length < 2)
            throw new InvalidNomParcoursException(
                parcours.NomParcours + " incorrect - Le nom d'un parcours doit contenir plus de 2 caractères");

        // Règle métier : l’année de formation doit être entre 1 et 5
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 5)
            throw new InvalidAnneeFormationException(
                parcours.AnneeFormation + " incorrecte - L'année de formation doit être comprise entre 1 et 5");
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
