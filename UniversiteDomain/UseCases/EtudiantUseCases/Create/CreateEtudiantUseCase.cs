using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

public class CreateEtudiantUseCase 
{
    private readonly IEtudiantRepository _etudiantRepository;

    public CreateEtudiantUseCase(IRepositoryFactory repositoryFactory)
    {
        _etudiantRepository = repositoryFactory.EtudiantRepository();
    }

    // Méthode principale pour créer un étudiant
    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        return await ExecuteAsync(etudiant);
    }

    // Variante avec un objet Etudiant déjà créé
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        Etudiant et = await _etudiantRepository.CreateAsync(etudiant);
        await _etudiantRepository.SaveChangesAsync();
        return et;
    }

    // Vérification des règles métier pour l'étudiant    

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(_etudiantRepository);

        // Vérification si l'étudiant avec le même numéro existe déjà
        var existe = await _etudiantRepository.FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
        if (existe.Any())
            throw new DuplicateNumEtudException(etudiant.NumEtud + " - ce numéro d'étudiant est déjà affecté à un étudiant");

        // Vérification du format de l'email
        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException(etudiant.Email + " - Email mal formé");

        // Vérification si l'email existe déjà
        existe = await _etudiantRepository.FindByConditionAsync(e => e.Email.Equals(etudiant.Email));
        if (existe?.Count > 0)
            throw new DuplicateEmailException(etudiant.Email + " est déjà affecté à un étudiant");

        // Vérification que le nom de l'étudiant est valide
        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException(etudiant.Nom + " incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
}
