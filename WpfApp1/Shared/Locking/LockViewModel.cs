using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WpfApp1.Shared.Locking;

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
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Sources
    public Subject<TItem?> SelectItem { get; } = new(); // user picks a new item
    public Subject<TItem?> ClaimLock { get; } = new(); // user tries to claim lock (unlock for themselves)
    public Subject<TItem?> ReleaseLock { get; } = new(); // user leaves the item or closes window

    // Current user
    private readonly string _currentUser = "UserA";

    // --- Reducers
    public LockViewModel()
    {
        // Initialize with an empty item and not loading
        _stateSubject = new BehaviorSubject<LockViewModelState<TItem>>(
            new LockViewModelState<TItem>
            {
                SelectedItem = null,
                Loading = false
            }
        );

        // When user selects a new item, release old one if we had “unlocked” it
        _disposables.Add(SelectItem
            .Subscribe(item =>
            {
                var previous = _stateSubject.Value.SelectedItem;
                if (previous != null && previous.LockState == LockState.Unlocked && previous.LockedBy == _currentUser)
                {
                    // Turn it back to SoftLocked so other users can lock it
                    previous.LockState = LockState.SoftLocked;
                    previous.LockedBy = null;
                    // Optionally update DB: “ReleaseLockInDbAsync(previous)”
                }

                if (item == null)
                {
                    // No new item selected
                    _stateSubject.OnNext(_stateSubject.Value with {SelectedItem = null});
                }
                else
                {
                    // Evaluate item’s current lock state
                    if (item.LockedBy == null)
                    {
                        // Means nobody has locked it => SoftLocked
                        item.LockState = LockState.SoftLocked;
                    }
                    else if (item.LockedBy == _currentUser)
                    {
                        // Means user had locked it previously => Unlocked
                        item.LockState = LockState.Unlocked;
                    }
                    else
                    {
                        // Another user locked it => Locked
                        item.LockState = LockState.Locked;
                    }

                    // Next state
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        SelectedItem = item
                    });
                }
            })
        );

        // Claim lock means: if item is SoftLocked => set to Unlocked for the current user
        _disposables.Add(ClaimLock
            .Subscribe(current =>
            {
                if (current is null) return;
                if (current.LockState != LockState.SoftLocked) return;

                current.LockState = LockState.Unlocked;
                current.LockedBy = _currentUser;
                // Optionally persist to DB
                // await _snackService.UpdateSnackLockAsync(current, _currentUser);

                _stateSubject.OnNext(_stateSubject.Value with {SelectedItem = current});
            })
        );

        // Release lock means: if item is Unlocked by me => set to SoftLocked
        _disposables.Add(ReleaseLock
            .Subscribe(current =>
            {
                if (current is null) return;
                if (current.LockState != LockState.Unlocked || current.LockedBy != _currentUser) return;

                current.LockState = LockState.SoftLocked;
                current.LockedBy = null;
                // Optionally update DB
                // await _snackService.UpdateSnackLockAsync(current, null);

                _stateSubject.OnNext(_stateSubject.Value with {SelectedItem = current});
            })
        );
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}