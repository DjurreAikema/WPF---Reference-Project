using Microsoft.EntityFrameworkCore;
using WpfApp1.Classes;
using WpfApp1.Data;
using WpfApp1.Windows.WindowSix.Interfaces;

namespace WpfApp1.Windows.WindowSix.Services;

public class SnackService : ISnackService
{
    public SnackService()
    {
    }

    private SnackDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SnackDbContext>();
        optionsBuilder.UseSqlite("Data Source=snacks.db");
        return new SnackDbContext(optionsBuilder.Options);
    }

    public async Task<List<Snack>> GetAllSnacksAsync()
    {
        await using var context = CreateDbContext();
        return await context.Snacks.ToListAsync();
    }

    public async Task<Snack> AddSnackAsync(Snack snack)
    {
        await using var context = CreateDbContext();
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    // public async Task UpdateSnackAsync(Snack snack)
    // {
    //     await using var context = CreateDbContext();
    //     context.Snacks.Update(snack);
    //     await context.SaveChangesAsync();
    // }

    public async Task<Snack> UpdateSnackAsync(Snack snack)
    {
        await using var context = CreateDbContext();
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();
        return snack; // Return the updated snack
    }

    public async Task<Snack> DeleteSnackAsync(int id)
    {
        await using var context = CreateDbContext();
        var snack = await context.Snacks.FindAsync(id);
        if (snack == null) return snack;
        context.Snacks.Remove(snack);
        await context.SaveChangesAsync();
        return snack;
    }
}