using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteDomainUnitTests;

public class EtudiantUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        string numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";

        // On crée l'étudiant qui doit être ajouté en base (sans Id)
        Etudiant etudiantSansId = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };

        // Création du mock du repository Etudiant
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();

        // Simulation de la fonction FindByConditionAsync : l'étudiant n'existe pas
        mockEtudiantRepo
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>());

        // Simulation de CreateAsync : on retourne l'étudiant avec un Id
        Etudiant etudiantCree = new Etudiant { Id = id, NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        mockEtudiantRepo
            .Setup(repo => repo.CreateAsync(etudiantSansId))
            .ReturnsAsync(etudiantCree);

        // Création du mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory
            .Setup(factory => factory.EtudiantRepository())
            .Returns(mockEtudiantRepo.Object);

        // Création du use case en utilisant la factory mockée
        CreateEtudiantUseCase useCase = new CreateEtudiantUseCase(mockFactory.Object);

        // Appel du use case
        var etudiantTeste = await useCase.ExecuteAsync(etudiantSansId);

        // Vérification du résultat
        Assert.That(etudiantTeste.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(etudiantTeste.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(etudiantTeste.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(etudiantTeste.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(etudiantTeste.Email, Is.EqualTo(etudiantCree.Email));
    }
}
