using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFive.UI;

public partial class SnackDetailsFive
{
    private IDisposable? _selectedSnackSubscription;

    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<Snack>), typeof(SnackDetailsFive),
        new PropertyMetadata(null, (d, e) =>
        {
            if (d is not SnackDetailsFive c) return;
            c._selectedSnackSubscription?.Dispose();

            if (e.NewValue is IObservable<Snack> newObs)
                c._selectedSnackSubscription = newObs.Subscribe(snack =>
                {
                    c.SelectedSnack = snack;
                });
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
    public SnackDetailsFive()
    {
        InitializeComponent();
    }
}