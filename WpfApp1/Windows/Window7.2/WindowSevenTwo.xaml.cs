using System.Reactive;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7._2;

public partial class WindowSevenTwo
{
    public WindowSevenTwoViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WindowSevenTwo()
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
        Console.WriteLine($"Saving snack: {snack.Name}, ID: {snack.Id}");

        if (snack.Id is 0 or null)
            ViewModel.Create.OnNext(snack);
        else
            ViewModel.Update.OnNext(snack);
    }

    private void Details_OnSnackDeleted(int snackId) => ViewModel.Delete.OnNext(snackId);
}