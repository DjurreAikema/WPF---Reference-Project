using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour;

public record WindowFourState
{
    public List<Snack> Snacks { get; set; } = [];
    public Snack? SelectedSnack { get; set; } = new();
    public bool Loading { get; set; } = true;
}

public class WindowFourViewModel
{
    private readonly CompositeDisposable _disposables = new();

    // --- State
    private readonly BehaviorSubject<WindowFourState> _stateSubject = new(new WindowFourState());
    private IObservable<WindowFourState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<Snack>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<Snack?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Sources
    public readonly Subject<Snack> SelectedSnackChanged = new();
    private IObservable<List<Snack>> SnacksLoadedObs => GetSnacks();

    // --- Reducers
    public WindowFourViewModel()
    {
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

    // --- Functions
    private static IObservable<List<Snack>> GetSnacks()
    {
        return Observable.FromAsync(async () =>
        {
            // Simulate API delay
            var random = new Random();
            var delay = random.Next(500, 2000);
            await Task.Delay(delay);

            // Return the data
            return new List<Snack>
            {
                new() {Name = "Doritos", Price = 1.50, Quantity = 4},
                new() {Name = "Lays", Price = 1.00, Quantity = 3},
                new() {Name = "Pringles", Price = 2.00, Quantity = 2},
                new() {Name = "Cheetos", Price = 1.25, Quantity = 5},
                new() {Name = "Ruffles", Price = 1.75, Quantity = 1},
                new() {Name = "Tostitos", Price = 1.50, Quantity = 6},
                new() {Name = "Sun Chips", Price = 1.25, Quantity = 7},
            };
        });
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}