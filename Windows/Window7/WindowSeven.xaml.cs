using System.Reactive;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7;

public partial class WindowSeven
{
    public WindowSevenViewModel ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WindowSeven()
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
        if (snack.Id is 0 or null)
            ViewModel.Create.OnNext(snack);
        else
            ViewModel.Update.OnNext(snack);
    }

    private void SnackDetailsSix_OnSnackDeleted(int snackId)
    {
        ViewModel.Delete.OnNext(snackId);
    }

    private void SnacksGridSix_OnReload()
    {
        ViewModel.Reload.OnNext(Unit.Default);
    }
}