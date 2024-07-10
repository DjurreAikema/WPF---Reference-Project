using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour.Ui;

public partial class SnacksGridFour
{
    public static readonly DependencyProperty SnacksProperty = DependencyProperty.Register(
        nameof(Snacks), typeof(IEnumerable<Snack>), typeof(SnacksGridFour),
        new PropertyMetadata(null));

    public IEnumerable<Snack> Snacks
    {
        get => (IEnumerable<Snack>) GetValue(SnacksProperty);
        set => SetValue(SnacksProperty, value);
    }

    public event Action<Snack>? SnackSelected;

    public SnacksGridFour()
    {
        InitializeComponent();

        SnacksDataGrid.SelectionChanged += (s, e) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack)
            {
                SnackSelected?.Invoke(selectedSnack);
            }
        };
    }
}