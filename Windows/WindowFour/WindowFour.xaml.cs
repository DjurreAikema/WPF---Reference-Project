using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour;

public partial class WindowFour : INotifyPropertyChanged
{
    public WindowFourViewModel ViewModel { get; } = new();
    public CompositeDisposable Disposables = new();

    // --- Constructor
    public WindowFour()
    {
        InitializeComponent();

        // Dispose of all subscriptions when the window is closed
        Closing += (_, _) =>
        {
            ViewModel.Dispose();
            Disposables.Dispose();
        };
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChangedS.OnNext(snack);
    }

    // --- On Property Changed
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}