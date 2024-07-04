using System.Windows;
using System.Windows.Controls;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowTwo.Ui;

public partial class SnackDetails : UserControl
{
    public static readonly DependencyProperty SelectedSnackProperty =
        DependencyProperty.Register("SelectedSnack", typeof(Snack), typeof(SnackDetails), new PropertyMetadata(null));

    public Snack SelectedSnack
    {
        get => (Snack)GetValue(SelectedSnackProperty);
        set => SetValue(SelectedSnackProperty, value);
    }

    public SnackDetails()
    {
        InitializeComponent();
    }
}