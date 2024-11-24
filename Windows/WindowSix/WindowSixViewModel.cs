using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Shared.DataAccess;
using WpfApp1.Windows.WindowSix.Shared.Interfaces;

namespace WpfApp1.Windows.WindowSix;

public record WindowSixState
{
    public List<Snack> Snacks { get; init; } = [];
    public Snack? SelectedSnack { get; init; }
    public bool Loading { get; init; } = true;
}

public class WindowSixViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ISnackService _snackService;

    // --- State
    private readonly BehaviorSubject<WindowSixState> _stateSubject = new(new WindowSixState());
    private IObservable<WindowSixState> StateObs => _stateSubject.AsObservable();

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
                .Do(_ => _notifications.OnNext(new NotificationMessage("Snacks loaded successfully.", true)))
                .Catch((Exception e) =>
                {
                    _notifications.OnNext(new NotificationMessage($"Error loading snacks: {e.Message}", false));
                    return Observable.Return(new List<Snack>());
                }));

    // Create
    private IObservable<Snack> SnackCreatedObs => Create.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.AddSnackAsync(obj))
            .Do(_ => _notifications.OnNext(new NotificationMessage("Snack added successfully.", true)))
            .Catch((Exception e) =>
            {
                _notifications.OnNext(new NotificationMessage($"Error creating snack: {e.Message}", false));
                return Observable.Return(new Snack());
            }));

    // Update
    private IObservable<Snack> SnackUpdatedObs => Update.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.UpdateSnackAsync(obj))
            .Do(_ => _notifications.OnNext(new NotificationMessage("Snack updated successfully.", true)))
            .Catch((Exception e) =>
            {
                _notifications.OnNext(new NotificationMessage($"Error updating snack: {e.Message}", false));
                return Observable.Return(new Snack());
            }));

    // Delete
    private IObservable<Snack> SnackDeletedObs => Delete.SelectMany(id =>
        Observable.FromAsync(async () => await _snackService.DeleteSnackAsync(id))
            .Do(_ => _notifications.OnNext(new NotificationMessage("Snack deleted successfully.", true)))
            .Catch((Exception e) =>
            {
                _notifications.OnNext(new NotificationMessage($"Error deleting snack: {e.Message}", false));
                return Observable.Return(new Snack());
            }));

    // --- Reducers
    public WindowSixViewModel()
    {
        _snackService = new SnackService
        {
            SimulateFailures = true,
            FailureProbability = 0.3
        };

        // SelectedSnackChanged reducer
        _disposables.Add(SelectedSnackChanged.Subscribe(snack => { _stateSubject.OnNext(_stateSubject.Value with {SelectedSnack = snack}); }));

        // SnacksLoaded reducer
        _disposables.Add(SnacksLoadedObs.Subscribe(snacks =>
        {
            _stateSubject.OnNext(_stateSubject.Value with
            {
                Snacks = snacks,
                Loading = false
            });
        }));

        // SnackCreated reducer
        _disposables.Add(SnackCreatedObs.Subscribe(snack =>
        {
            if (snack.Id is null) return;
            var snacks = _stateSubject.Value.Snacks;
            snacks.Add(snack);

            _stateSubject.OnNext(_stateSubject.Value with
            {
                Snacks = snacks,
                SelectedSnack = snack
            });
        }));

        // SnackUpdated reducer
        _disposables.Add(SnackUpdatedObs.Subscribe(snack =>
        {
            if (snack.Id is null) return;
            var snacks = _stateSubject.Value.Snacks;
            var index = snacks.FindIndex(s => s.Id == snack.Id);
            snacks[index] = snack;

            _stateSubject.OnNext(_stateSubject.Value with
            {
                Snacks = snacks,
                SelectedSnack = snack
            });
        }));

        // SnackDeleted reducer
        _disposables.Add(SnackDeletedObs.Subscribe(snack =>
        {
            if (snack.Id is null) return;
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