using Microsoft.EntityFrameworkCore;
using WpfApp1.Classes;
using WpfApp1.Data;
using WpfApp1.Windows.WindowSix.Interfaces;

namespace WpfApp1.Windows.WindowSix.Services;

public class SnackService(SnackDbContext context) : ISnackService
{
    public async Task<IEnumerable<Snack>> GetAllSnacksAsync()
    {
        return await context.Snacks.ToListAsync();
    }

    public async Task AddSnackAsync(Snack snack)
    {
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSnackAsync(Snack snack)
    {
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSnackAsync(int id)
    {
        var snack = await context.Snacks.FindAsync(id);
        if (snack != null)
        {
            context.Snacks.Remove(snack);
            await context.SaveChangesAsync();
        }
    }
}