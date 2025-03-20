namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class ValidationAttribute : Attribute
{
    public string ErrorMessage { get; set; }

    protected ValidationAttribute(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public abstract bool Validate(object value);
}