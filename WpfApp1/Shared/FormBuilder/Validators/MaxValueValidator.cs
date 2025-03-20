namespace WpfApp1.Shared.FormBuilder.Validators;

public class MaxValueValidator<T> : FormValidator<T> where T : IComparable<T>
{
    private readonly T _maxValue;
    private readonly string _errorMessage;

    public MaxValueValidator(T maxValue, string errorMessage = null)
    {
        _maxValue = maxValue;
        _errorMessage = errorMessage ?? $"Value must be at most {maxValue}";
    }

    public override bool Validate(T value, out string errorMessage)
    {
        bool isValid = value.CompareTo(_maxValue) <= 0;
        errorMessage = isValid ? string.Empty : _errorMessage;
        return isValid;
    }
}