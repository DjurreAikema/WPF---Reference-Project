using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.Snacks;

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

    private void UnitSizeVm()
    {
        // CreatedUnitSize reducer
        _disposables.Add(CreatedUnitSizeObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            Selected = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var objs = new List<UnitSize>(_stateSubject.Value.Selected.UnitSize) {obj};

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
                        Selected = obj,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Created reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Selected = _beforeOperation,
                        InProgress = false
                    });
                }
            ));
    }
}