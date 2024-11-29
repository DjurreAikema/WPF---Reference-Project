using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.Interfaces;
using WpfApp1.Windows.Window8.UI;

namespace WpfApp1.Windows.Window8;

public class WindowEightViewModel : ReactiveObject
{
    private readonly ISnackService _snackService;

    // Child ViewModels
    public SnacksGridEightViewModel SnacksGridViewModel { get; }
    public SnackDetailsEightViewModel SnackDetailsViewModel { get; }

    // --- Selectors
    [Reactive] public List<Snack> Snacks { get; set; } = [];
    [Reactive] public Snack? SelectedSnack { get; set; }
    [Reactive] public bool IsLoading { get; set; } = true;

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Sources
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

        // Initialize child ViewModels
        SnacksGridViewModel = new SnacksGridEightViewModel();
        SnackDetailsViewModel = new SnackDetailsEightViewModel();

        // Initialize commands
        var canExecuteSnackSelected = this.WhenAnyValue(vm => vm.SnackDetailsViewModel.Snack)
            .Select(snack => snack != null);

        // Load Snacks Command
        var loadSnacksCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var snacks = await _snackService.GetAllSnacksAsync();
            SnacksGridViewModel.Snacks = new ObservableCollection<Snack>(snacks);
        });

        // Add Snack Command
        var addSnackCommand = ReactiveCommand.Create(() =>
        {
            var newSnack = new Snack();
            SnackDetailsViewModel.Snack = newSnack;
        });

        // Save Snack Command
        var saveSnackCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var snack = SnackDetailsViewModel.Snack;
            if (snack == null) return;

            if (snack.Id == null || snack.Id == 0)
            {
                // Create new snack
                var createdSnack = await _snackService.AddSnackAsync(snack);
                SnacksGridViewModel.Snacks.Add(createdSnack);
                SnackDetailsViewModel.Snack = createdSnack;
            }
            else
            {
                // Update existing snack
                var updatedSnack = await _snackService.UpdateSnackAsync(snack);
                var index = SnacksGridViewModel.Snacks.IndexOf(snack);
                if (index >= 0)
                {
                    SnacksGridViewModel.Snacks[index] = updatedSnack;
                }

                SnackDetailsViewModel.Snack = updatedSnack;
            }
        }, canExecuteSnackSelected);

        // Delete Snack Command
        var deleteSnackCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var snack = SnackDetailsViewModel.Snack;
            if (snack == null || snack.Id == null) return;

            await _snackService.DeleteSnackAsync(snack.Id.Value);
            SnacksGridViewModel.Snacks.Remove(snack);
            SnackDetailsViewModel.Snack = null;
        }, canExecuteSnackSelected);

        // Assign commands to child ViewModels
        SnacksGridViewModel.ReloadCommand = loadSnacksCommand;
        SnacksGridViewModel.AddCommand = addSnackCommand;

        SnackDetailsViewModel.SaveCommand = saveSnackCommand;
        SnackDetailsViewModel.DeleteCommand = deleteSnackCommand;

        // Handle selection changes
        SnacksGridViewModel.WhenAnyValue(vm => vm.SelectedSnack)
            .BindTo(this, vm => vm.SnackDetailsViewModel.Snack);

        // Load initial data
        loadSnacksCommand.Execute().Subscribe();
    }
}