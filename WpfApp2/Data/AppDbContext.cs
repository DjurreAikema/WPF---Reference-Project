using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;
using WpfApp2.Data.Classes.Orders;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data;

public class AppDbContext : DbContext
{
    // --- DbSet properties for each entity
    // Stamdata
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Snack> Snacks { get; set; } = null!;
    public DbSet<UnitSize> UnitSizes { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;

    // Inbound
    public DbSet<InboundOrder> InboundOrders { get; set; } = null!;
    public DbSet<InboundOrderLine> InboundOrderLines { get; set; } = null!;
    public DbSet<InboundReceipt> InboundReceipts { get; set; } = null!;
    public DbSet<InboundReceiptLine> InboundReceiptLines { get; set; } = null!;

    // Outbound

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

        modelBuilder.Entity<Supplier>()
            .HasOne(s => s.Country)
            .WithMany()
            .HasForeignKey(s => s.CountryId);

        modelBuilder.Entity<InboundOrder>()
            .HasOne(o => o.Supplier)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SupplierId);

        modelBuilder.Entity<InboundOrderLine>()
            .HasOne(ol => ol.Order)
            .WithMany(o => o.OrderLines)
            .HasForeignKey(ol => ol.InboundOrderId);

        modelBuilder.Entity<InboundReceiptLine>()
            .HasOne(rl => rl.Receipt)
            .WithMany(r => r.ReceiptLines)
            .HasForeignKey(rl => rl.InboundReceiptId);

        // Configure table names
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");
        modelBuilder.Entity<Snack>().ToTable("Snacks");
        modelBuilder.Entity<UnitSize>().ToTable("UnitSizes");
        modelBuilder.Entity<Inventory>().ToTable("Inventories");
        modelBuilder.Entity<Supplier>().ToTable("Suppliers");
        modelBuilder.Entity<InboundOrder>().ToTable("InboundOrders");
        modelBuilder.Entity<InboundOrderLine>().ToTable("InboundOrderLines");
        modelBuilder.Entity<InboundReceipt>().ToTable("InboundReceipts");
        modelBuilder.Entity<InboundReceiptLine>().ToTable("InboundReceiptLines");
    }
}