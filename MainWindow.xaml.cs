using System.Windows;
using WpfApp1.Windows.WindowFive;
using WpfApp1.Windows.WindowFour;
using WpfApp1.Windows.WindowOne;
using WpfApp1.Windows.WindowSix;
using WpfApp1.Windows.WindowThree;
using WpfApp1.Windows.WindowTwo;

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

    private void OpenWindowTwoButton_Click(object sender, RoutedEventArgs e)
    {
        var windowTwo = new WindowTwo();
        windowTwo.Show();
    }

    private void OpenWindowThreeButton_Click(object sender, RoutedEventArgs e)
    {
        var windowThree = new WindowThree();
        windowThree.Show();
    }

    private void OpenWindowFourButton_Click(object sender, RoutedEventArgs e)
    {
        var windowFour = new WindowFour();
        windowFour.Show();
    }

    private void OpenWindowFiveButton_Click(object sender, RoutedEventArgs e)
    {
        var windowFive = new WindowFive();
        windowFive.Show();
    }

    private void OpenWindowSixButton_Click(object sender, RoutedEventArgs e)
    {
        var windowSix = new WindowSix();
        windowSix.Show();
    }
}