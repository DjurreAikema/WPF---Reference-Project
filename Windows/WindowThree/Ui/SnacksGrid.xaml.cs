using System.Windows;
using System.Windows.Controls;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowThree.Ui;

public partial class SnacksGrid : UserControl
{
    public static readonly DependencyProperty SnacksProperty = DependencyProperty.Register(
        nameof(Snacks), typeof(IEnumerable<Snack>),
        typeof(SnacksGrid), new PropertyMetadata(null));

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