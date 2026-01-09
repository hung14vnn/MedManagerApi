using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MedManagerApi.Models;

namespace MedManagerApi.Data;

public class MedManagerDbContext : IdentityDbContext<ApplicationUser>
{
    public MedManagerDbContext(DbContextOptions<MedManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Drug> Drugs { get; set; }
    public DbSet<DrugInteraction> DrugInteractions { get; set; }
    public DbSet<DrugReference> DrugReferences { get; set; }
    public DbSet<InteractionReference> InteractionReferences { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<DiseaseProtocol> DiseaseProtocols { get; set; }
    public DbSet<DoseCalculation> DoseCalculations { get; set; }
    public DbSet<CounselingChecklist> CounselingChecklists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Drug Configuration
        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActiveIngredient).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BrandName).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.ActiveIngredient);
            entity.HasIndex(e => e.BrandName);
            entity.HasIndex(e => e.PharmacologicalGroup);
        });

        // DrugInteraction Configuration
        modelBuilder.Entity<DrugInteraction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Drug1)
                .WithMany(d => d.InteractionsAsDrug1)
                .HasForeignKey(e => e.Drug1Id)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Drug2)
                .WithMany(d => d.InteractionsAsDrug2)
                .HasForeignKey(e => e.Drug2Id)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasIndex(e => new { e.Drug1Id, e.Drug2Id }).IsUnique();
            entity.Property(e => e.Severity).HasConversion<string>();
        });

        // DrugReference Configuration
        modelBuilder.Entity<DrugReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Drug)
                .WithMany(d => d.References)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // InteractionReference Configuration
        modelBuilder.Entity<InteractionReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Interaction)
                .WithMany(i => i.References)
                .HasForeignKey(e => e.InteractionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Disease Configuration
        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IcdCode);
        });

        // DiseaseProtocol Configuration
        modelBuilder.Entity<DiseaseProtocol>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Disease)
                .WithMany(d => d.TreatmentProtocols)
                .HasForeignKey(e => e.DiseaseId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Drug)
                .WithMany(d => d.DiseaseProtocols)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.DiseaseId, e.DrugId });
        });

        // DoseCalculation Configuration
        modelBuilder.Entity<DoseCalculation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Drug)
                .WithMany()
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CounselingChecklist Configuration
        modelBuilder.Entity<CounselingChecklist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Drug)
                .WithMany()
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
