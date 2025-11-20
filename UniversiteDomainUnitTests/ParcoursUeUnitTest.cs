using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTests;

public class ParcoursUeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 1;
        long idParcours = 3;

        Ue ue = new Ue
        {
            Id = idUe,
            NumeroUe = "UE101",
            Intitule = "Programmation C#"
        };

        Parcours parcours = new Parcours
        {
            Id = idParcours,
            NomParcours = "Parcours 1",
            AnneeFormation = 1
        };

        // Très important pour ne pas déclencher DuplicateNumeroUeException
        // vu la logique actuelle de CheckBusinessRules
        parcours.UesEnseignees = null;

        // On initialise des faux repositories
        var mockUeRepo = new Mock<IUeRepository>();
        var mockParcoursRepo = new Mock<IParcoursRepository>();

        // L'UE existe bien en base
        List<Ue> ues = new List<Ue> { ue };
        mockUeRepo
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idUe)))
            .ReturnsAsync(ues);

        // Le parcours existe bien en base
        List<Parcours> parcoursList = new List<Parcours> { parcours };
        mockParcoursRepo
            .Setup(repo => repo.FindByConditionAsync(p => p.Id.Equals(idParcours)))
            .ReturnsAsync(parcoursList);

        // On simule l'ajout de l'UE dans le parcours
        Parcours parcoursFinal = new Parcours
        {
            Id = idParcours,
            NomParcours = parcours.NomParcours,
            AnneeFormation = parcours.AnneeFormation,
            UesEnseignees = new List<Ue> { ue }
        };

        mockParcoursRepo
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);

        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);

        // Création du use case en utilisant le mock comme datasource
        AddUeDansParcoursUseCase useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        // Appel du use case
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);

        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees!.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }
}
