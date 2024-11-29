using System.Windows;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window3.Ui;

public partial class SnackDetails
{
    public static readonly DependencyProperty SelectedSnackProperty = DependencyProperty.Register(
        nameof(SelectedSnack), typeof(Snack), typeof(SnackDetails),
        new PropertyMetadata(null));

    public Snack SelectedSnack
    {
        get => (Snack) GetValue(SelectedSnackProperty);
        set => SetValue(SelectedSnackProperty, value);
    }

    public SnackDetails()
    {
        InitializeComponent();
    }
}