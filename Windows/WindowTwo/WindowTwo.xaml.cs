using System.Windows;

namespace WpfApp1.Windows.WindowTwo;

public partial class WindowTwo : Window
{
    public WindowTwo()
    {
        InitializeComponent();
        DataContext = new WindowTwoViewModel();
    }
}