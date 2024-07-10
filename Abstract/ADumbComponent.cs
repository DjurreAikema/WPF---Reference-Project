using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Abstract;

public abstract class ADumbComponent : UserControl, INotifyPropertyChanged
{
    // --- Dealing with disposables
    protected readonly CompositeDisposable Disposables = new();

    public static readonly DependencyProperty TriggerDisposeProperty = DependencyProperty.Register(
        nameof(TriggerDispose), typeof(Subject<bool>), typeof(ADumbComponent),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not ADumbComponent c) return;
            c.Disposables.Add(c.TriggerDispose.Subscribe(trigger =>
            {
                if (trigger) c.Disposables.Dispose();
            }));
        }));

    public Subject<bool> TriggerDispose
    {
        get => (Subject<bool>) GetValue(TriggerDisposeProperty);
        set => SetValue(TriggerDisposeProperty, value);
    }

    // --- OnPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}