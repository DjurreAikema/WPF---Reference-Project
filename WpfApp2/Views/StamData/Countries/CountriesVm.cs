using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Data.Classes.System;
using WpfApp2.Data.DataAccess;
using WpfApp2.Shared.Abstract;
using WpfApp2.Shared.Debugging.Extensions;
using WpfApp2.Shared.ExtensionMethods;

namespace WpfApp2.Views.StamData.Countries;

public record CountriesState : BaseState<CountriesState>
{
    public List<Country> Countries { get; init; } = [];
    public Country? Selected { get; init; }

    public override CountriesState WithInProgress(bool inProgress) =>
        this with {InProgress = inProgress};
}

public class CountriesViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly CountryService _countryService;

    // --- State
    private readonly BehaviorSubject<CountriesState> _stateSubject = new(new CountriesState());
    private IObservable<CountriesState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<Country>> CountriesObs => StateObs.Select(state => state.Countries);
    public IObservable<Country?> SelectedObs => StateObs.Select(state => state.Selected);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Track for error recovery
    private Country? _beforeOperation;

    // --- Sources
    public readonly Subject<Country> SelectedChanged = new();
    public readonly Subject<Country> Create = new();
    public readonly Subject<Country> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();

    // Load
    private IObservable<List<Country>?> LoadedObs =>
        Reload.StartWith(Unit.Default)
            .ExecuteAsyncOperation(
                _stateSubject,
                _ => _countryService.GetAllAsync(),
                _notifications, "Countries loaded successfully.", e => $"Error loading countries: {e.Message}", []
            );

    // Create
    private IObservable<Country?> CreatedObs => Create
        .Do(obj => _beforeOperation = new Country(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _countryService.AddAsync(obj),
            _notifications, "Country added successfully.", e => $"Error creating country: {e.Message}"
        );

    // Update
    private IObservable<Country?> UpdatedObs => Update
        .Do(obj => _beforeOperation = new Country(obj))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _countryService.UpdateAsync(obj),
            _notifications, "Country updated successfully.", e => $"Error updating country: {e.Message}"
        );

    // Delete
    private IObservable<Country?> DeletedObs => Delete
        .ExecuteAsyncOperation(
            _stateSubject,
            id => _countryService.DeleteAsync(id),
            _notifications, "Country deleted successfully.", e => $"Error deleting country: {e.Message}"
        );

    // --- Reducers
    public CountriesViewModel()
    {
        _countryService = new CountryService
        {
            SimulateFailures = false,
            FailureProbability = 0.1,
            FailureProbabilityOnLoad = 0
        };

        // SelectedChanged reducer
        _disposables.Add(SelectedChanged
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(
                obj => { _stateSubject.OnNext(_stateSubject.Value with {Selected = obj}); },
                error => Console.WriteLine($"Unexpected error in SelectedChanged: {error.Message}")
            ));

        // Loaded reducer
        _disposables.Add(LoadedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(objs =>
                {
                    if (objs is null) return;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Countries = [..objs],
                        Loading = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Loaded reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Loading = false
                    });
                }));

        // Created reducer
        _disposables.Add(CreatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            Selected = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var objs = new List<Country>(_stateSubject.Value.Countries) {obj};

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Countries = objs,
                        Selected = obj,
                        InProgress = false
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Created reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Selected = _beforeOperation,
                        InProgress = false
                    });
                }
            ));

        // Updated reducer
        _disposables.Add(UpdatedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(updated =>
                {
                    if (updated is null)
                    {
                        _stateSubject.OnNext(_stateSubject.Value with
                        {
                            Selected = _beforeOperation,
                            InProgress = false
                        });
                        return;
                    }

                    var objs = new List<Country>(_stateSubject.Value.Countries);
                    var index = objs.FindIndex(s => s.Id == updated.Id);
                    if (index < 0) return;
                    objs[index] = updated;

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Countries = objs,
                        Selected = updated
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Updated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Selected = _beforeOperation,
                        InProgress = false
                    });
                }
            ));

        // Deleted reducer
        _disposables.Add(DeletedObs
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(obj =>
                {
                    if (obj is null) return;
                    var objs = new List<Country>(_stateSubject.Value.Countries);
                    var index = objs.FindIndex(s => s.Id == obj.Id);
                    objs.RemoveAt(index);

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Countries = objs,
                        Selected = null
                    });
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in Deleted reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        InProgress = false
                    });
                }
            ));
    }

    // --- Dispose
    public void Dispose()
    {
        this.UnregisterFromTracker();
        _disposables.Dispose();
    }
}