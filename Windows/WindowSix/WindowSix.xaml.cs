using System.Reactive.Subjects;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Interfaces;

namespace WpfApp1.Windows.WindowSix;

public partial class WindowSix
{
    public WindowSixViewModel ViewModel { get; }
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WindowSix()
    {
        InitializeComponent();

        var app = (App)Application.Current;
        var snackService = app.ServiceProvider.GetRequiredService<ISnackService>();
        ViewModel = new WindowSixViewModel(snackService);

        // Dispose of all subscriptions when the window is closed
        Closing += (_, _) =>
        {
            ViewModel.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }
}