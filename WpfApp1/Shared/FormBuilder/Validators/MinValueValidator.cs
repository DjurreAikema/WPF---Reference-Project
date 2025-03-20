namespace WpfApp1.Shared.FormBuilder.Validators;

public class MinValueValidator<T> : FormValidator<T> where T : IComparable<T>
{
    private readonly T _minValue;
    private readonly string _errorMessage;

    public MinValueValidator(T minValue, string errorMessage = null)
    {
        _minValue = minValue;
        _errorMessage = errorMessage ?? $"Value must be at least {minValue}";
    }

    public override bool Validate(T value, out string errorMessage)
    {
        bool isValid = value.CompareTo(_minValue) >= 0;
        errorMessage = isValid ? string.Empty : _errorMessage;
        return isValid;
    }
}