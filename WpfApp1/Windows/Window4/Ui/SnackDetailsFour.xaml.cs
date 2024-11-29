using System.Windows;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window4.Ui;

public partial class SnackDetailsFour
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<Snack>), typeof(SnackDetailsFour),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsFour c) return;
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
    public SnackDetailsFour()
    {
        InitializeComponent();
    }
}