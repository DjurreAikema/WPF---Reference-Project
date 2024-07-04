using System.Windows;
using WpfApp1.Windows;
using WpfApp1.Windows.WindowOne;

namespace WpfApp1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenWindowOneButton_Click(object sender, RoutedEventArgs e)
    {
        var windowOne = new WindowOne();
        windowOne.Show();
    }
}