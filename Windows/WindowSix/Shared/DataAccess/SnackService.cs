using Microsoft.EntityFrameworkCore;
using WpfApp1.Classes;
using WpfApp1.Data;
using WpfApp1.Windows.WindowSix.Shared.Interfaces;

namespace WpfApp1.Windows.WindowSix.Shared.DataAccess;

public class SnackService : ISnackService
{
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.5;
    private static readonly Random RandomGenerator = new();

    private static SnackDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SnackDbContext>();
        optionsBuilder.UseSqlite("Data Source=snacks.db");
        return new SnackDbContext(optionsBuilder.Options);
    }

    public async Task<List<Snack>> GetAllSnacksAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during GetAllSnacksAsync");

        await using var context = CreateDbContext();
        return await context.Snacks.ToListAsync();
    }

    public async Task<Snack> AddSnackAsync(Snack snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<Snack> UpdateSnackAsync(Snack snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    public async Task<Snack> DeleteSnackAsync(int id)
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