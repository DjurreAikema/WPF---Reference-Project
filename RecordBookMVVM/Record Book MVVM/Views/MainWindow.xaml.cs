using System.Windows;
using System.Windows.Controls;
using Record_Book_MVVM.ViewModel;

namespace Record_Book_MVVM.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var mainViewModel = new MainViewModel();
        DataContext = mainViewModel;
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        throw new NotImplementedException();
    }
}