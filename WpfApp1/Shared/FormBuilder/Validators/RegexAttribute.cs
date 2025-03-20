namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class RegexAttribute : ValidationAttribute
{
    private readonly System.Text.RegularExpressions.Regex _regex;

    public RegexAttribute(string pattern, string errorMessage = "Invalid format")
        : base(errorMessage)
    {
        _regex = new System.Text.RegularExpressions.Regex(pattern);
    }

    public override bool Validate(object value)
    {
        if (value == null || value is not string stringValue)
            return true; // Null values are handled by Required validators

        return _regex.IsMatch(stringValue);
    }
}