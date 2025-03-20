using System.Reactive;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window8;

public partial class WindowEight
{
    public WindowEightViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WindowEight()
    {
        InitializeComponent();

        // Dispose of all subscriptions when the window is closed
        Closing += (_, _) =>
        {
            ViewModel.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    // --- Grid
    private void Grid_SnackSelected(SnackV2 snack) => ViewModel.SelectedSnackChanged.OnNext(snack);

    private void Grid_OnAddSnack() => ViewModel.SelectedSnackChanged.OnNext(new SnackV2());

    private void Grid_OnReload() => ViewModel.Reload.OnNext(Unit.Default);

    // --- Details
    private void Details_OnSnackSaved(SnackV2 snack)
    {
        if (snack.Id is 0 or null)
            ViewModel.Create.OnNext(snack);
        else
            ViewModel.Update.OnNext(snack);
    }

    private void Details_OnSnackDeleted(int snackId) => ViewModel.Delete.OnNext(snackId);
}