using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour.Ui;

public partial class SnackDetailsFour
{
    public static readonly DependencyProperty SelectedSnackProperty = DependencyProperty.Register(
        nameof(SelectedSnack), typeof(Snack), typeof(SnackDetailsFour),
        new PropertyMetadata(null));

    public Snack SelectedSnack
    {
        get => (Snack) GetValue(SelectedSnackProperty);
        set => SetValue(SelectedSnackProperty, value);
    }

    public SnackDetailsFour()
    {
        InitializeComponent();
    }
}