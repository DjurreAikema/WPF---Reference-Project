using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix.UI;

public partial class SnacksGridSix
{
    // --- Dependency Properties
    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksGridSix),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnacksGridSix c) return;
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
    public event Action<Snack>? SnackSelected;


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
    public SnacksGridSix()
    {
        InitializeComponent();

        SnacksDataGrid.SelectionChanged += (_, _) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack) SnackSelected?.Invoke(selectedSnack);
        };
    }
}