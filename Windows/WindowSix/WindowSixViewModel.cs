using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Interfaces;
using WpfApp1.Windows.WindowSix.Services;

namespace WpfApp1.Windows.WindowSix;

public record WindowSixState
{
    public IEnumerable<Snack> Snacks { get; init; } = [];
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
    public IObservable<IEnumerable<Snack>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<Snack?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Sources
    public readonly Subject<Snack> SelectedSnackChanged = new();
    private IObservable<IEnumerable<Snack>> SnacksLoadedObs => Observable.FromAsync(_snackService.GetAllSnacksAsync);

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
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}