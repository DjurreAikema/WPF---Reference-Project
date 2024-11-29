using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window5.Events;

public class SnackSelectedEventArgs(Snack selectedSnack) : EventArgs
{
    public Snack SelectedSnack { get; } = selectedSnack;
}