using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;

namespace UniversiteApplication.UseCases.EtudiantUseCases.Create;

public class AddUeUseCase(IEtudiantRepository etudiantRepository)
{
    private readonly IEtudiantRepository _etudiantRepository = etudiantRepository;

    public async Task<Etudiant> ExecuteAsync(long idEtudiant, long idUe)
    {
        return await _etudiantRepository.AddUeAsync(idEtudiant, idUe);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, Ue ue)
    {
        return await _etudiantRepository.AddUeAsync(etudiant, ue);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, List<Ue> ues)
    {
        return await _etudiantRepository.AddUeAsync(etudiant, ues);
    }

    public async Task<Etudiant> ExecuteAsync(long idEtudiant, long[] idUes)
    {
        return await _etudiantRepository.AddUeAsync(idEtudiant, idUes);
    }
}