using System.Reactive.Subjects;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour;

public partial class WindowFour
{
    public WindowFourViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    // --- Constructor
    public WindowFour()
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