using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.ExtensionMethods;
using WpfApp1.Shared.Interfaces;

namespace WpfApp1.Windows.Window7._2;

public record WindowSevenTwoState : IBaseState
{
    public List<SnackV2> Snacks { get; init; } = [];
    public SnackV2? SelectedSnack { get; init; }
    public bool Loading { get; init; } = true;
    public bool InProgress { get; init; }
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

    // --- Sources
    public readonly Subject<SnackV2> SelectedSnackChanged = new();
    public readonly Subject<SnackV2> Create = new();
    public readonly Subject<SnackV2> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

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

    private IObservable<SnackV2?> SnackCreatedObs2 => Create
        .Synchronize()
        .Do(_ => _stateSubject.OnNext(_stateSubject.Value with {InProgress = true}))
        .SelectMany(obj => Observable.FromAsync(() => _snackService.AddSnackAsync(obj)))
        .Do(
            _ => { }, // On next (success) - do nothing here, handled by NotifyOnSuccess
            _ => { }, // On error - do nothing here, handled by NotifyOnError
            () => _stateSubject.OnNext(_stateSubject.Value with {InProgress = false}) // On complete
        )
        .NotifyOnSuccessAndError(_notifications,
            "Snack added successfully.",
            e => $"Error creating snack: {e.Message}");


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
        _disposables.Add(SnackCreatedObs2
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
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}