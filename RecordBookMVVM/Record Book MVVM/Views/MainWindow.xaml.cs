using System.Windows;
using System.Windows.Controls;
using Record_Book_MVVM.Models;
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
        UserList.Items.Filter = FilterMethod;
    }

    private bool FilterMethod(object obj)
    {
        if (obj is not User user)
            return false;

        return user.Name.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase);
    }
}