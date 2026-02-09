using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteApplication.UseCases.EtudiantUseCases.Get;

public class GetEtudiantByNumUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant?> ExecuteAsync(string numEtud)
    {
        var repo = repositoryFactory.EtudiantRepository();
        return await repo.GetByNumEtudAsync(numEtud);
    }
}