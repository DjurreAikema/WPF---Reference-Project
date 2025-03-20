namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : ValidationAttribute
{
    public RequiredAttribute(string errorMessage = "This field is required")
        : base(errorMessage)
    {
    }

    public override bool Validate(object value)
    {
        if (value == null)
            return false;

        if (value is string stringValue)
            return !string.IsNullOrWhiteSpace(stringValue);

        return true;
    }
}