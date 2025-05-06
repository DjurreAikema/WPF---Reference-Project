using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;
using WpfApp2.Data.DataAccess;
using WpfApp2.Shared.Abstract;
using WpfApp2.Shared.Debugging.Extensions;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.StamData.Warehouses;

public record WarehousesState : BaseState<WarehousesState>
{
    public List<Warehouse> Warehouses { get; init; } = [];
    public Warehouse? Selected { get; init; }

    public override WarehousesState WithInProgress(bool inProgress) =>
        this with {InProgress = inProgress};
}

public class WarehousesViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly WarehouseService _warehouseService;

    // --- State
    private readonly BehaviorSubject<WarehousesState> _stateSubject = new(new WarehousesState());
    private IObservable<WarehousesState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<Warehouse>> WarehousesObs => StateObs.Select(state => state.Warehouses);
    public IObservable<Warehouse?> SelectedObs => StateObs.Select(state => state.Selected);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Track for error recovery
    private Warehouse? _beforeOperation;

    // --- Sources
    public readonly Subject<Warehouse> SelectedChanged = new();
    public readonly Subject<Warehouse> Create = new();
    public readonly Subject<Warehouse> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // Load
    private IObservable<Warehouse?> SelectedChangedObs => SelectedChanged
        .Do(obj => _beforeOperation = new Warehouse(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _warehouseService.FillAsync(obj),
            _notifications, "Warehouse selected successfully.", e => $"Error selecting warehouse: {e.Message}"
        );

    private IObservable<List<Warehouse>?> LoadedObs => Reload
        .StartWith(Unit.Default)
        .ExecuteAsyncOperation(
            _stateSubject,
            _ => _warehouseService.GetAllAsync(),
            _notifications, "Warehouses loaded successfully.", e => $"Error loading Warehouses: {e.Message}", []
        );

    // Create
    private IObservable<Warehouse?> CreatedObs => Create
        .Do(obj => _beforeOperation = new Warehouse(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _warehouseService.AddAsync(obj),
            _notifications, "Warehouse added successfully.", e => $"Error creating warehouse: {e.Message}"
        );

    // Update
    private IObservable<Warehouse?> UpdatedObs => Update
        .Do(obj => _beforeOperation = new Warehouse(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _warehouseService.UpdateAsync(obj),
            _notifications, "Warehouse updated successfully.", e => $"Error updating warehouse: {e.Message}"
        );

    // Delete
    private IObservable<Warehouse?> DeletedObs => Delete
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _warehouseService.DeleteAsync(id),
            _notifications, "Warehouse deleted successfully.", e => $"Error deleting warehouse: {e.Message}"
        );

    // --- Reducers
    public WarehousesViewModel()
    {
        _warehouseService = new WarehouseService
        {
            SimulateFailures = false,
            FailureProbability = 0.1,
            FailureProbabilityOnLoad = 0.1
        };

        // SelectedChanged reducer
        _disposables.Add(SelectedChangedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(
                obj => { _stateSubject.OnNext(_stateSubject.Value with {Selected = obj}); },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SelectedChanged reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Selected = _beforeOperation,
                        InProgress = false
                    });
                }));

        // Loaded reducer
        _disposables.Add(LoadedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(objs =>
                {
                    if (objs is null) return;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Warehouses = [..objs],
                        Loading = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Loaded reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Loading = false
                    });
                }));

        // Created reducer
        _disposables.Add(CreatedObs
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

                    var objs = new List<Warehouse>(_stateSubject.Value.Warehouses) {obj};

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Warehouses = objs,
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

        // Updated reducer
        _disposables.Add(UpdatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(updated =>
                {
                    if (updated is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            Selected = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var objs = new List<Warehouse>(_stateSubject.Value.Warehouses);
                    var index = objs.FindIndex(s => s.Id == updated.Id);
                    if (index < 0) return;
                    objs[index] = updated;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Warehouses = objs,
                        Selected = updated
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Updated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Selected = _beforeOperation,
                        InProgress = false
                    });
                }
            ));

        // Deleted reducer
        _disposables.Add(DeletedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null) return;
                    var objs = new List<Warehouse>(_stateSubject.Value.Warehouses);
                    var index = objs.FindIndex(s => s.Id == obj.Id);
                    objs.RemoveAt(index);

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Warehouses = objs,
                        Selected = null
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Deleted reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        InProgress = false
                    });
                }
            ));
    }

    // --- Dispose
    public void Dispose()
    {
        this.UnregisterFromTracker();
        _disposables.Dispose();
    }
}