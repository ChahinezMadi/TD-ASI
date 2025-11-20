using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create
{
    public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
    {
        public async Task<Ue> ExecuteAsync(string numero, string intitule)
        {
            var ue = new Ue { NumeroUe = numero, Intitule = intitule };
            return await ExecuteAsync(ue);
        }

        public async Task<Ue> ExecuteAsync(Ue ue)
        {
            await CheckBusinessRules(ue);

            var repo = repositoryFactory.UeRepository();

            Ue created = await repo.CreateAsync(ue);

            repositoryFactory.SaveChangesAsync().Wait(); // cohérent avec ton style existant

            return created;
        }

        private async Task CheckBusinessRules(Ue ue)
        {
            ArgumentNullException.ThrowIfNull(ue);
            ArgumentNullException.ThrowIfNull(ue.NumeroUe);
            ArgumentNullException.ThrowIfNull(ue.Intitule);

            var repo = repositoryFactory.UeRepository();

            // Vérifier unicité du numéro d'UE
            var existe = await repo.FindByConditionAsync(u =>
                u.NumeroUe.Equals(ue.NumeroUe));

            if (existe is { Count: > 0 })
                throw new DuplicateNumeroUeException(
                    $"{ue.NumeroUe} - ce numéro d'UE existe déjà");

            // Vérifier longueur intitulé
            if (ue.Intitule.Length <= 3)
                throw new InvalidIntituleUeException(
                    $"{ue.Intitule} incorrect - l'intitulé doit contenir plus de 3 caractères");
        }
    }
}