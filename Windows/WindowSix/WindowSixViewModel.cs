using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Interfaces;
using WpfApp1.Windows.WindowSix.Services;

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

    // --- Sources
    public readonly Subject<Snack> SelectedSnackChanged = new();
    public readonly Subject<Snack> Create = new();
    public readonly Subject<Snack> Update = new();
    public readonly Subject<int> Delete = new();

    private IObservable<Snack> SnackCreatedObs => Create.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.AddSnackAsync(obj))
            .Catch((Exception e) =>
            {
                Console.WriteLine($"Error creating snack: {e.Message}");
                return Observable.Return(new Snack());
            }));

    private IObservable<Snack> SnackUpdatedObs => Update.SelectMany(obj =>
        Observable.FromAsync(async () => await _snackService.UpdateSnackAsync(obj))
            .Catch((Exception e) =>
            {
                Console.WriteLine($"Error updating snack: {e.Message}");
                return Observable.Return(new Snack());
            }));

    private IObservable<Snack> SnackDeletedObs => Delete.SelectMany(id =>
        Observable.FromAsync(async () => await _snackService.DeleteSnackAsync(id))
            .Catch((Exception e) =>
            {
                Console.WriteLine($"Error deleting snack: {e.Message}");
                return Observable.Return(new Snack());
            }));

    private IObservable<List<Snack>> SnacksLoadedObs => Observable.FromAsync(_snackService.GetAllSnacksAsync);

    // --- Reducers
    public WindowSixViewModel()
    {
        _snackService = new SnackService();

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