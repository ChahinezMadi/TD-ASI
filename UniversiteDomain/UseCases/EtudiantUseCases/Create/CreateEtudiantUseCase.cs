using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

public class CreateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        return await ExecuteAsync(etudiant);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);

        var repo = repositoryFactory.EtudiantRepository();
        Etudiant created = await repo.CreateAsync(etudiant);

        await repositoryFactory.SaveChangesAsync(); // ✅ cohérent partout
        return created;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);

        var repo = repositoryFactory.EtudiantRepository();

        var existe = await repo.FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
        if (existe.Any())
            throw new DuplicateNumEtudException(etudiant.NumEtud + " - ce numéro d'étudiant est déjà affecté à un étudiant");

        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException(etudiant.Email + " - Email mal formé");

        existe = await repo.FindByConditionAsync(e => e.Email.Equals(etudiant.Email));
        if (existe.Count > 0)
            throw new DuplicateEmailException(etudiant.Email + " est déjà affecté à un étudiant");

        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException(etudiant.Nom + " incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}
