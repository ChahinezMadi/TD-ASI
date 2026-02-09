using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Data;
 
public class UniversiteDbContext : DbContext
{
    public static readonly ILoggerFactory consoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }
 
    public UniversiteDbContext():base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(consoleLogger)  //on lie le contexte avec le système de journalisation
            .EnableSensitiveDataLogging() 
            .EnableDetailedErrors();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Note>()
            .HasOne(n => n.Etudiant)
            .WithMany(e => e.Notes)
            .HasForeignKey(n => n.EtudiantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Note>()
            .HasOne(n => n.Ue)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bonus : empêche 2 notes pour même étudiant dans même UE
        modelBuilder.Entity<Note>()
            .HasIndex(n => new { n.EtudiantId, n.UeId })
            .IsUnique();
    }
    public DbSet <Parcours>? Parcours { get; set; }
    public DbSet <Etudiant>? Etudiants { get; set; }
    public DbSet <Ue>? Ues { get; set; }
    public DbSet <Note>? Notes { get; set; }
}