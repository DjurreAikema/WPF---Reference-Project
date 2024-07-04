using System.Windows;

namespace WpfApp1.Windows.WindowOne;

public partial class WindowOne : Window
{
    public string HeaderText { get; set; } = "Default Header";

    public WindowOne()
    {
        InitializeComponent();
        DataContext = this;
    }
}