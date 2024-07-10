using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace WpfApp1.Abstract;

public abstract class ADumbComponent : UserControl, INotifyPropertyChanged
{
    protected readonly CompositeDisposable Disposables = new();

    protected void Dispose()
    {
        Disposables.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}