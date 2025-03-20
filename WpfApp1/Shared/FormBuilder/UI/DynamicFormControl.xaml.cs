using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.Shared.FormBuilder.UI
{
    public class FormFieldInfo
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public object Control { get; set; }
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
            new PropertyMetadata("Submit"));

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

                // Create a user-friendly label from the control name
                var label = MakeReadableLabel(controlName);

                FormFields.Add(new FormFieldInfo
                {
                    Name = controlName,
                    Label = label,
                    Control = control
                });
            }
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
                (IsFormDirty ? "Modified - Ready to submit" : "No changes") :
                "Please correct errors before submitting";
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