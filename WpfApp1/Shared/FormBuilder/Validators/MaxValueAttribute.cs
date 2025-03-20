namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class MaxValueAttribute : ValidationAttribute
{
    private readonly double _maxValue;

    public MaxValueAttribute(double maxValue, string errorMessage = null)
        : base(errorMessage ?? $"Value must be at most {maxValue}")
    {
        _maxValue = maxValue;
    }

    public override bool Validate(object value)
    {
        if (value == null)
            return true; // Null values are handled by Required validators

        if (value is IComparable comparable)
            return comparable.CompareTo(Convert.ChangeType(_maxValue, value.GetType())) <= 0;

        return true;
    }
}