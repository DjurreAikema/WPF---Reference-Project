using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.Shared.Abstract;

[ObservableObject]
public abstract partial class BDumbComponent : UserControl
{
    // --- Dealing with disposables
    protected readonly CompositeDisposable Disposables = new();

    public static readonly DependencyProperty TriggerDisposeProperty = DependencyProperty.Register(
        nameof(TriggerDispose), typeof(Subject<bool>), typeof(BDumbComponent),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not BDumbComponent c) return;
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
}