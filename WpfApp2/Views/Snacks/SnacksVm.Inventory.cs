using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.Snacks;

public partial class SnacksVm
{
    // --- Track for error recovery
    private Inventory? _beforeOperationInventory;

    // --- Sources
    public readonly Subject<Inventory> CreateInventory = new();
    public readonly Subject<Inventory> UpdateInventory = new();
    public readonly Subject<int> DeleteInventory = new();

    // Create
    private IObservable<Inventory?> CreatedInventoryObs => CreateInventory
        .Do(obj => _beforeOperationInventory = new Inventory(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _inventoryService.AddAsync(obj),
            _notifications, "Inventory added successfully.", e => $"Error creating inventory: {e.Message}"
        );

    // Update
    private IObservable<Inventory?> UpdatedInventoryObs => UpdateInventory
        .Do(obj => _beforeOperationInventory = new Inventory(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _inventoryService.UpdateAsync(obj),
            _notifications, "Inventory updated successfully.", e => $"Error updating inventory: {e.Message}"
        );

    // Delete inventory
    private IObservable<Inventory?> DeletedInventoryObs => DeleteInventory
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _inventoryService.DeleteAsync(id),
            _notifications, "Inventory deleted successfully.", e => $"Error deleting inventory: {e.Message}"
        );

    // --- Constructor
    private void InventoryVm()
    {
        // CreatedInventory reducer
        _disposables.Add(CreatedInventoryObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                        return;
                    }

                    var currentSnack = _stateSubject.Value.Selected;
                    if (currentSnack == null) return;

                    currentSnack.Inventories ??= [];
                    currentSnack.Inventories.Add(obj);
                    UpdateTotalQuantity(currentSnack);

                    // Update the snack in the overall list
                    var objs = new List<Snack>(_stateSubject.Value.Snacks);
                    var index = objs.FindIndex(s => s.Id == currentSnack.Id);
                    if (index >= 0)
                        objs[index] = currentSnack;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
                        Selected = currentSnack,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in CreatedUnitSize reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with {Selected = _beforeOperation, InProgress = false});
                }
            ));

        // UpdatedInventory reducer
        _disposables.Add(UpdatedInventoryObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                        return;
                    }

                    var currentSnack = _stateSubject.Value.Selected;
                    if (currentSnack?.Inventories == null) return;

                    // Find and replace the updated inventory
                    var index = currentSnack.Inventories.ToList().FindIndex(i => i.Id == obj.Id);
                    if (index >= 0)
                        currentSnack.Inventories[index] = obj;

                    UpdateTotalQuantity(currentSnack);

                    // Update the snack in the overall list
                    var objs = new List<Snack>(_stateSubject.Value.Snacks);
                    var snackIndex = objs.FindIndex(s => s.Id == currentSnack.Id);
                    if (snackIndex >= 0)
                        objs[snackIndex] = currentSnack;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
                        Selected = currentSnack,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in UpdatedInventory reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                }
            ));

        // DeletedInventory reducer
        _disposables.Add(DeletedInventoryObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                        return;
                    }

                    var currentSnack = _stateSubject.Value.Selected;
                    if (currentSnack?.Inventories == null) return;

                    currentSnack.Inventories = new ObservableCollection<Inventory>(currentSnack.Inventories.Where(i => i.Id != obj.Id));
                    UpdateTotalQuantity(currentSnack);

                    // Update the snack in the overall list
                    var objs = new List<Snack>(_stateSubject.Value.Snacks);
                    var snackIndex = objs.FindIndex(s => s.Id == currentSnack.Id);
                    if (snackIndex >= 0)
                        objs[snackIndex] = currentSnack;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
                        Selected = currentSnack,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in DeletedInventory reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                }
            ));
    }

    private static void UpdateTotalQuantity(Snack snack) => snack.Quantity = snack.Inventories?.Sum(i => i.Quantity) ?? 0;
}