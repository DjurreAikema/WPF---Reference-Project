namespace WpfApp1.Shared.FormBuilder;

[AttributeUsage(AttributeTargets.Property)]
public class FormPropertyAttribute : Attribute
{
    public string Label { get; }
    public int Order { get; }
    public string Description { get; }
    public bool Excluded { get; }

    /// <summary>
    /// Marks a property to be included in a form with custom settings
    /// </summary>
    /// <param name="label">Custom label for the form field (if null, property name will be used)</param>
    /// <param name="order">Display order in the form (lower numbers come first)</param>
    /// <param name="description">Optional help text to display with the field</param>
    public FormPropertyAttribute(string? label = null, int order = 999, string? description = null)
    {
        Label = label ?? string.Empty;
        Order = order;
        Description = description ?? string.Empty;
        Excluded = false;
    }

    /// <summary>
    /// Exclude this property from the form
    /// </summary>
    /// <param name="excluded">Set to true to exclude this property</param>
    public FormPropertyAttribute(bool excluded)
    {
        Label = string.Empty;
        Order = 999;
        Description = string.Empty;
        Excluded = excluded;
    }
}

// Helper attribute that excludes a property from the form
[AttributeUsage(AttributeTargets.Property)]
public class FormExcludeAttribute : FormPropertyAttribute
{
    public FormExcludeAttribute() : base(true)
    {
    }
}