using Microsoft.EntityFrameworkCore;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Data;

public class AppDbContext : DbContext
{
    // DbSet properties for each entity
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Snack> Snacks { get; set; } = null!;
    public DbSet<UnitSize> UnitSizes { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Country -> Warehouses (one-to-many)
        modelBuilder.Entity<Warehouse>()
            .HasOne<Country>()
            .WithMany()
            .HasForeignKey(w => w.CountryId);

        // Warehouse -> Snacks (one-to-many)
        modelBuilder.Entity<Snack>()
            .HasOne<Warehouse>()
            .WithMany(w => w.Snacks)
            .HasForeignKey(s => s.WarehouseId);

        // Snack -> UnitSizes (one-to-many)
        modelBuilder.Entity<UnitSize>()
            .HasOne<Snack>()
            .WithMany(s => s.UnitSize)
            .HasForeignKey(s => s.SnackId);

        // Configure table names
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");
        modelBuilder.Entity<Snack>().ToTable("Snacks");
        modelBuilder.Entity<UnitSize>().ToTable("UnitSizes");
    }
}