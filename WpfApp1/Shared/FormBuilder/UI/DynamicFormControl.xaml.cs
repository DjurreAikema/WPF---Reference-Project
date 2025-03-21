using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Shared.FormBuilder.UI
{
    public class FormFieldInfo
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public object Control { get; set; }
        public bool IsRequired { get; set; }
    }

    public partial class DynamicFormControl
    {
        // --- Dependency Properties
        public static readonly DependencyProperty FormObsProperty = DependencyProperty.Register(
            nameof(FormObs), typeof(IObservable<FormGroup>), typeof(DynamicFormControl),
            new PropertyMetadata(null, OnFormObsChanged));

        public static readonly DependencyProperty FormTitleProperty = DependencyProperty.Register(
            nameof(FormTitle), typeof(string), typeof(DynamicFormControl),
            new PropertyMetadata("Form"));

        public static readonly DependencyProperty SubmitButtonTextProperty = DependencyProperty.Register(
            nameof(SubmitButtonText), typeof(string), typeof(DynamicFormControl),
            new PropertyMetadata("Save"));

        public static readonly DependencyProperty CancelButtonTextProperty = DependencyProperty.Register(
            nameof(CancelButtonText), typeof(string), typeof(DynamicFormControl),
            new PropertyMetadata("Cancel"));

        // --- Properties
        public IObservable<FormGroup> FormObs
        {
            get => (IObservable<FormGroup>)GetValue(FormObsProperty);
            set => SetValue(FormObsProperty, value);
        }

        public string FormTitle
        {
            get => (string)GetValue(FormTitleProperty);
            set => SetValue(FormTitleProperty, value);
        }

        public string SubmitButtonText
        {
            get => (string)GetValue(SubmitButtonTextProperty);
            set => SetValue(SubmitButtonTextProperty, value);
        }

        public string CancelButtonText
        {
            get => (string)GetValue(CancelButtonTextProperty);
            set => SetValue(CancelButtonTextProperty, value);
        }

        // --- Internal properties
        [ObservableProperty] private FormGroup _form;
        [ObservableProperty] private ObservableCollection<FormFieldInfo> _formFields = new();
        [ObservableProperty] private bool _isFormValid = false;
        [ObservableProperty] private bool _isFormDirty = false;
        [ObservableProperty] private string _formStatusText = "Ready";

        // --- Events
        public event Action FormSubmitted;
        public event Action FormCancelled;

        public DynamicFormControl()
        {
            InitializeComponent();
        }

        // --- Event handlers
        private static void OnFormObsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DynamicFormControl control || e.NewValue == null) return;

            // Set up subscriptions to the form
            control.Disposables.Add(((IObservable<FormGroup>)e.NewValue).Subscribe(form =>
            {
                control.Form = form;
                control.BuildFormFields();

                // Subscribe to form state changes
                control.Disposables.Add(form.Valid.Subscribe(valid =>
                {
                    control.IsFormValid = valid;
                    control.UpdateFormStatus();
                }));

                control.Disposables.Add(form.Dirty.Subscribe(dirty =>
                {
                    control.IsFormDirty = dirty;
                    control.UpdateFormStatus();
                }));
            }));
        }

        private void BuildFormFields()
        {
            if (Form == null) return;

            FormFields.Clear();

            foreach (var controlName in Form.ControlNames)
            {
                var control = Form.GetControlDynamic(controlName);
                var propertyInfo = GetPropertyInfoFromControlName(controlName);

                // Get FormProperty attribute if it exists
                var formAttr = propertyInfo?.GetCustomAttribute<FormPropertyAttribute>();

                // Check for Required validation attribute or RequiredValidator
                bool isRequired = IsFieldRequired(propertyInfo, control);

                // Create a user-friendly label from the control name or attribute
                var label = formAttr?.Label;
                if (string.IsNullOrEmpty(label))
                {
                    label = MakeReadableLabel(controlName);
                }

                var description = formAttr?.Description ?? string.Empty;
                var order = formAttr?.Order ?? 999;

                FormFields.Add(new FormFieldInfo
                {
                    Name = controlName,
                    Label = label,
                    Description = description,
                    Order = order,
                    Control = control,
                    IsRequired = isRequired
                });
            }

            // Sort by Order property
            FormFields = new ObservableCollection<FormFieldInfo>(
                FormFields.OrderBy(f => f.Order).ToList()
            );
        }

        private bool IsFieldRequired(PropertyInfo propertyInfo, object control)
        {
            // Check for Required validation attribute
            if (propertyInfo?.GetCustomAttributes<RequiredAttribute>(true).Any() == true)
            {
                return true;
            }

            // Check for RequiredValidator in the control
            try
            {
                // Get the validators from the form field
                var validatorsField = control.GetType().GetField("_validators", BindingFlags.NonPublic | BindingFlags.Instance);
                if (validatorsField != null)
                {
                    var validators = validatorsField.GetValue(control) as System.Collections.IList;
                    if (validators != null)
                    {
                        // Check if any validators are RequiredValidator
                        foreach (var validator in validators)
                        {
                            if (validator.GetType().Name == "RequiredValidator")
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore any reflection errors
            }

            return false;
        }

        private PropertyInfo GetPropertyInfoFromControlName(string controlName)
        {
            if (Form == null) return null;

            // Find the model type that was used to create this form
            var modelType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t =>
                    t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(p => p.Name.ToLowerInvariant() == controlName.ToLowerInvariant()));

            if (modelType == null) return null;

            // Find the matching property
            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name.ToLowerInvariant() == controlName.ToLowerInvariant());
        }

        private string MakeReadableLabel(string name)
        {
            // Convert camelCase or snake_case to a readable label
            if (string.IsNullOrEmpty(name)) return string.Empty;

            // First, handle snake_case
            name = name.Replace("_", " ");

            // Then handle camelCase by inserting spaces before capital letters
            var result = new System.Text.StringBuilder(name.Length * 2);
            result.Append(char.ToUpper(name[0]));

            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    result.Append(' ');
                }
                result.Append(name[i]);
            }

            return result.ToString();
        }

        private void UpdateFormStatus()
        {
            FormStatusText = IsFormValid ?
                (IsFormDirty ? "Modified - Ready to save" : "No changes") :
                "Please correct highlighted fields before saving";
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Form != null)
            {
                Form.MarkAllAsTouched();

                if (Form.IsValid)
                {
                    FormSubmitted?.Invoke();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FormCancelled?.Invoke();
        }
    }
}