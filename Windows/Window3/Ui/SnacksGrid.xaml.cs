using System.Windows;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.WindowThree.Ui;

public partial class SnacksGrid
{
    public static readonly DependencyProperty SnacksProperty = DependencyProperty.Register(
        nameof(Snacks), typeof(IEnumerable<Snack>), typeof(SnacksGrid),
        new PropertyMetadata(null));

    public IEnumerable<Snack> Snacks
    {
        get => (IEnumerable<Snack>) GetValue(SnacksProperty);
        set => SetValue(SnacksProperty, value);
    }

    public event Action<Snack>? SnackSelected;

    public SnacksGrid()
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