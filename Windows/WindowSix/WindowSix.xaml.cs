using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Interfaces;

namespace WpfApp1.Windows.WindowSix;

public partial class WindowSix : Window
{
    public WindowSixViewModel ViewModel { get; }

    public WindowSix()
    {
        InitializeComponent();

        var app = (App)Application.Current;
        var snackService = app.ServiceProvider.GetRequiredService<ISnackService>();
        ViewModel = new WindowSixViewModel(snackService);

        Closing += (_, _) => { ViewModel.Dispose(); };
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }
}