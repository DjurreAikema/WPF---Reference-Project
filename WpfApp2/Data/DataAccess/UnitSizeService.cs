using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data.DataAccess;

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

    public async Task<List<UnitSize>> GetByParentIdAsync(int snackId)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during GetByParentIdAsync");

        await using var context = CreateDbContext();
        var unitSizes = await context.UnitSizes
            .Where(u => u.SnackId == snackId)
            .ToListAsync();

        // Create clean objects without circular references
        return unitSizes.Select(us => new UnitSize
        {
            Id = us.Id,
            SnackId = us.SnackId,
            Name = us.Name,
            Price = us.Price,
            Quantity = us.Quantity,
            Description = us.Description
        }).ToList();
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