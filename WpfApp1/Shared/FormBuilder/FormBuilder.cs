using WpfApp1.Shared.FormBuilder.Validators;

namespace WpfApp1.Shared.FormBuilder;

public static class FormBuilder
{
    /// <summary>
    /// Creates a form control with the specified initial value and validators
    /// </summary>
    public static FormField<T> Control<T>(T initialValue = default, params FormValidator<T>[] validators)
    {
        return new FormField<T>(initialValue, validators);
    }

    /// <summary>
    /// Creates a form control for string values
    /// </summary>
    public static FormField<string> TextControl(string initialValue = "", bool required = false, int? minLength = null, int? maxLength = null)
    {
        var validators = new List<FormValidator<string>>();

        if (required)
        {
            validators.Add(new RequiredValidator("This field is required"));
        }

        if (minLength.HasValue && maxLength.HasValue)
        {
            validators.Add(new CustomValidator<string>(
                s => s == null || (s.Length >= minLength.Value && s.Length <= maxLength.Value),
                $"Text must be between {minLength.Value} and {maxLength.Value} characters"));
        }
        else if (minLength.HasValue)
        {
            validators.Add(new CustomValidator<string>(
                s => s == null || s.Length >= minLength.Value,
                $"Text must be at least {minLength.Value} characters"));
        }
        else if (maxLength.HasValue)
        {
            validators.Add(new CustomValidator<string>(
                s => s == null || s.Length <= maxLength.Value,
                $"Text must be at most {maxLength.Value} characters"));
        }

        return new FormField<string>(initialValue, validators);
    }

    /// <summary>
    /// Creates a form control for numeric values
    /// </summary>
    public static FormField<T> NumericControl<T>(T initialValue = default, T? minValue = default, T? maxValue = default) where T : struct, IComparable<T>
    {
        var validators = new List<FormValidator<T>>();

        if (minValue != null)
        {
            validators.Add(new MinValueValidator<T>(minValue.Value));
        }

        if (maxValue != null)
        {
            validators.Add(new MaxValueValidator<T>(maxValue.Value));
        }

        return new FormField<T>(initialValue, validators);
    }

    /// <summary>
    /// Creates a form control for email
    /// </summary>
    public static FormField<string> EmailControl(string initialValue = "", bool required = false)
    {
        var validators = new List<FormValidator<string>>();

        if (required)
        {
            validators.Add(new RequiredValidator("Email is required"));
        }

        // Email validation
        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        validators.Add(new CustomValidator<string>(
            s => s == null || string.IsNullOrEmpty(s) || emailRegex.IsMatch(s),
            "Please enter a valid email address"));

        return new FormField<string>(initialValue, validators);
    }

    /// <summary>
    /// Creates a form group with the specified controls
    /// </summary>
    public static FormGroup Group(Dictionary<string, object> controls)
    {
        return new FormGroup(controls);
    }

    /// <summary>
    /// Creates a form from a model
    /// </summary>
    public static FormGroup FromModel<T>(T model) where T : class
    {
        return FormModelBuilder.FromModel(model);
    }

    /// <summary>
    /// Maps form values back to a model
    /// </summary>
    public static void MapToModel<T>(FormGroup form, T model) where T : class
    {
        FormModelBuilder.MapToModel(form, model);
    }

    /// <summary>
    /// Creates a model from form values
    /// </summary>
    public static T CreateFromForm<T>(FormGroup form) where T : class, new()
    {
        return FormModelBuilder.CreateFromForm<T>(form);
    }
}