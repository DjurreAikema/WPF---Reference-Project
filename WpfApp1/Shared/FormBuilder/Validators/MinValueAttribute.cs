namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class MinValueAttribute : ValidationAttribute
{
    private readonly double _minValue;

    public MinValueAttribute(double minValue, string errorMessage = null)
        : base(errorMessage ?? $"Value must be at least {minValue}")
    {
        _minValue = minValue;
    }

    public override bool Validate(object value)
    {
        if (value == null)
            return true; // Null values are handled by Required validators

        if (value is IComparable comparable)
            return comparable.CompareTo(Convert.ChangeType(_minValue, value.GetType())) >= 0;

        return true;
    }
}