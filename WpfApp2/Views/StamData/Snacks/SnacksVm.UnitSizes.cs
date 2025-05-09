using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.StamData.Snacks;

public partial class SnacksVm
{
    // --- Track for error recovery
    private UnitSize? _beforeOperationUnitSize;

    // --- Sources
    public readonly Subject<UnitSize> CreateUnitSize = new();
    public readonly Subject<UnitSize> UpdateUnitSize = new();
    public readonly Subject<int> DeleteUnitSize = new();

    // Create
    private IObservable<UnitSize?> CreatedUnitSizeObs => CreateUnitSize
        .Do(obj => _beforeOperationUnitSize = new UnitSize(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _unitSizeService.AddAsync(obj),
            _notifications, "Unit size added successfully.", e => $"Error creating unit size: {e.Message}"
        );

    // Update
    private IObservable<UnitSize?> UpdatedUnitSizeObs => UpdateUnitSize
        .Do(obj => _beforeOperationUnitSize = new UnitSize(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _unitSizeService.UpdateAsync(obj),
            _notifications, "Unit size updated successfully.", e => $"Error updating unit size: {e.Message}"
        );

    // Delete
    private IObservable<UnitSize?> DeletedUnitSizeObs => DeleteUnitSize
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _unitSizeService.DeleteAsync(id),
            _notifications, "Unit size deleted successfully.", e => $"Error deleting unit size: {e.Message}"
        );

    // --- Constructor
    private void UnitSizeVm()
    {
        // CreatedUnitSize reducer
        _disposables.Add(CreatedUnitSizeObs
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

                    currentSnack.UnitSizes ??= [];
                    currentSnack.UnitSizes.Add(obj);

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

        // UpdatedUnitSize reducer
        _disposables.Add(UpdatedUnitSizeObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                        return;
                    }

                    var currentSnack = _stateSubject.Value.Selected;
                    if (currentSnack?.UnitSizes == null) return;

                    currentSnack.UnitSizes = currentSnack.UnitSizes == null
                        ? []
                        : new ObservableCollection<UnitSize>(currentSnack.UnitSizes);

                    // Find and replace the updated UnitSize
                    var index = currentSnack.UnitSizes.ToList().FindIndex(u => u.Id == obj.Id);
                    if (index >= 0)
                        currentSnack.UnitSizes[index] = obj;

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
                    Console.WriteLine($"Unhandled error in UpdatedUnitSize reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                }
            ));

        // DeletedUnitSize reducer
        _disposables.Add(DeletedUnitSizeObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                        return;
                    }

                    var currentSnack = _stateSubject.Value.Selected;
                    if (currentSnack?.UnitSizes == null) return;

                    currentSnack.UnitSizes = new ObservableCollection<UnitSize>(currentSnack.UnitSizes.Where(u => u.Id != obj.Id));

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
                    Console.WriteLine($"Unhandled error in DeletedUnitSize reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with {InProgress = false});
                }
            ));
    }
}