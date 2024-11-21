using System.Reactive.Subjects;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix;

public partial class WindowSix
{
    public WindowSixViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WindowSix()
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

    private void SnacksGridSix_OnAddSnack()
    {
        ViewModel.SelectedSnackChanged.OnNext(new Snack());
    }

    private void SnackDetailsSix_OnSnackSaved(Snack snack)
    {
        ViewModel.Update.OnNext(snack);
    }

    private void SnackDetailsSix_OnSnackDeleted(int snackId)
    {
        ViewModel.Delete.OnNext(snackId);
    }
}