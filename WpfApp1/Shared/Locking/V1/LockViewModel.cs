using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.ExtensionMethods;

namespace WpfApp1.Shared.Locking.V1;

public record LockViewModelState<TItem> where TItem : class, ILockable
{
    public TItem? SelectedItem { get; init; }
    public bool Loading { get; init; } = true;

    // Possibly track “current user name” if needed
}

public class LockViewModel<TItem> : IDisposable where TItem : class, ILockable
{
    private readonly CompositeDisposable _disposables = new();

    // --- State
    private readonly BehaviorSubject<LockViewModelState<TItem>> _stateSubject;
    private IObservable<LockViewModelState<TItem>> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<TItem?> SelectedItemObs => StateObs.Select(state => state.SelectedItem);
    public IObservable<bool> IsLockedObs => StateObs.Select(state => state.SelectedItem?.LockState == LockState.Locked);
    public IObservable<bool> IsSoftLockedObs => StateObs.Select(state => state.SelectedItem?.LockState == LockState.SoftLocked);
    public IObservable<bool> IsUnlockedObs => StateObs.Select(state => state.SelectedItem?.LockState == LockState.Unlocked);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Sources
    public Subject<TItem?> SelectItem { get; } = new(); // user picks a new item
    public Subject<TItem> ClaimLock { get; } = new(); // user tries to claim lock (unlock for themselves)
    public Subject<TItem> ReleaseLock { get; } = new(); // user leaves the item or closes window

    // Claim
    private IObservable<TItem?> ClaimLockObs => ClaimLock.SelectMany(obj =>
    {
        obj.Locked = DateTime.Now;
        obj.LockedBy = Environment.UserName;
        obj.LockState = LockState.Unlocked;

        return Observable.FromAsync(() => _updateLockFunc(obj))
            .NotifyOnError(_notifications, e => $"Error claiming lock: {e.Message}");
    });

    // Release
    private IObservable<TItem?> ReleaseLockObs => ReleaseLock.SelectMany(obj =>
    {
        obj.Locked = null;
        obj.LockedBy = null;
        obj.LockState = LockState.SoftLocked;

        return Observable.FromAsync(() => _updateLockFunc(obj))
            .NotifyOnError(_notifications, e => $"Error releasing lock: {e.Message}");
    });

    // Current user
    private readonly Func<TItem, Task<TItem>> _updateLockFunc;

    // --- Reducers
    public LockViewModel(Func<TItem, Task<TItem>> updateLockFunc)
    {
        _updateLockFunc = updateLockFunc;

        // Initialize with an empty item and not loading
        _stateSubject = new BehaviorSubject<LockViewModelState<TItem>>(new LockViewModelState<TItem>
            {
                SelectedItem = null,
                Loading = false
            }
        );

        // When user selects a new item, release old one if we had “unlocked” it
        _disposables.Add(SelectItem
            .Subscribe(item =>
            {
                if (item == null)
                {
                    _stateSubject.OnNext(_stateSubject.Value with {SelectedItem = null});
                    return;
                }

                if (item.LockedBy == null)
                    item.LockState = LockState.SoftLocked;
                else if (item.LockedBy == Environment.UserName)
                    item.LockState = LockState.Unlocked;
                else
                    item.LockState = LockState.Locked;

                _stateSubject.OnNext(_stateSubject.Value with {SelectedItem = item});
            })
        );

        // ClaimLockObs reducer
        _disposables.Add(ClaimLockObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(item =>
            {
                _stateSubject.OnNext(_stateSubject.Value with
                {
                    SelectedItem = item
                });
            }));

        // ReleaseLockObs reducer
        _disposables.Add(ReleaseLockObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(item =>
            {
                _stateSubject.OnNext(_stateSubject.Value with
                {
                    SelectedItem = item
                });
            }));
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}