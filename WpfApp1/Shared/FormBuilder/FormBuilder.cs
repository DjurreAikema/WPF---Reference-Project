namespace WpfApp1.Shared.FormBuilder;

public static class FormBuilder
{
    public static FormField<T> Control<T>(T initialValue = default, params FormValidator<T>[] validators)
    {
        return new FormField<T>(initialValue, validators);
    }

    public static FormGroup Group(Dictionary<string, object> controls)
    {
        return new FormGroup(controls);
    }
}