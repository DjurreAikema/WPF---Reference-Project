using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Shared.Debugging.Extensions;
using WpfApp2.Shared.Navigation.Interfaces;

namespace WpfApp2.Views.StamData.Warehouses;

public partial class WarehousesView : INavigationAware
{
    public WarehousesViewModel Vm { get; set; } = new WarehousesViewModel().RegisterWithTracker();
    public Subject<bool> TriggerDispose { get; set; } = new();

    // --- Internal Properties
    private bool _shouldDispose;

    // --- Navigation
    public void OnNavigatedTo()
    {
        if (!_shouldDispose) return;

        Vm = new WarehousesViewModel().RegisterWithTracker();
        _shouldDispose = false;
    }

    public void OnNavigatedFrom(bool isInStack)
    {
        _shouldDispose = !isInStack;
        if (!_shouldDispose) return;

        Vm.Dispose();
        TriggerDispose.OnNext(true);
    }

    public WarehousesView()
    {
        InitializeComponent();

        // Dispose of all subscriptions when the window is closed
        Unloaded += (_, _) =>
        {
            if (Application.Current.MainWindow == null || !_shouldDispose)
                return;

            Vm.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    // --- List
    private void OnWarehouseSelected(object obj) => Vm.SelectedChanged.OnNext((Warehouse) obj);

    private void OnAddWarehouse() => Vm.SelectedChanged.OnNext(new Warehouse());

    private void OnRefreshWarehouses() => Vm.Reload.OnNext(Unit.Default);

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