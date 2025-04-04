using System.Reactive;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.Snacks;

public partial class SnacksView
{
    public SnacksViewModel Vm { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public SnacksView()
    {
        InitializeComponent();

        // Dispose of all subscriptions when the window is closed
        Unloaded += (_, _) =>
        {
            Vm.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    // --- List
    private void OnSelected(Snack obj) => Vm.SelectedChanged.OnNext(obj);

    private void OnAdd() => Vm.SelectedChanged.OnNext(new Snack());

    private void OnReload() => Vm.Reload.OnNext(Unit.Default);

    // --- Details
    private void OnSaved(Snack obj)
    {
        Console.WriteLine($"Saving: {obj.Name}, ID: {obj.Id}");

        if (obj.Id is 0 or null)
            Vm.Create.OnNext(obj);
        else
            Vm.Update.OnNext(obj);
    }

    private void OnDeleted(int id) => Vm.Delete.OnNext(id);
}