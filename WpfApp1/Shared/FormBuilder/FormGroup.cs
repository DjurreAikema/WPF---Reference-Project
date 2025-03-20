using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WpfApp1.Shared.FormBuilder;

public class FormGroup : IDisposable
{
    private readonly Dictionary<string, object> _fields = new();
    private readonly BehaviorSubject<bool> _valid = new(true);
    private readonly BehaviorSubject<bool> _dirty = new(false);
    private readonly BehaviorSubject<bool> _touched = new(false);
    private readonly CompositeDisposable _disposables = new();

    public FormGroup(Dictionary<string, object> fields)
    {
        foreach (var (key, field) in fields)
        {
            _fields[key] = field;

            // Set up subscriptions to track overall form state
            SetupFieldSubscriptions(key, field);
        }

        UpdateValidity();
    }

    // Observable properties
    public IObservable<bool> Valid => _valid.AsObservable();
    public IObservable<bool> Dirty => _dirty.AsObservable();
    public IObservable<bool> Touched => _touched.AsObservable();

    // Current state properties
    public bool IsValid => _valid.Value;
    public bool IsDirty => _dirty.Value;
    public bool IsTouched => _touched.Value;

    // Get all control names
    public IEnumerable<string> ControlNames => _fields.Keys;

    // Methods
    public FormField<T> GetControl<T>(string name)
    {
        name = name.ToLowerInvariant();
        if (!_fields.ContainsKey(name))
        {
            throw new KeyNotFoundException($"Form control '{name}' not found.");
        }

        return (FormField<T>) _fields[name];
    }

    public object GetControlDynamic(string name)
    {
        name = name.ToLowerInvariant();
        if (!_fields.ContainsKey(name))
        {
            throw new KeyNotFoundException($"Form control '{name}' not found.");
        }

        return _fields[name];
    }

    public bool HasControl(string name)
    {
        name = name.ToLowerInvariant();
        return _fields.ContainsKey(name);
    }

    public T GetValue<T>(string name)
    {
        name = name.ToLowerInvariant();
        var control = GetControl<T>(name);
        return control.CurrentValue;
    }

    public object GetValueDynamic(string name)
    {
        name = name.ToLowerInvariant();
        var control = GetControlDynamic(name);

        // Use reflection to get the CurrentValue property
        var valueProperty = control.GetType().GetProperty("CurrentValue");
        return valueProperty?.GetValue(control);
    }

    public void SetValue<T>(string name, T value)
    {
        name = name.ToLowerInvariant();
        var control = GetControl<T>(name);
        control.SetValue(value);
    }

    public void SetValueDynamic(string name, object value)
    {
        name = name.ToLowerInvariant();
        var control = GetControlDynamic(name);

        // Use reflection to call the SetValue method
        var setValue = control.GetType().GetMethod("SetValue");
        setValue?.Invoke(control, new[] {value});
    }

    public Dictionary<string, object> GetValues()
    {
        var result = new Dictionary<string, object>();

        foreach (var (key, field) in _fields)
        {
            // Use reflection to get the CurrentValue property
            var valueProperty = field.GetType().GetProperty("CurrentValue");
            result[key] = valueProperty?.GetValue(field);
        }

        return result;
    }

    public void Reset()
{
    foreach (var field in _fields.Values)
    {
        try
        {
            // Get the field's type
            var fieldType = field.GetType();

            // Get the Reset method with its parameter information
            var resetMethod = fieldType.GetMethod("Reset");

            if (resetMethod != null)
            {
                // Get parameters info
                var parameters = resetMethod.GetParameters();

                if (parameters.Length == 0)
                {
                    // No parameters needed
                    resetMethod.Invoke(field, null);
                }
                else
                {
                    // Create default parameter values for optional parameters
                    object[] paramValues = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        // For optional parameters or parameters with default values,
                        // we use Type.Missing or default values
                        if (parameters[i].IsOptional || parameters[i].HasDefaultValue)
                        {
                            paramValues[i] = parameters[i].DefaultValue;
                        }
                        else
                        {
                            // For non-optional parameters, provide a default value based on type
                            paramValues[i] = parameters[i].ParameterType.IsValueType ?
                                Activator.CreateInstance(parameters[i].ParameterType) : null;
                        }
                    }

                    // Invoke with parameters
                    resetMethod.Invoke(field, paramValues);
                }
            }
        }
        catch (Exception ex)
        {
            // Log but don't crash on Reset errors
            Console.WriteLine($"Error resetting field: {ex.Message}");
        }
    }
}

    public void MarkAllAsTouched()
    {
        foreach (var field in _fields.Values)
        {
            // Use reflection to call MarkAsTouched()
            var touchMethod = field.GetType().GetMethod("MarkAsTouched");
            touchMethod?.Invoke(field, null);
        }
    }

    private void SetupFieldSubscriptions(string key, object field)
    {
        // Get observable properties using reflection
        var validProperty = field.GetType().GetProperty("Valid");
        var dirtyProperty = field.GetType().GetProperty("Dirty");
        var touchedProperty = field.GetType().GetProperty("Touched");

        if (validProperty?.GetValue(field) is IObservable<bool> validObs)
        {
            _disposables.Add(validObs.Subscribe(_ => UpdateValidity()));
        }

        if (dirtyProperty?.GetValue(field) is IObservable<bool> dirtyObs)
        {
            _disposables.Add(dirtyObs.Subscribe(_ => UpdateDirty()));
        }

        if (touchedProperty?.GetValue(field) is IObservable<bool> touchedObs)
        {
            _disposables.Add(touchedObs.Subscribe(_ => UpdateTouched()));
        }
    }

    private void UpdateValidity()
    {
        bool isValid = true;

        foreach (var field in _fields.Values)
        {
            var validProperty = field.GetType().GetProperty("IsValid");
            if (validProperty?.GetValue(field) is bool fieldValid && !fieldValid)
            {
                isValid = false;
                break;
            }
        }

        _valid.OnNext(isValid);
    }

    private void UpdateDirty()
    {
        bool isDirty = false;

        foreach (var field in _fields.Values)
        {
            var dirtyProperty = field.GetType().GetProperty("IsDirty");
            if (dirtyProperty?.GetValue(field) is bool fieldDirty && fieldDirty)
            {
                isDirty = true;
                break;
            }
        }

        _dirty.OnNext(isDirty);
    }

    private void UpdateTouched()
    {
        bool isTouched = false;

        foreach (var field in _fields.Values)
        {
            var touchedProperty = field.GetType().GetProperty("IsTouched");
            if (touchedProperty?.GetValue(field) is bool fieldTouched && fieldTouched)
            {
                isTouched = true;
                break;
            }
        }

        _touched.OnNext(isTouched);
    }

    public void Dispose()
    {
        _disposables.Dispose();

        foreach (var field in _fields.Values)
        {
            if (field is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}