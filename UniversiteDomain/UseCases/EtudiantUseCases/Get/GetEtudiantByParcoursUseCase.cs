using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteApplication.UseCases.EtudiantUseCases.Get;

public class GetEtudiantsByParcoursUseCase(IEtudiantRepository etudiantRepository)
{
    private readonly IEtudiantRepository _etudiantRepository = etudiantRepository;

    public async Task<IEnumerable<Etudiant>> ExecuteAsync(long idParcours)
    {
        return await _etudiantRepository.GetByParcoursAsync(idParcours);
    }
}