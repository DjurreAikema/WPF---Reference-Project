namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class StringLengthAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly int _maxLength;

    public StringLengthAttribute(int maxLength, int minLength = 0, string errorMessage = null)
        : base(errorMessage ?? $"String length must be between {minLength} and {maxLength}")
    {
        _minLength = minLength;
        _maxLength = maxLength;
    }

    public override bool Validate(object value)
    {
        if (value == null)
            return _minLength == 0; // Null is valid if min length is 0

        if (value is string stringValue)
            return stringValue.Length >= _minLength && stringValue.Length <= _maxLength;

        return true;
    }
}