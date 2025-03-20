namespace WpfApp1.Shared.FormBuilder.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class EmailAttribute : ValidationAttribute
{
    private static readonly System.Text.RegularExpressions.Regex EmailRegex =
        new System.Text.RegularExpressions.Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    public EmailAttribute(string errorMessage = "Invalid email address")
        : base(errorMessage)
    {
    }

    public override bool Validate(object value)
    {
        if (value == null || value is not string stringValue)
            return true; // Null values are handled by Required validators

        return EmailRegex.IsMatch(stringValue);
    }
}