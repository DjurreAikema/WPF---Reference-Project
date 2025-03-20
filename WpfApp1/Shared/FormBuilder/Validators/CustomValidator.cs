namespace WpfApp1.Shared.FormBuilder.Validators;

public class CustomValidator<T> : FormValidator<T>
{
    private readonly Func<T, bool> _validationFunc;
    private readonly string _errorMessage;

    public CustomValidator(Func<T, bool> validationFunc, string errorMessage)
    {
        _validationFunc = validationFunc;
        _errorMessage = errorMessage;
    }

    public override bool Validate(T value, out string errorMessage)
    {
        bool isValid = _validationFunc(value);
        errorMessage = isValid ? string.Empty : _errorMessage;
        return isValid;
    }
}