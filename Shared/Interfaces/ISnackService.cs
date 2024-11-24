using WpfApp1.Shared.Classes;

namespace WpfApp1.Shared.Interfaces;

public interface ISnackService
{
    Task<List<Snack>> GetAllSnacksAsync();
    Task<Snack> AddSnackAsync(Snack snack);
    Task<Snack> UpdateSnackAsync(Snack snack);
    Task<Snack> DeleteSnackAsync(int id);
}