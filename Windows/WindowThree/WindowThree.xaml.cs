using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowThree;

public partial class WindowThree : Window
{
    private WindowThreeViewModel ViewModel { get; } = new();

    private IEnumerable<Snack>? _snacks;

    public IEnumerable<Snack>? Snacks
    {
        get => _snacks;
        set
        {
            _snacks = value;
            OnPropertyChanged(default);
        }
    }

    private Snack? _selectedSnack;

    public Snack? SelectedSnack
    {
        get => _selectedSnack;
        set
        {
            _selectedSnack = value;
            OnPropertyChanged(default);
        }
    }

    public WindowThree()
    {
        InitializeComponent();
        DataContext = this;

        ViewModel.Snacks.Subscribe(snacks => { Snacks = snacks; });
    }
}