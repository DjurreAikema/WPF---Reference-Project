using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WpfApp1.Shared.FormBuilder;

public class FormField<T> : IDisposable
{
    private readonly BehaviorSubject<T> _value;
    private readonly BehaviorSubject<bool> _valid = new(true);
    private readonly BehaviorSubject<bool> _dirty = new(false);
    private readonly BehaviorSubject<bool> _touched = new(false);
    private readonly BehaviorSubject<string> _error = new(string.Empty);
    private readonly List<FormValidator<T>> _validators = new();
    private readonly CompositeDisposable _disposables = new();

    public FormField(T initialValue = default, IEnumerable<FormValidator<T>> validators = null)
    {
        _value = new BehaviorSubject<T>(initialValue);

        if (validators != null)
        {
            _validators.AddRange(validators);
        }

        // Validate when value changes
        _disposables.Add(_value.Subscribe(_ => Validate()));

        // Initial validation
        Validate();
    }

    // Observable properties
    public IObservable<T> Value => _value.AsObservable();
    public IObservable<bool> Valid => _valid.AsObservable();
    public IObservable<bool> Dirty => _dirty.AsObservable();
    public IObservable<bool> Touched => _touched.AsObservable();
    public IObservable<string> Error => _error.AsObservable();

    // Current value properties (convenient for binding)
    public T CurrentValue => _value.Value;
    public bool IsValid => _valid.Value;
    public bool IsDirty => _dirty.Value;
    public bool IsTouched => _touched.Value;
    public string CurrentError => _error.Value;

    // Methods
    public void SetValue(T value)
    {
        if (!EqualityComparer<T>.Default.Equals(_value.Value, value))
        {
            _dirty.OnNext(true);
            _value.OnNext(value);
        }
    }

    public void MarkAsTouched()
    {
        _touched.OnNext(true);
    }

    public void Reset(T value = default)
    {
        _value.OnNext(value);
        _dirty.OnNext(false);
        _touched.OnNext(false);
    }

    public void Validate()
    {
        string errorMessage = string.Empty;
        bool isValid = true;

        foreach (var validator in _validators)
        {
            if (!validator.Validate(_value.Value, out var error))
            {
                errorMessage = error;
                isValid = false;
                break;
            }
        }

        _valid.OnNext(isValid);
        _error.OnNext(errorMessage);
    }

    public void AddValidator(FormValidator<T> validator)
    {
        _validators.Add(validator);
        Validate();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}