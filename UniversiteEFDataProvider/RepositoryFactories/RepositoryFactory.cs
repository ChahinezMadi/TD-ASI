using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(UniversiteDbContext context) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;

    public IParcoursRepository ParcoursRepository()
    {
        _parcours ??= new ParcoursRepository(context ?? throw new InvalidOperationException());
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        _etudiants ??= new EtudiantRepository(context ?? throw new InvalidOperationException());
        return _etudiants;
    }

    public IUeRepository? UeRepository()
    {
        _ues ??= new UeRepository(context ?? throw new InvalidOperationException());
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        _notes ??= new NoteRepository(context ?? throw new InvalidOperationException());
        return _notes;
    }

    // Implémentation manquante demandée par l’interface
    public INoteRepository CreateNoteRepository()
    {
        return NoteRepository();
    }

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    public async Task EnsureCreatedAsync() => await context.Database.EnsureCreatedAsync();
    public async Task EnsureDeletedAsync() => await context.Database.EnsureDeletedAsync();

    // Ces méthodes n’ont pas vraiment leur place dans une factory, mais je les laisse
    // pour matcher ton interface sans casser le compile.
    public async Task<UniversiteDomain.Entities.Parcours> CreateAsync(UniversiteDomain.Entities.Parcours parcours)
    {
        context.Parcours.Add(parcours);
        await context.SaveChangesAsync();
        return parcours;
    }

    public async Task<object> FindByConditionAsync(Func<object, bool> func)
    {
        var result = context.Set<object>().FirstOrDefault((object)func);
        return result ?? throw new InvalidOperationException("Aucun objet trouvé correspondant à la condition.");
    }
}
