using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.ExtensionMethods;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1.Windows.Window7._1;

public record WindowSevenOneStateV2
{
    public List<SnackV2> Snacks { get; init; } = [];
    public SnackV2? SelectedSnack { get; init; }
    public bool Loading { get; init; } = true;
}

public class WindowSevenOneViewModelV2 : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly SnackServiceV3 _snackService;

    // --- State
    private readonly BehaviorSubject<WindowSevenOneStateV2> _stateSubject = new(new WindowSevenOneStateV2());
    private IObservable<WindowSevenOneStateV2> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<SnackV2>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<SnackV2?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // Access to the lock service through the snack service
    public ILockService LockService => _snackService.LockService;

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Sources
    public readonly Subject<SnackV2> SelectedSnackChanged = new();
    public readonly Subject<SnackV2> Create = new();
    public readonly Subject<SnackV2> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // New lock-related subjects
    public readonly Subject<LockResult> LockStatusChanged = new();
    public readonly Subject<Unit> ReleaseAllLocks = new();
    public readonly Subject<Unit> CleanupExpiredLocks = new();

    // Load
    private IObservable<List<SnackV2>> SnacksLoadedObs =>
        Reload.StartWith(Unit.Default)
            .SelectMany(_ => Observable.FromAsync(_snackService.GetAllSnacksAsync)
                .NotifyOnSuccessAndError(_notifications,
                    "Snacks loaded successfully.",
                    e => $"Error loading snacks: {e.Message}",
                    new List<SnackV2>()));

    // Create
    private IObservable<SnackV2?> SnackCreatedObs => Create.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.AddSnackAsync(obj))
            .NotifyOnSuccessAndError(_notifications,
                "Snack added successfully.",
                e => $"Error creating snack: {e.Message}"));

    // Update
    private IObservable<SnackV2?> SnackUpdatedObs => Update.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.UpdateSnackAsync(obj))
            .NotifyOnSuccessAndError(_notifications,
                "Snack updated successfully.",
                e => $"Error updating snack: {e.Message}"));

    // Delete
    private IObservable<SnackV2?> SnackDeletedObs => Delete.SelectMany(id =>
        Observable.FromAsync(async () => await _snackService.DeleteSnackAsync(id))
            .NotifyOnSuccessAndError(_notifications,
                "Snack deleted successfully.",
                e => $"Error deleting snack: {e.Message}"));

    // Lock events
    private IObservable<LockResult> LockEventsObs => LockStatusChanged
        .Do(result =>
        {
            // Show notification about lock status
            _notifications.OnNext(new NotificationMessage(
                result.Message,
                result.Success
            ));

            // If a lock status changed, reload the data to refresh UI
            Reload.OnNext(Unit.Default);
        });

    // Release all locks
    private IObservable<Unit> ReleaseAllLocksObs => ReleaseAllLocks
        .SelectMany(_ => Observable.FromAsync(_snackService.ReleaseAllMyLocksAsync)
            .Select(_ => Unit.Default)
            .NotifyOnSuccessAndError(_notifications,
                "All locks released successfully.",
                e => $"Error releasing locks: {e.Message}",
                Unit.Default));

    // Cleanup expired locks
    private IObservable<Unit> CleanupExpiredLocksObs => CleanupExpiredLocks
        .SelectMany(_ => Observable.FromAsync(_snackService.CleanupExpiredLocksAsync)
            .Select(_ => Unit.Default)
            .NotifyOnSuccessAndError(_notifications,
                "Expired locks cleaned up successfully.",
                e => $"Error cleaning up locks: {e.Message}",
                Unit.Default));

    // --- Reducers
    public WindowSevenOneViewModelV2()
    {
        _snackService = new SnackServiceV3
        {
            SimulateFailures = true,
            FailureProbability = 0.3,
            FailureProbabilityOnLoad = 0.3
        };

        // Subscribe to lock events from the lock service itself
        _disposables.Add(_snackService.LockService.LockEvents
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(result => { LockStatusChanged.OnNext(result); }));

        // SelectedSnackChanged reducer
        _disposables.Add(SelectedSnackChanged
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snack => { _stateSubject.OnNext(_stateSubject.Value with {SelectedSnack = snack}); }));

        // SnacksLoaded reducer
        _disposables.Add(SnacksLoadedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snacks =>
            {
                _stateSubject.OnNext(_stateSubject.Value with
                {
                    Snacks = snacks,
                    Loading = false
                });
            }));

        // SnackCreated reducer
        _disposables.Add(SnackCreatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snack =>
            {
                if (snack is null) return;
                var snacks = _stateSubject.Value.Snacks;
                snacks.Add(snack);

                _stateSubject.OnNext(_stateSubject.Value with
                {
                    Snacks = snacks,
                    SelectedSnack = snack
                });
            }));

        // SnackUpdated reducer
        _disposables.Add(SnackUpdatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(updatedSnack =>
            {
                if (updatedSnack is null) return;
                var snacks = _stateSubject.Value.Snacks;
                var index = snacks.FindIndex(s => s.Id == updatedSnack.Id);
                if (index < 0) return;
                snacks[index] = updatedSnack;

                _stateSubject.OnNext(_stateSubject.Value with
                {
                    Snacks = snacks,
                    SelectedSnack = updatedSnack
                });
            }));

        // SnackDeleted reducer
        _disposables.Add(SnackDeletedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snack =>
            {
                if (snack is null) return;
                var snacks = _stateSubject.Value.Snacks;
                var index = snacks.FindIndex(s => s.Id == snack.Id);
                snacks.RemoveAt(index);

                _stateSubject.OnNext(_stateSubject.Value with
                {
                    Snacks = snacks,
                    SelectedSnack = null
                });
            }));

        // ReleaseAllLocks reducer
        _disposables.Add(ReleaseAllLocksObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(_ =>
            {
                // After releasing all locks, reload the data
                Reload.OnNext(Unit.Default);
            }));

        // CleanupExpiredLocks reducer
        _disposables.Add(CleanupExpiredLocksObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(_ =>
            {
                // After cleaning up locks, reload the data
                Reload.OnNext(Unit.Default);
            }));

        // Lock events reducer
        _disposables.Add(LockEventsObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(_ => { })); // We handle the actions in the Do() operator
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
        _snackService.Dispose();
    }
}