using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Shared.DataAccess;

public class SnackServiceV2
{
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.3;
    public double FailureProbabilityOnLoad { get; set; } = 0.3;
    private static readonly Random RandomGenerator = new();

    private static SnackDbContextV2 CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SnackDbContextV2>();
        optionsBuilder.UseSqlite("Data Source=SnacksV2.db");
        return new SnackDbContextV2(optionsBuilder.Options);
    }

    public async Task<List<SnackV2>> GetAllSnacksAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllSnacksAsync");

        await using var context = CreateDbContext();
        return await context.Snacks.ToListAsync();
    }

    public async Task<SnackV2> AddSnackAsync(SnackV2 snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<SnackV2> UpdateSnackAsync(SnackV2 snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<SnackV2?> DeleteSnackAsync(int id)
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

    // --- locking
    public async Task<SnackV2> UpdateLockingAsync(SnackV2 snack)
    {
        await using var context = CreateDbContext();

        context.Snacks.Attach(snack);
        // context.Entry(snack).Property(s => s.Locked).IsModified = true;
        context.Entry(snack).Property(s => s.LockedBy).IsModified = true;

        await context.SaveChangesAsync();
        return snack;
    }
}