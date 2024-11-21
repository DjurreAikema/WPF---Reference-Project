using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix;

public partial class WindowSix : Window
{
    public WindowSixViewModel ViewModel { get; } = new();

    public WindowSix()
    {
        InitializeComponent();

        Closing += (_, _) => { ViewModel.Dispose(); };
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }
}