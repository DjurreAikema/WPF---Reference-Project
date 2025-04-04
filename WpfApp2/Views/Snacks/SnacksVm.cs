using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes;
using WpfApp2.Data.DataAccess;
using WpfApp2.Shared.Abstract;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.Snacks;

public record SnacksState : BaseState<SnacksState>
{
    public List<Snack> Snacks { get; init; } = [];
    public Snack? Selected { get; init; }

    public override SnacksState WithInProgress(bool inProgress) =>
        this with {InProgress = inProgress};
}

public class SnackFlags
{
    public bool HasId { get; set; }
    public bool HasMultipleUnitSizes { get; set; }
}

public partial class SnacksVm : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly UnitSizeService _unitSizeService;
    private readonly SnackService _snackService;


    // --- State
    private readonly BehaviorSubject<SnacksState> _stateSubject = new(new SnacksState());
    private IObservable<SnacksState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<Snack>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<Snack?> SelectedObs => StateObs.Select(state => state.Selected);
    public IObservable<List<UnitSize>?> SelectedUnitSizesObs => StateObs.Select(state => state.Selected?.UnitSize?.ToList() ?? []);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    public IObservable<SnackFlags> FlagsObs => StateObs.Select(state => new SnackFlags
    {
        HasId = state.Selected?.Id != null,
        HasMultipleUnitSizes = state.Selected?.MultipleUnitSizes ?? false,
    });

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Track for error recovery
    private Snack? _beforeOperation;

    // --- Sources
    public readonly Subject<Snack> SelectedChanged = new();
    public readonly Subject<Snack> Create = new();
    public readonly Subject<Snack> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // Load
    private IObservable<Snack?> SelectedChangedObs => SelectedChanged
        .Do(obj => _beforeOperation = new Snack(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.FillAsync(obj),
            _notifications, "Snack selected successfully.", e => $"Error selecting snack: {e.Message}"
        );

    private IObservable<List<Snack>?> LoadedObs => Reload
        .StartWith(Unit.Default)
        .ExecuteAsyncOperation(
            _stateSubject,
            _ => _snackService.GetAllAsync(),
            _notifications, "Snacks loaded successfully.", e => $"Error loading snacks: {e.Message}", []
        );

    // Create
    private IObservable<Snack?> CreatedObs => Create
        .Do(obj => _beforeOperation = new Snack(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.AddAsync(obj),
            _notifications, "Snack added successfully.", e => $"Error creating snack: {e.Message}"
        );

    // Update
    private IObservable<Snack?> UpdatedObs => Update
        .Do(obj => _beforeOperation = new Snack(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.UpdateAsync(obj),
            _notifications, "Snack updated successfully.", e => $"Error updating snack: {e.Message}"
        );

    // Delete
    private IObservable<Snack?> DeletedObs => Delete
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _snackService.DeleteAsync(id),
            _notifications, "Snack deleted successfully.", e => $"Error deleting snack: {e.Message}"
        );

    // --- Reducers
    public SnacksVm()
    {
        _snackService = new SnackService
        {
            SimulateFailures = true,
            FailureProbability = 0.1,
            FailureProbabilityOnLoad = 0.1
        };

        _unitSizeService = new UnitSizeService
        {
            SimulateFailures = true,
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
                        Snacks = [..objs],
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

                    var objs = new List<Snack>(_stateSubject.Value.Snacks) {obj};

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

                    var objs = new List<Snack>(_stateSubject.Value.Snacks);
                    var index = objs.FindIndex(s => s.Id == updated.Id);
                    if (index < 0) return;
                    objs[index] = updated;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
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
                    var objs = new List<Snack>(_stateSubject.Value.Snacks);
                    var index = objs.FindIndex(s => s.Id == obj.Id);
                    objs.RemoveAt(index);

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = objs,
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

        // --- UnitSize
        UnitSizeVm();
    }

    // --- Dispose
    public void Dispose() => _disposables.Dispose();
}