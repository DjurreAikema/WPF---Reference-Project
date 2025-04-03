using System.Reactive;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.Warehouses;

public partial class WarehousesView
{
    public WarehousesViewModel Vm { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    public WarehousesView()
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
    private void OnSelected(Warehouse obj) => Vm.SelectedChanged.OnNext(obj);

    private void OnAdd() => Vm.SelectedChanged.OnNext(new Warehouse());

    private void OnReload() => Vm.Reload.OnNext(Unit.Default);

    // --- Details
    private void OnSaved(Warehouse obj)
    {
        Console.WriteLine($"Saving: {obj.Name}, ID: {obj.Id}");

        if (obj.Id is 0 or null)
            Vm.Create.OnNext(obj);
        else
            Vm.Update.OnNext(obj);
    }

    private void OnDeleted(int id) => Vm.Delete.OnNext(id);
}