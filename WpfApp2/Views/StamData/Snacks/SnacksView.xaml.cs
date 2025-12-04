using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Shared.Debugging.Extensions;
using WpfApp2.Shared.ExtensionMethods;
using WpfApp2.Shared.Navigation.Interfaces;

namespace WpfApp2.Views.StamData.Snacks;

[ObservableObject]
public partial class SnacksView : INavigationAware
{
    public SnacksVm Vm { get; set; } = new SnacksVm().RegisterWithTracker();
    public Subject<bool> TriggerDispose { get; set; } = new();
    private CompositeDisposable _disposables = new();

    // --- Internal Properties
    private bool _shouldDispose;
    [ObservableProperty] private SnackFlags? _flags;

    // --- Navigation
    public void OnNavigatedTo()
    {
        if (!_shouldDispose) return;

        Vm = new SnacksVm().RegisterWithTracker();
        _disposables = new CompositeDisposable();
        _disposables.Add(Vm.FlagsObs.Subscribe(flags => { Flags = flags; }));
        _shouldDispose = false;
    }

    public void OnNavigatedFrom(bool isInStack)
    {
        _shouldDispose = !isInStack;
        if (!_shouldDispose) return;

        Vm.Dispose();
        _disposables.Dispose();
        TriggerDispose.OnNext(true);
    }

    // --- Constructor
    public SnacksView()
    {
        InitializeComponent();

        _disposables.Add(Vm.FlagsObs.Subscribe(flags => { Flags = flags; }));

        // Dispose of all subscriptions when the window is closed
        Unloaded += (_, _) =>
        {
            if (Application.Current.MainWindow == null || !_shouldDispose)
                return;

            Vm.Dispose();
            _disposables.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    // --- List
    private void OnSnackSelected(object obj)
    {
        var snack = obj.SafeCast<Snack>();
        if (snack != null) Vm.SelectedChanged.OnNext(snack);
    }

    private void OnAddSnack() => Vm.SelectedChanged.OnNext(new Snack());

    private void OnRefreshSnacks() => Vm.Reload.OnNext(Unit.Default);

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