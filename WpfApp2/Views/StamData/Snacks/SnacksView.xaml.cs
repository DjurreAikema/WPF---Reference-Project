using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Snacks;

[ObservableObject]
public partial class SnacksView
{
    public SnacksVm Vm { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();
    private readonly CompositeDisposable _disposables = new();

    // --- Internal Properties
    [ObservableProperty] private SnackFlags? _flags;

    // --- Constructor
    public SnacksView()
    {
        InitializeComponent();

        _disposables.Add(Vm.FlagsObs.Subscribe(flags => { Flags = flags; }));

        // Dispose of all subscriptions when the window is closed
        Unloaded += (_, _) =>
        {
            Vm.Dispose();
            _disposables.Dispose();
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

    // --- Unit Size
    private void OnUnitSizeSaved(UnitSize obj)
    {
        if (obj.Id is 0 or null)
            Vm.CreateUnitSize.OnNext(obj);
        else
            Vm.UpdateUnitSize.OnNext(obj);
    }

    private void OnUnitSizeDeleted(int id) => Vm.DeleteUnitSize.OnNext(id);

    // --- Inventory
    private void OnInventorySaved(Inventory obj)
    {
        if (obj.Id is 0 or null)
            Vm.CreateInventory.OnNext(obj);
        else
            Vm.UpdateInventory.OnNext(obj);
    }

    private void OnInventoryDeleted(int id) => Vm.DeleteInventory.OnNext(id);
}