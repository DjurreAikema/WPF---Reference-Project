using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.FormBuilder;

namespace WpfApp1.Windows.Window8.UI;

public partial class SnackDetailsEight
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<SnackV2?>), typeof(SnackDetailsEight),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsEight c) return;
            c.Disposables.Add(c.SelectedSnackObs.Subscribe(snack =>
            {
                c.SelectedSnack = snack != null ? new SnackV2(snack) : null;
                c.HasId = snack?.Id != null && snack.Id > 0;
            }));
        }));

    public static readonly DependencyProperty FormObsProperty = DependencyProperty.Register(
        nameof(FormObs), typeof(IObservable<FormGroup>), typeof(SnackDetailsEight),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsEight c) return;
            c.Disposables.Add(c.FormObs.Subscribe(form =>
            {
                c.Form = form;
                c.UpdateFormFields();
            }));
        }));

    public static readonly DependencyProperty FormValidObsProperty = DependencyProperty.Register(
        nameof(FormValidObs), typeof(IObservable<bool>), typeof(SnackDetailsEight),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsEight c) return;
            c.Disposables.Add(c.FormValidObs.Subscribe(isValid =>
            {
                c.IsFormValid = isValid;
                c.UpdateFormStatus();
            }));
        }));

    public static readonly DependencyProperty FormDirtyObsProperty = DependencyProperty.Register(
        nameof(FormDirtyObs), typeof(IObservable<bool>), typeof(SnackDetailsEight),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsEight c) return;
            c.Disposables.Add(c.FormDirtyObs.Subscribe(isDirty =>
            {
                c.IsFormDirty = isDirty;
                c.UpdateFormStatus();
            }));
        }));

    // Observables
    public IObservable<SnackV2?> SelectedSnackObs
    {
        get => (IObservable<SnackV2?>) GetValue(SelectedSnackObsProperty);
        set => SetValue(SelectedSnackObsProperty, value);
    }

    public IObservable<FormGroup> FormObs
    {
        get => (IObservable<FormGroup>) GetValue(FormObsProperty);
        set => SetValue(FormObsProperty, value);
    }

    public IObservable<bool> FormValidObs
    {
        get => (IObservable<bool>) GetValue(FormValidObsProperty);
        set => SetValue(FormValidObsProperty, value);
    }

    public IObservable<bool> FormDirtyObs
    {
        get => (IObservable<bool>) GetValue(FormDirtyObsProperty);
        set => SetValue(FormDirtyObsProperty, value);
    }

    // --- Events
    public event Action<SnackV2>? SnackSaved;
    public event Action<int>? SnackDeleted;
    public event Action? FormSubmitted;

    // --- Internal Properties
    [ObservableProperty] private SnackV2? _selectedSnack;
    [ObservableProperty] private FormGroup? _form;
    [ObservableProperty] private object? _nameField;
    [ObservableProperty] private object? _priceField;
    [ObservableProperty] private object? _quantityField;
    [ObservableProperty] private bool _isFormValid;
    [ObservableProperty] private bool _isFormDirty;
    [ObservableProperty] private bool _hasId;
    [ObservableProperty] private string _formStatusText = "Ready";

    // --- Constructor
    public SnackDetailsEight()
    {
        InitializeComponent();
    }

    private void UpdateFormFields()
    {
        if (Form == null) return;

        NameField = Form.GetControlDynamic("name");
        PriceField = Form.GetControlDynamic("price");
        QuantityField = Form.GetControlDynamic("quantity");
    }

    private void UpdateFormStatus()
    {
        FormStatusText = IsFormValid ?
            (IsFormDirty ? "Valid but not saved" : "Valid and saved") :
            "Invalid - please check errors";
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        FormSubmitted?.Invoke();
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectedSnack == null || SelectedSnack.Id is 0 or null) return;
        SnackDeleted?.Invoke(SelectedSnack.Id.Value);
    }
}