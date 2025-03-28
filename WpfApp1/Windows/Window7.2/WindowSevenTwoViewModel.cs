using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Abstract;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.ExtensionMethods;

namespace WpfApp1.Windows.Window7._2;

public record WindowSevenTwoState : BaseState<WindowSevenTwoState>
{
    public List<SnackV2> Snacks { get; init; } = [];
    public SnackV2? SelectedSnack { get; init; }

    public override WindowSevenTwoState WithInProgress(bool inProgress) =>
        this with {InProgress = inProgress};
}

public class WindowSevenTwoViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly SnackServiceV2 _snackService;

    // --- State
    private readonly BehaviorSubject<WindowSevenTwoState> _stateSubject = new(new WindowSevenTwoState());
    private IObservable<WindowSevenTwoState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<SnackV2>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<SnackV2?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Track snack states for error recovery
    private SnackV2? _beforeOperation;

    // --- Sources
    public readonly Subject<SnackV2> SelectedSnackChanged = new();
    public readonly Subject<SnackV2> Create = new();
    public readonly Subject<SnackV2> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // Load
    private IObservable<List<SnackV2>?> SnacksLoadedObs =>
        Reload.StartWith(Unit.Default)
            .ExecuteAsyncOperation(
                _stateSubject,
                _ => _snackService.GetAllSnacksAsync(),
                _notifications, "Snacks loaded successfully.", e => $"Error loading snacks: {e.Message}", []
            );

    // Create
    private IObservable<SnackV2?> SnackCreatedObs => Create
        .Do(snack => _beforeOperation = new SnackV2(snack))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.AddSnackAsync(obj),
            _notifications, "Snack added successfully.", e => $"Error creating snack: {e.Message}"
        );

    // Update
    private IObservable<SnackV2?> SnackUpdatedObs => Update
        .Do(snack => _beforeOperation = new SnackV2(snack))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.UpdateSnackAsync(obj),
            _notifications, "Snack updated successfully.", e => $"Error updating snack: {e.Message}"
        );

    // Delete
    private IObservable<SnackV2?> SnackDeletedObs => Delete
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _snackService.DeleteSnackAsync(id),
            _notifications, "Snack deleted successfully.", e => $"Error deleting snack: {e.Message}"
        );

    // --- Reducers
    public WindowSevenTwoViewModel()
    {
        _snackService = new SnackServiceV2
        {
            SimulateFailures = true,
            FailureProbability = 0.3,
            FailureProbabilityOnLoad = 0.3
        };

        // SelectedSnackChanged reducer
        _disposables.Add(SelectedSnackChanged
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(
                snack => { _stateSubject.OnNext(_stateSubject.Value with {SelectedSnack = snack}); },
                error => Console.WriteLine($"Unexpected error in SelectedSnackChanged: {error.Message}")
            ));

        // SnacksLoaded reducer
        _disposables.Add(SnacksLoadedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snacks =>
                {
                    if (snacks is null) return;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = [..snacks],
                        Loading = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnacksLoaded reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Loading = false
                    });
                }));

        // SnackCreated reducer
        _disposables.Add(SnackCreatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snack =>
                {
                    if (snack is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            SelectedSnack = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var snacks = new List<SnackV2>(_stateSubject.Value.Snacks) {snack};

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = snacks,
                        SelectedSnack = snack,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnackCreated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        SelectedSnack = _beforeOperation,
                        InProgress = false
                    });
                }
            ));

        // SnackUpdated reducer
        _disposables.Add(SnackUpdatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(updatedSnack =>
                {
                    if (updatedSnack is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            SelectedSnack = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var snacks = new List<SnackV2>(_stateSubject.Value.Snacks);
                    var index = snacks.FindIndex(s => s.Id == updatedSnack.Id);
                    if (index < 0) return;
                    snacks[index] = updatedSnack;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = snacks,
                        SelectedSnack = updatedSnack
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnackUpdated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        SelectedSnack = _beforeOperation,
                        InProgress = false
                    });
                }
            ));

        // SnackDeleted reducer
        _disposables.Add(SnackDeletedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(snack =>
                {
                    if (snack is null) return;
                    var snacks = new List<SnackV2>(_stateSubject.Value.Snacks);
                    var index = snacks.FindIndex(s => s.Id == snack.Id);
                    snacks.RemoveAt(index);

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = snacks,
                        SelectedSnack = null
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnackDeleted reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        InProgress = false
                    });
                }
            ));
    }

    // --- Dispose
    public void Dispose() => _disposables.Dispose();
}

// List of things to add/improve
// 1 - Cant use arrow keys to move through list
// 2 - Add combobox to form
// 3 - Add Stamdata to application (connect to combobox)
// 4 - Add a form array to snacks
//      - Calculate a total from this form array
// 5 - Add locking to form
// 6 - Add unsaved changes handling
// 7 - Have a checkbox enable and disable part of the form
// 8 - Add validation to the form
//      - Manage UI based on form state
// 9 - Save button should show why it cant save the form
// 10 - Refresh button
