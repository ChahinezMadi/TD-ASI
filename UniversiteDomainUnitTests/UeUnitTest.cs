using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using NUnit.Framework;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase()
    {
        long idUe = 1;
        string numeroUe = "UE101";
        string intitule = "Programmation C#";

        // UE à ajouter
        Ue ueAvant = new Ue { NumeroUe = numeroUe, Intitule = intitule };

        // Mock du IUeRepository
        var mockUeRepo = new Mock<IUeRepository>();

        // On simule : aucune UE n'existe déjà avec ce numéro
        mockUeRepo
            .Setup(repo => repo.FindByConditionAsync(u => u.NumeroUe.Equals(numeroUe)))
            .ReturnsAsync((List<Ue>)null);

        // On simule la création qui renvoie une UE avec Id = 1
        Ue ueFinal = new Ue { Id = idUe, NumeroUe = numeroUe, Intitule = intitule };
        mockUeRepo
            .Setup(repo => repo.CreateAsync(ueAvant))
            .ReturnsAsync(ueFinal);

        // Mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(facto => facto.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Use case
        var useCase = new CreateUeUseCase(mockFactory.Object);

        // Act
        var ueTestee = await useCase.ExecuteAsync(ueAvant);

        // Assert
        Assert.That(ueTestee.Id, Is.EqualTo(ueFinal.Id));
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(ueFinal.NumeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(ueFinal.Intitule));
    }
}