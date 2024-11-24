using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.WindowTwo;

public class WindowTwoViewModel : INotifyPropertyChanged
{
    private IEnumerable<Snack>? _snacks;

    public IEnumerable<Snack>? Snacks
    {
        get => _snacks;
        set
        {
            _snacks = value;
            OnPropertyChanged();
        }
    }

    private Snack? _selectedSnack;

    public Snack? SelectedSnack
    {
        get => _selectedSnack;
        set
        {
            _selectedSnack = value;
            OnPropertyChanged();
        }
    }

    public WindowTwoViewModel()
    {
        Snacks = new List<Snack>
        {
            new() {Name = "Chips", Price = 1.50, Quantity = 10},
            new() {Name = "Chocolate", Price = 2.50, Quantity = 5},
            new() {Name = "Candy", Price = 0.50, Quantity = 20},
            new() {Name = "Gum", Price = 0.25, Quantity = 50},
            new() {Name = "Soda", Price = 1.00, Quantity = 15},
            new() {Name = "Water", Price = 0.75, Quantity = 25},
            new() {Name = "Juice", Price = 1.25, Quantity = 10},
            new() {Name = "Cookies", Price = 1.75, Quantity = 5}
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}