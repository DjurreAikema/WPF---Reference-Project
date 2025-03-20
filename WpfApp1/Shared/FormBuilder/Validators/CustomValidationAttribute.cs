namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class CustomValidationAttribute : ValidationAttribute
{
    private readonly string _validationMethodName;

    public CustomValidationAttribute(string validationMethodName, string errorMessage)
        : base(errorMessage)
    {
        _validationMethodName = validationMethodName;
    }

    public override bool Validate(object value)
    {
        // This will be handled separately in the FormModelBuilder
        // because it requires the model instance for validation context
        return true;
    }

    public string ValidationMethodName => _validationMethodName;
}