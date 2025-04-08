using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;

namespace WpfApp2.Data.DataAccess;

public class SnackService
{
    // -- Dependencies
    private readonly UnitSizeService _unitSizeService;
    private readonly InventoryService _inventoryService;

    // --- Properties
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.3;
    public double FailureProbabilityOnLoad { get; set; } = 0.3;
    private static readonly Random RandomGenerator = new();

    // --- Constructor
    public SnackService()
    {
        _unitSizeService = new UnitSizeService
        {
            SimulateFailures = false
        };

        _inventoryService = new InventoryService
        {
            SimulateFailures = false
        };
    }

    // --- Methods
    private static AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=Inventory.db");
        return new AppDbContext(optionsBuilder.Options);
    }

    public async Task<Snack> FillAsync(Snack snack)
    {
        if (snack.Id == null) return snack;

        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during FillSnackAsync");

        // UnitSizes
        snack.UnitSizes = new ObservableCollection<UnitSize>(await _unitSizeService.GetByParentIdAsync(snack.Id.Value));
        // Inventory
        snack.Inventories = new ObservableCollection<Inventory>(await _inventoryService.GetBySnackIdAsync(snack.Id.Value));

        return snack;
    }

    public async Task<List<Snack>> GetAllAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllSnacksAsync");

        await using var context = CreateDbContext();
        return await context.Snacks.ToListAsync();
    }

    public async Task<Snack> AddAsync(Snack snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<Snack> UpdateAsync(Snack snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<Snack?> DeleteAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteSnackAsync");

        await using var context = CreateDbContext();
        var snack = await context.Snacks.FindAsync(id);
        if (snack == null) return snack;

        context.Snacks.Remove(snack);
        await context.SaveChangesAsync();
        return snack;
    }
}