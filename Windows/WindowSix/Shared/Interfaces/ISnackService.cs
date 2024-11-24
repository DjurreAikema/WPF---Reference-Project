using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix.Shared.Interfaces;

public interface ISnackService
{
    Task<List<Snack>> GetAllSnacksAsync();
    Task<Snack> AddSnackAsync(Snack snack);
    Task<Snack> UpdateSnackAsync(Snack snack);
    Task<Snack> DeleteSnackAsync(int id);
}