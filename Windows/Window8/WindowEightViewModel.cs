using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.Interfaces;

namespace WpfApp1.Windows.Window8;

public record WindowEightState
{
    public List<Snack> Snacks { get; init; } = [];
    public Snack? SelectedSnack { get; init; }
    public bool Loading { get; init; } = true;
}

public class WindowEightViewModel : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ISnackService _snackService;

    // --- State
    private readonly BehaviorSubject<WindowEightState> _stateSubject = new(new WindowEightState());
    private IObservable<WindowEightState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    [Reactive] public List<Snack> Snacks { get; set; } = [];
    [Reactive] public Snack? SelectedSnack { get; set; }
    [Reactive] public bool IsLoading { get; set; } = true;

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Sources
    public readonly Subject<Snack> SelectedSnackChanged = new();

    public ReactiveCommand<Unit, List<Snack>> LoadSnacksCommand { get; }
    public ReactiveCommand<Snack, Snack> CreateSnackCommand { get; }
    public ReactiveCommand<Snack, Snack> UpdateSnackCommand { get; }
    public ReactiveCommand<int, Snack> DeleteSnackCommand { get; }

    // --- Reducers
    public WindowEightViewModel()
    {
        _snackService = new SnackService
        {
            SimulateFailures = true,
            FailureProbability = 0.7,
            FailureProbabilityOnLoad = 0.3
        };

        // Load Snacks Command
        LoadSnacksCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsLoading = true;
            try
            {
                var snacks = await _snackService.GetAllSnacksAsync();
                Snacks = snacks;
                _notifications.OnNext(new NotificationMessage("Snacks loaded successfully.", true));
                return snacks;
            }
            catch (Exception e)
            {
                _notifications.OnNext(new NotificationMessage($"Error loading snacks: {e.Message}", false));
                return [];
            }
            finally
            {
                IsLoading = false;
            }
        });

        // Create Snack Command
        CreateSnackCommand = ReactiveCommand.CreateFromTask<Snack, Snack>(async snack =>
        {
            try
            {
                var createdSnack = await _snackService.AddSnackAsync(snack);
                _notifications.OnNext(new NotificationMessage("Snack added successfully.", true));
                return createdSnack;
            }
            catch (Exception e)
            {
                _notifications.OnNext(new NotificationMessage($"Error creating snack: {e.Message}", false));
                return null;
            }
        });

        // Update Snack Command
        UpdateSnackCommand = ReactiveCommand.CreateFromTask<Snack, Snack>(async snack =>
        {
            try
            {
                var updatedSnack = await _snackService.UpdateSnackAsync(snack);
                _notifications.OnNext(new NotificationMessage("Snack updated successfully.", true));
                return updatedSnack;
            }
            catch (Exception e)
            {
                _notifications.OnNext(new NotificationMessage($"Error updating snack: {e.Message}", false));
                return null;
            }
        });

        // Delete Snack Command
        DeleteSnackCommand = ReactiveCommand.CreateFromTask<int, Snack>(async id =>
        {
            try
            {
                var deletedSnack = await _snackService.DeleteSnackAsync(id);
                _notifications.OnNext(new NotificationMessage("Snack deleted successfully.", true));
                return deletedSnack;
            }
            catch (Exception e)
            {
                _notifications.OnNext(new NotificationMessage($"Error deleting snack: {e.Message}", false));
                return null;
            }
        });

        // Handle the results of the commands
        LoadSnacksCommand.Subscribe(snacks => { Snacks = snacks; });

        CreateSnackCommand.Subscribe(snack =>
        {
            Snacks.Add(snack);
            SelectedSnack = snack;
        });

        UpdateSnackCommand.Subscribe(snack =>
        {
            var index = Snacks.FindIndex(s => s.Id == snack.Id);
            if (index < 0) return;

            Snacks[index] = snack;
            SelectedSnack = snack;
        });

        DeleteSnackCommand.Subscribe(snack =>
        {
            Snacks.Remove(snack);
            SelectedSnack = null;
        });
    }

    // --- Dispose
    public void Dispose()
    {
        _disposables.Dispose();
    }
}