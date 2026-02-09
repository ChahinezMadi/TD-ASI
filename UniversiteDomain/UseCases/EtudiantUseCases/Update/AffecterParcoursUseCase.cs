using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteApplication.UseCases.EtudiantUseCases.Update;

public class AffecterParcoursUseCase(IEtudiantRepository etudiantRepository)
{
    private readonly IEtudiantRepository _etudiantRepository = etudiantRepository;

    public async Task ExecuteAsync(long idEtudiant, long idParcours)
    {
        await _etudiantRepository.AffecterParcoursAsync(idEtudiant, idParcours);
    }

    public async Task ExecuteAsync(Etudiant etudiant, Parcours parcours)
    {
        await _etudiantRepository.AffecterParcoursAsync(etudiant, parcours);
    }
}