using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.ExtensionMethods;
using WpfApp1.Shared.Interfaces;

namespace WpfApp1.Windows.Window7._1;

public record WindowSevenOneState
{
    public List<Snack> Snacks { get; init; } = [];
    public Snack? SelectedSnack { get; init; }
    public bool Loading { get; init; } = true;
}

public class WindowSevenOneViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ISnackService _snackService;

    // --- State
    private readonly BehaviorSubject<WindowSevenOneState> _stateSubject = new(new WindowSevenOneState());
    private IObservable<WindowSevenOneState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<Snack>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<Snack?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Sources
    public readonly Subject<Snack> SelectedSnackChanged = new();
    public readonly Subject<Snack> Create = new();
    public readonly Subject<Snack> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // Load
    private IObservable<List<Snack>> SnacksLoadedObs =>
        Reload.StartWith(Unit.Default)
            .SelectMany(_ => Observable.FromAsync(_snackService.GetAllSnacksAsync)
                .NotifyOnSuccessAndError(_notifications,
                    "Snacks loaded successfully.",
                    e => $"Error loading snacks: {e.Message}",
                    new List<Snack>()));

    // Create
    private IObservable<Snack?> SnackCreatedObs => Create.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.AddSnackAsync(obj))
            .NotifyOnSuccessAndError(_notifications,
                "Snack added successfully.",
                e => $"Error creating snacks: {e.Message}"));

    // Update
    private IObservable<Snack?> SnackUpdatedObs => Update.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.UpdateSnackAsync(obj))
            .NotifyOnSuccessAndError(_notifications,
                "Snack updated successfully.",
                e => $"Error updating snacks: {e.Message}"));

    // Delete
    private IObservable<Snack?> SnackDeletedObs => Delete.SelectMany(id =>
        Observable.FromAsync(async () => await _snackService.DeleteSnackAsync(id))
            .NotifyOnSuccessAndError(_notifications,
                "Snack deleted successfully.",
                e => $"Error deleting snacks: {e.Message}"));

    // --- Reducers
    public WindowSevenOneViewModel()
    {
        _snackService = new SnackService
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
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}