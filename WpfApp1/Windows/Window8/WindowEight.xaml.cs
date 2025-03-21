using System.Reactive;
using System.Reactive.Linq;
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

    // --- Form
    private void Form_Submitted() => ViewModel.SaveForm.OnNext(Unit.Default);

    private void Form_DeleteRequested()
    {
        var currentSnack = ViewModel.SelectedSnackObs.FirstAsync().Wait();
        if (currentSnack is {Id: > 0})
        {
            ViewModel.Delete.OnNext(currentSnack.Id.Value);
        }
    }

    private void Form_Cancelled()
    {
        // Revert form changes by refreshing from the current snack
        ViewModel.SelectedSnackChanged.OnNext(ViewModel.SelectedSnackObs.FirstAsync().Wait() ?? new SnackV2());
    }
}