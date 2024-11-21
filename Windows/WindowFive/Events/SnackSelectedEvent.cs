using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFive.Events;

public class SnackSelectedEventArgs(Snack selectedSnack) : EventArgs
{
    public Snack SelectedSnack { get; } = selectedSnack;
}