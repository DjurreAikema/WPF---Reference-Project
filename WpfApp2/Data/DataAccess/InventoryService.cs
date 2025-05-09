using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data.DataAccess;

public class InventoryService
{
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.3;
    public double FailureProbabilityOnLoad { get; set; } = 0.3;
    private static readonly Random RandomGenerator = new();

    private static AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=Inventory.db");
        return new AppDbContext(optionsBuilder.Options);
    }

    public async Task<List<Inventory>> GetBySnackIdAsync(int snackId)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during GetBySnackIdAsync");

        await using var context = CreateDbContext();
        var inventoryItems = await context.Inventories
            .Where(i => i.SnackId == snackId)
            .Include(i => i.Warehouse)
            .Include(i => i.UnitSize)
            .ToListAsync();

        // Create clean objects without circular references
        return inventoryItems.Select(item => new Inventory
        {
            Id = item.Id,
            SnackId = item.SnackId,
            WarehouseId = item.WarehouseId,
            UnitSizeId = item.UnitSizeId,
            Quantity = item.Quantity,
            Warehouse = new Warehouse
            {
                Id = item.Warehouse.Id,
                Name = item.Warehouse.Name
            },
            UnitSize = new UnitSize
            {
                Id = item.UnitSize?.Id,
                Name = item.UnitSize?.Name,
            }
        }).ToList();
    }

    public async Task<Inventory> AddAsync(Inventory inventory)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddUnitSizeAsync");

        await using var context = CreateDbContext();
        context.Inventories.Add(inventory);
        await context.SaveChangesAsync();

        // Include warehouse info for the response
        await context.Entry(inventory).Reference(i => i.Warehouse).LoadAsync();
        return inventory;
    }

    public async Task<Inventory> UpdateAsync(Inventory inventory)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateUnitSizeAsync");

        await using var context = CreateDbContext();
        context.Inventories.Update(inventory);
        await context.SaveChangesAsync();

        // Include warehouse info for the response
        await context.Entry(inventory).Reference(i => i.Warehouse).LoadAsync();
        return inventory;
    }

    public async Task<Inventory?> DeleteAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteUnitSizeAsync");

        await using var context = CreateDbContext();
        var inventory = await context.Inventories.FindAsync(id);
        if (inventory == null) return inventory;

        context.Inventories.Remove(inventory);
        await context.SaveChangesAsync();
        return inventory;
    }
}