using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;

namespace WpfApp2.Data.DataAccess;

public class WarehouseService
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

    public async Task<List<Warehouse>> GetAllAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllWarehousesAsync");

        await using var context = CreateDbContext();
        return await context.Warehouses.ToListAsync();
    }

    public async Task<Warehouse> AddAsync(Warehouse warehouse)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddWarehouseAsync");

        await using var context = CreateDbContext();
        context.Warehouses.Add(warehouse);
        await context.SaveChangesAsync();
        return warehouse;
    }

    public async Task<Warehouse> UpdateAsync(Warehouse warehouse)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateWarehouseAsync");

        await using var context = CreateDbContext();
        context.Warehouses.Update(warehouse);
        await context.SaveChangesAsync();
        return warehouse;
    }

    public async Task<Warehouse?> DeleteAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteWarehouseAsync");

        await using var context = CreateDbContext();
        var warehouse = await context.Warehouses.FindAsync(id);
        if (warehouse == null) return warehouse;

        context.Warehouses.Remove(warehouse);
        await context.SaveChangesAsync();
        return warehouse;
    }
}