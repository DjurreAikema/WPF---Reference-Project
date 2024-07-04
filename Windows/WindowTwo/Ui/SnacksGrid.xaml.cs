using System.Windows;
using System.Windows.Controls;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowTwo.Ui;

public partial class SnacksGrid : UserControl
{
    public static readonly DependencyProperty SnacksProperty =
        DependencyProperty.Register("Snacks", typeof(IEnumerable<Snack>), typeof(SnacksGrid), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedSnackProperty =
        DependencyProperty.Register("SelectedSnack", typeof(Snack), typeof(SnacksGrid), new PropertyMetadata(null));

    public IEnumerable<Snack> Snacks
    {
        get => (IEnumerable<Snack>) GetValue(SnacksProperty);
        set => SetValue(SnacksProperty, value);
    }

    public Snack SelectedSnack
    {
        get => (Snack) GetValue(SelectedSnackProperty);
        set => SetValue(SelectedSnackProperty, value);
    }

    public SnacksGrid()
    {
        InitializeComponent();
        Loaded += (s, e) => { SnacksDataGrid.ItemsSource = Snacks; };
    }
}