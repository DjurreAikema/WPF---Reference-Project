using System.Windows;
using WpfApp1.Shared.Classes;
using WpfApp1.Windows.Window5.Events;

namespace WpfApp1.Windows.Window5.UI;

public partial class SnacksGridFive
{
    // --- Dependency Properties
    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksGridFive),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnacksGridFive c) return;
            c.Disposables.Add(c.SnacksObs.Subscribe(snacks =>
            {
                c.Snacks = snacks;
                c.OnPropertyChanged(nameof(Snacks));
            }));
        }));

    public IObservable<IEnumerable<Snack>> SnacksObs
    {
        get => (IObservable<IEnumerable<Snack>>) GetValue(SnacksObsProperty);
        set => SetValue(SnacksObsProperty, value);
    }

    // --- Events
    public event EventHandler<SnackSelectedEventArgs>? SnackSelected;

    // --- Internal Properties
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

    // --- Constructor
    public SnacksGridFive()
    {
        InitializeComponent();

        SnacksDataGrid.SelectionChanged += (_, _) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack) SnackSelected?.Invoke(this, new SnackSelectedEventArgs(selectedSnack));
        };
    }
}