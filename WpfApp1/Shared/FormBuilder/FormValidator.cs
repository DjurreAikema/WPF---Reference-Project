namespace WpfApp1.Shared.FormBuilder;

public abstract class FormValidator<T>
{
    public abstract bool Validate(T value, out string errorMessage);
}