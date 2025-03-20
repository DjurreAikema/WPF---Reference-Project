using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.Shared.FormBuilder.UI;

public partial class FormFieldControl
{
    // --- Dependency Properties
    public static readonly DependencyProperty FormFieldProperty = DependencyProperty.Register(
        nameof(FormField), typeof(object), typeof(FormFieldControl),
        new PropertyMetadata(null, OnFormFieldChanged));

    public static readonly DependencyProperty FieldValueProperty = DependencyProperty.Register(
        nameof(FieldValue), typeof(object), typeof(FormFieldControl),
        new PropertyMetadata(null, OnFieldValueChanged));

    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
        nameof(ErrorMessage), typeof(string), typeof(FormFieldControl),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
        nameof(IsValid), typeof(bool), typeof(FormFieldControl),
        new PropertyMetadata(true, OnIsValidChanged));

    // --- Properties
    public object FormField
    {
        get => GetValue(FormFieldProperty);
        set => SetValue(FormFieldProperty, value);
    }

    public object FieldValue
    {
        get => GetValue(FieldValueProperty);
        set => SetValue(FieldValueProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    // --- Calculated properties
    [ObservableProperty] private Brush _validationBrush = Brushes.Gray;
    [ObservableProperty] private Visibility _errorVisibility = Visibility.Collapsed;

    public FormFieldControl()
    {
        InitializeComponent();
    }

    // --- Event handlers
    private static void OnFormFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FormFieldControl control || e.NewValue == null) return;

        // Set up subscriptions
        control.SetupFieldSubscriptions(e.NewValue);
    }

    private static void OnFieldValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FormFieldControl control || control.FormField == null) return;

        // Update the field value using reflection
        var valueProperty = control.FormField.GetType().GetProperty("CurrentValue");
        var setValue = control.FormField.GetType().GetMethod("SetValue");

        // Only call SetValue if the new value is different from CurrentValue
        var currentValue = valueProperty?.GetValue(control.FormField);
        if (!Equals(e.NewValue, currentValue))
        {
            setValue?.Invoke(control.FormField, new[] { e.NewValue });
        }
    }

    private static void OnIsValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FormFieldControl control) return;

        control.ValidationBrush = (bool)e.NewValue ? Brushes.Gray : Brushes.Red;
    }

    private void SetupFieldSubscriptions(object field)
    {
        // Get observable properties using reflection
        var valueProperty = field.GetType().GetProperty("Value");
        var validProperty = field.GetType().GetProperty("Valid");
        var errorProperty = field.GetType().GetProperty("Error");

        // Subscribe to value changes
        if (valueProperty?.GetValue(field) is IObservable<object> valueObs)
        {
            Disposables.Add(valueObs.Subscribe(value =>
            {
                // Check if FieldValue is different from the incoming value to avoid circular updates
                if (!Equals(FieldValue, value))
                {
                    FieldValue = value;
                }
            }));
        }

        // Subscribe to validity changes
        if (validProperty?.GetValue(field) is IObservable<bool> validObs)
        {
            Disposables.Add(validObs.Subscribe(valid =>
            {
                IsValid = valid;
                ErrorVisibility = valid ? Visibility.Collapsed : Visibility.Visible;
            }));
        }

        // Subscribe to error message changes
        if (errorProperty?.GetValue(field) is IObservable<string> errorObs)
        {
            Disposables.Add(errorObs.Subscribe(error =>
            {
                ErrorMessage = error;
            }));
        }

        // Get initial values
        var currentValueProperty = field.GetType().GetProperty("CurrentValue");
        if (currentValueProperty != null)
        {
            FieldValue = currentValueProperty.GetValue(field);
        }

        var isValidProperty = field.GetType().GetProperty("IsValid");
        if (isValidProperty != null && isValidProperty.GetValue(field) is bool isValid)
        {
            IsValid = isValid;
            ErrorVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
        }

        var currentErrorProperty = field.GetType().GetProperty("CurrentError");
        if (currentErrorProperty != null)
        {
            ErrorMessage = (string)currentErrorProperty.GetValue(field);
        }
    }
}