using System.Windows;
using WpfApp1.Abstract;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour.Ui;

public partial class SnacksGridFour : ADumbComponent
{
    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksGridFour),
        new PropertyMetadata(null, (d, e) =>
        {
            if (d is not SnacksGridFour component) return;
            component._disposables.Add(component.SnacksObs.Subscribe(snacks =>
            {
                component.Snacks = snacks;
                component.OnPropertyChanged(nameof(Snacks));
            }));
        }));

    public IObservable<IEnumerable<Snack>> SnacksObs
    {
        get => (IObservable<IEnumerable<Snack>>) GetValue(SnacksObsProperty);
        set => SetValue(SnacksObsProperty, value);
    }

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

    public event Action<Snack>? SnackSelected;

    public SnacksGridFour()
    {
        InitializeComponent();

        SnacksDataGrid.SelectionChanged += (_, _) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack)
            {
                SnackSelected?.Invoke(selectedSnack);
            }
        };
    }
}