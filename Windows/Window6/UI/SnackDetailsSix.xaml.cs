using System.Windows;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window6.UI;

public partial class SnackDetailsSix
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<Snack>), typeof(SnackDetailsSix),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsSix c) return;
            c.Disposables.Add(c.SelectedSnackObs.Subscribe(snack =>
            {
                c.SelectedSnack = snack;
                c.OnPropertyChanged(nameof(SelectedSnack));
            }));
        }));

    public IObservable<Snack> SelectedSnackObs
    {
        get => (IObservable<Snack>) GetValue(SelectedSnackObsProperty);
        set => SetValue(SelectedSnackObsProperty, value);
    }

    // --- Events
    public event Action<Snack>? SnackSaved;
    public event Action<int>? SnackDeleted;

    // --- Internal Properties
    private Snack _selectedSnack = new();

    public Snack SelectedSnack
    {
        get => _selectedSnack;
        set
        {
            _selectedSnack = value;
            OnPropertyChanged();
        }
    }

    // --- Constructor
    public SnackDetailsSix()
    {
        InitializeComponent();
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        SnackSaved?.Invoke(SelectedSnack);
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectedSnack.Id is 0 or null) return;
        SnackDeleted?.Invoke(SelectedSnack.Id.Value);
    }
}