using System.Windows;
using Record_Book_MVVM.ViewModel;

namespace Record_Book_MVVM.Views;

public partial class AddUser : Window
{
    public AddUser()
    {
        InitializeComponent();
        var addViewModel = new AddUserViewModel();
        DataContext = addViewModel;
    }
}