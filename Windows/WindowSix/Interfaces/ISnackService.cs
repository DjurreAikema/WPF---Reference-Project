using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix.Interfaces;

public interface ISnackService
{
    Task<List<Snack>> GetAllSnacksAsync();
    Task AddSnackAsync(Snack snack);
    Task<Snack> UpdateSnackAsync(Snack snack);
    Task DeleteSnackAsync(int id);
}