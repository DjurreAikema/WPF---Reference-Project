using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour;

public partial class WindowFour : INotifyPropertyChanged
{
    private WindowFourViewModel ViewModel { get; } = new();
    private readonly CompositeDisposable _disposables = new();

    // --- Properties
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

    private bool _loading;

    public bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;
            OnPropertyChanged();
        }
    }

    // --- Constructor
    public WindowFour()
    {
        InitializeComponent();
        DataContext = this;

        _disposables.Add(ViewModel.Snacks.Subscribe(snacks =>
        {
            Snacks = snacks;
            OnPropertyChanged(nameof(Snacks));
        }));

        _disposables.Add(ViewModel.SelectedSnack.Subscribe(snack =>
        {
            SelectedSnack = snack;
            OnPropertyChanged(nameof(SelectedSnack));
        }));

        _disposables.Add(ViewModel.Loading.Subscribe(loading =>
        {
            Loading = loading;
            OnPropertyChanged(nameof(Loading));
        }));


        // Dispose of all subscriptions when the window is closed
        Closing += (_, _) =>
        {
            ViewModel.Dispose();
            _disposables.Dispose();
        };
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }

    // --- On Property Changed
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}