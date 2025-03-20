using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WpfApp1.Shared.Abstract;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.ExtensionMethods;
using WpfApp1.Shared.FormBuilder;
using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Windows.Window8;

public record WindowEightState : BaseState<WindowEightState>
{
    public List<SnackV2> Snacks { get; init; } = [];
    public SnackV2? SelectedSnack { get; init; }
    public FormGroup Form { get; init; }

    public override WindowEightState WithInProgress(bool inProgress) =>
        this with { InProgress = inProgress };
}

public class WindowEightViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly SnackServiceV2 _snackService;

    // --- State
    private readonly BehaviorSubject<WindowEightState> _stateSubject = new(new WindowEightState
    {
        Form = CreateSnackForm(new SnackV2())
    });

    private IObservable<WindowEightState> StateObs => _stateSubject.AsObservable();

    // --- Selectors
    public IObservable<List<SnackV2>> SnacksObs => StateObs.Select(state => state.Snacks);
    public IObservable<SnackV2?> SelectedSnackObs => StateObs.Select(state => state.SelectedSnack);
    public IObservable<bool> LoadingObs => StateObs.Select(state => state.Loading);
    public IObservable<FormGroup> FormObs => StateObs.Select(state => state.Form);
    public IObservable<bool> FormValidObs => StateObs.Select(state => state.Form?.IsValid ?? false);
    public IObservable<bool> FormDirtyObs => StateObs.Select(state => state.Form?.IsDirty ?? false);

    // --- Notifications
    private readonly Subject<NotificationMessage> _notifications = new();
    public IObservable<NotificationMessage> NotificationsObs => _notifications.AsObservable();

    // --- Track original snack states for error recovery
    private SnackV2? _originalBeforeOperation;

    // --- Sources
    public readonly Subject<SnackV2> SelectedSnackChanged = new();
    public readonly Subject<SnackV2> Create = new();
    public readonly Subject<SnackV2> Update = new();
    public readonly Subject<Unit> Reload = new();
    public readonly Subject<int> Delete = new();
    public readonly Subject<Unit> SaveForm = new();

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
        .Do(snack => _originalBeforeOperation = new SnackV2(snack))
        .ExecuteAsyncOperation(
            _stateSubject,
            obj => _snackService.AddSnackAsync(obj),
            _notifications, "Snack added successfully.", e => $"Error creating snack: {e.Message}"
        );

    // Update
    private IObservable<SnackV2?> SnackUpdatedObs => Update
        .Do(snack => _originalBeforeOperation = new SnackV2(snack))
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

    // --- Form handling
    private IObservable<Unit> FormSubmittedObs => SaveForm
        .Do(_ => _stateSubject.Value.Form.MarkAllAsTouched())
        .Where(_ => _stateSubject.Value.Form.IsValid)
        .Select(_ => {
            var formValues = GetSnackFromForm();

            if (formValues.Id is 0 or null)
                Create.OnNext(formValues);
            else
                Update.OnNext(formValues);

            return Unit.Default;
        });

    // --- Reducers
    public WindowEightViewModel()
    {
        _snackService = new SnackServiceV2
        {
            SimulateFailures = true,
            FailureProbability = 0.8,
            FailureProbabilityOnLoad = 0.3
        };

        // SelectedSnackChanged reducer
        _disposables.Add(SelectedSnackChanged
            .ObserveOnCurrentSynchronizationContext()
            .Subscribe(
                snack => {
                    var form = _stateSubject.Value.Form;
                    UpdateFormWithSnack(form, snack);

                    _stateSubject.OnNext(_stateSubject.Value with {
                        SelectedSnack = snack
                    });
                },
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
                            SelectedSnack = _originalBeforeOperation,
                            InProgress = false
                        });

                        // Restore form to original values
                        if (_originalBeforeOperation != null)
                        {
                            UpdateFormWithSnack(_stateSubject.Value.Form, _originalBeforeOperation);
                        }

                        return;
                    }

                    var snacks = new List<SnackV2>(_stateSubject.Value.Snacks) {snack};

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = snacks,
                        SelectedSnack = snack,
                        InProgress = false
                    });

                    // Reset form dirty state
                    _stateSubject.Value.Form.Reset();
                    UpdateFormWithSnack(_stateSubject.Value.Form, snack);
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnackCreated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        SelectedSnack = _originalBeforeOperation,
                        InProgress = false
                    });

                    // Restore form to original values
                    if (_originalBeforeOperation != null)
                    {
                        UpdateFormWithSnack(_stateSubject.Value.Form, _originalBeforeOperation);
                    }
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
                            SelectedSnack = _originalBeforeOperation,
                            InProgress = false
                        });

                        // Restore form to original values
                        if (_originalBeforeOperation != null)
                        {
                            UpdateFormWithSnack(_stateSubject.Value.Form, _originalBeforeOperation);
                        }

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

                    // Reset form dirty state
                    _stateSubject.Value.Form.Reset();
                    UpdateFormWithSnack(_stateSubject.Value.Form, updatedSnack);
                },
                error =>
                {
                    Console.WriteLine($"Unhandled error in SnackUpdated reducer: {error.Message}");
                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        SelectedSnack = _originalBeforeOperation,
                        InProgress = false
                    });

                    // Restore form to original values
                    if (_originalBeforeOperation != null)
                    {
                        UpdateFormWithSnack(_stateSubject.Value.Form, _originalBeforeOperation);
                    }
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
                    if (index >= 0) snacks.RemoveAt(index);

                    _stateSubject.OnNext(_stateSubject.Value with
                    {
                        Snacks = snacks,
                        SelectedSnack = null
                    });

                    // Reset form with empty snack
                    _stateSubject.Value.Form.Reset();
                    UpdateFormWithSnack(_stateSubject.Value.Form, new SnackV2());
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

        // Form submitted reducer
        _disposables.Add(FormSubmittedObs.Subscribe());
    }

    // --- Form utilities
    private static FormGroup CreateSnackForm(SnackV2 snack)
    {
        return FormBuilder.Group(new Dictionary<string, object>
        {
            ["name"] = FormBuilder.Control(snack.Name, new RequiredValidator()),
            ["price"] = FormBuilder.Control(snack.Price, new MinValueValidator<double>(0, "Price must be positive")),
            ["quantity"] = FormBuilder.Control(snack.Quantity, new MinValueValidator<int>(0, "Quantity must be positive"))
        });
    }

    private void UpdateFormWithSnack(FormGroup form, SnackV2 snack)
    {
        form.GetControl<string>("name").SetValue(snack.Name);
        form.GetControl<double>("price").SetValue(snack.Price);
        form.GetControl<int>("quantity").SetValue(snack.Quantity);
    }

    private SnackV2 GetSnackFromForm()
    {
        var selectedSnack = _stateSubject.Value.SelectedSnack ?? new SnackV2();
        var snack = new SnackV2(selectedSnack)
        {
            Name = _stateSubject.Value.Form.GetControl<string>("name").CurrentValue,
            Price = _stateSubject.Value.Form.GetControl<double>("price").CurrentValue,
            Quantity = _stateSubject.Value.Form.GetControl<int>("quantity").CurrentValue
        };

        return snack;
    }

    // --- Dispose
    public void Dispose() => _disposables.Dispose();
}