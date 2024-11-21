using System.Reactive.Subjects;
using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFive;

public partial class WindowFive : Window
{
    public WindowFiveViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    // --- Constructor
    public WindowFive()
    {
        InitializeComponent();

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