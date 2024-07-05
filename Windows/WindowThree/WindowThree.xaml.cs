using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowThree;

public partial class WindowThree : Window, INotifyPropertyChanged
{
    private WindowThreeViewModel ViewModel { get; } = new();

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

    public WindowThree()
    {
        InitializeComponent();
        DataContext = this;

        ViewModel.Snacks.Subscribe(snacks =>
        {
            Snacks = snacks;
            OnPropertyChanged(nameof(Snacks));
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}