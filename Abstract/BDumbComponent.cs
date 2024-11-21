using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Abstract;

public abstract class BDumbComponent : UserControl, INotifyPropertyChanged
{
    // --- Dealing with disposables
    protected readonly CompositeDisposable Disposables = new();

    protected BDumbComponent()
    {
        // Subscribe to the Unloaded event
        Unloaded += OnComponentUnloaded;
    }

    private void OnComponentUnloaded(object sender, RoutedEventArgs e)
    {
        Disposables.Dispose();
    }

    // --- OnPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}