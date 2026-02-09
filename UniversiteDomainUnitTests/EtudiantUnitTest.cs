using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteDomainUnitTests;

public class EtudiantUnitTest
{
    [Test]
    public async Task CreateEtudiantUseCase_CreeEtudiant_SiPasDeDoublon()
    {
        // Arrange
        long id = 1;
        string numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";

        var etudiantSansId = new Etudiant
        {
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        var etudiantCree = new Etudiant
        {
            Id = id,
            NumEtud = numEtud,
            Nom = nom,
            Prenom = prenom,
            Email = email
        };

        // Mock du repository Etudiant
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();

        // FindByConditionAsync -> renvoie liste vide (pas de doublon numEtud / email)
        mockEtudiantRepo
            .Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant>());

        // CreateAsync -> renvoie l'étudiant avec un Id
        mockEtudiantRepo
            .Setup(r => r.CreateAsync(It.IsAny<Etudiant>()))
            .ReturnsAsync(etudiantCree);

        // SaveChangesAsync -> fait rien (mais doit exister)
        mockEtudiantRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory
            .Setup(f => f.EtudiantRepository())
            .Returns(mockEtudiantRepo.Object);

        // Use case avec factory mockée
        var useCase = new CreateEtudiantUseCase(mockFactory.Object);

        // Act
        var etudiantTeste = await useCase.ExecuteAsync(etudiantSansId);

        // Assert (valeurs)
        Assert.That(etudiantTeste.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(etudiantTeste.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(etudiantTeste.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(etudiantTeste.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(etudiantTeste.Email, Is.EqualTo(etudiantCree.Email));

        // Assert (appels)
        mockEtudiantRepo.Verify(r => r.CreateAsync(It.IsAny<Etudiant>()), Times.Once);
        mockEtudiantRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        mockFactory.Verify(f => f.EtudiantRepository(), Times.AtLeastOnce);
    }
}
