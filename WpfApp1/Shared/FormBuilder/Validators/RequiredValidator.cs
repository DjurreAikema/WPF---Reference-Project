namespace WpfApp1.Shared.FormBuilder.Validators;

public class RequiredValidator : FormValidator<string>
{
    private readonly string _errorMessage;

    public RequiredValidator(string errorMessage = "This field is required")
    {
        _errorMessage = errorMessage;
    }

    public override bool Validate(string value, out string errorMessage)
    {
        bool isValid = !string.IsNullOrWhiteSpace(value);
        errorMessage = isValid ? string.Empty : _errorMessage;
        return isValid;
    }
}