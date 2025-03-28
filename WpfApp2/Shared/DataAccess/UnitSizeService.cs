using Microsoft.EntityFrameworkCore;
using WpfApp2.Data;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Shared.DataAccess;

public class UnitSizeService
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

    public async Task<List<UnitSize>> GetAllAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllUnitSizesAsync");

        await using var context = CreateDbContext();
        return await context.UnitSizes.ToListAsync();
    }

    public async Task<UnitSize> AddAsync(UnitSize unitSize)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddUnitSizeAsync");

        await using var context = CreateDbContext();
        context.UnitSizes.Add(unitSize);
        await context.SaveChangesAsync();
        return unitSize;
    }

    public async Task<UnitSize> UpdateAsync(UnitSize unitSize)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateUnitSizeAsync");

        await using var context = CreateDbContext();
        context.UnitSizes.Update(unitSize);
        await context.SaveChangesAsync();
        return unitSize;
    }

    public async Task<UnitSize?> DeleteAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteUnitSizeAsync");

        await using var context = CreateDbContext();
        var unitSize = await context.UnitSizes.FindAsync(id);
        if (unitSize == null) return unitSize;

        context.UnitSizes.Remove(unitSize);
        await context.SaveChangesAsync();
        return unitSize;
    }
}