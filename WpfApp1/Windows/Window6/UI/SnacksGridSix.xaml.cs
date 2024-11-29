using System.Collections.ObjectModel;
using System.Windows;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window6.UI;

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
                c.Snacks = new ObservableCollection<Snack>(snacks);
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
    public event Action? AddSnack;
    public event Action? Reload;


    // --- Internal Properties
    private ObservableCollection<Snack>? _snacks;

    public ObservableCollection<Snack>? Snacks
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

    private void New_OnClick(object sender, RoutedEventArgs e)
    {
        AddSnack?.Invoke();
    }

    private void Reload_OnClick(object sender, RoutedEventArgs e)
    {
        Reload?.Invoke();
    }
}