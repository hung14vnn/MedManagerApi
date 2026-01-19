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
    
    // New tables
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<DrugIngredient> DrugIngredients { get; set; }
    public DbSet<DosageForm> DosageForms { get; set; }
    public DbSet<RouteInformation> RouteInformations { get; set; }
    public DbSet<MechanismInformation> MechanismInformations { get; set; }
    public DbSet<IngredientMechanism> IngredientMechanisms { get; set; }
    public DbSet<InteractionMechanism> InteractionMechanisms { get; set; }
    public DbSet<SearchLog> SearchLogs { get; set; }

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
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasOne(e => e.DosageForm)
                .WithMany(d => d.Drugs)
                .HasForeignKey(e => e.DosageFormId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Route)
                .WithMany(r => r.Drugs)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Ingredient Configuration
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // DrugIngredient Configuration
        modelBuilder.Entity<DrugIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Drug)
                .WithMany(d => d.DrugIngredients)
                .HasForeignKey(e => e.DrugId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Ingredient)
                .WithMany(i => i.DrugIngredients)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.DrugId, e.IngredientId }).IsUnique();
        });

        // DosageForm Configuration
        modelBuilder.Entity<DosageForm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // RouteInformation Configuration
        modelBuilder.Entity<RouteInformation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // MechanismInformation Configuration
        modelBuilder.Entity<MechanismInformation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // IngredientMechanism Configuration
        modelBuilder.Entity<IngredientMechanism>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Ingredient)
                .WithMany(i => i.IngredientMechanisms)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Mechanism)
                .WithMany(m => m.IngredientMechanisms)
                .HasForeignKey(e => e.MechanismId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.IngredientId, e.MechanismId }).IsUnique();
        });

        // InteractionMechanism Configuration
        modelBuilder.Entity<InteractionMechanism>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Interaction)
                .WithMany(i => i.InteractionMechanisms)
                .HasForeignKey(e => e.InteractionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Mechanism)
                .WithMany()
                .HasForeignKey(e => e.MechanismId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.InteractionId, e.MechanismId });
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

        // SearchLog Configuration
        modelBuilder.Entity<SearchLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SearchQuery).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EntityType).HasConversion<string>();
            entity.HasIndex(e => e.SearchedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.SearchQuery);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
