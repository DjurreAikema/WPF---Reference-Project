using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data;

public class AppDbContext : DbContext
{
    // DbSet properties for each entity
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Snack> Snacks { get; set; } = null!;
    public DbSet<UnitSize> UnitSizes { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;

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

        // Configure the many-to-many relationship using Inventory
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Snack)
            .WithMany(s => s.Inventories)
            .HasForeignKey(i => i.SnackId);

        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Warehouse)
            .WithMany(w => w.Inventories)
            .HasForeignKey(i => i.WarehouseId);

        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.UnitSize)
            .WithMany()
            .HasForeignKey(i => i.UnitSizeId);

        // Snack -> UnitSizes (one-to-many)
        modelBuilder.Entity<UnitSize>()
            .HasOne<Snack>()
            .WithMany(s => s.UnitSizes)
            .HasForeignKey(s => s.SnackId);

        // Configure table names
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");
        modelBuilder.Entity<Snack>().ToTable("Snacks");
        modelBuilder.Entity<UnitSize>().ToTable("UnitSizes");
        modelBuilder.Entity<Inventory>().ToTable("Inventories");
    }
}