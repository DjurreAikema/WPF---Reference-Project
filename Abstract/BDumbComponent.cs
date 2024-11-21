using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Abstract;

public abstract class BDumbComponent : UserControl, INotifyPropertyChanged
{
    // --- Dealing with disposables
    protected readonly CompositeDisposable Disposables = new();

    // protected override void OnUnloaded(RoutedEventArgs e)
    // {
        // base.OnUnloaded(e);
        // Disposables.Dispose();
    // }

    // --- OnPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}