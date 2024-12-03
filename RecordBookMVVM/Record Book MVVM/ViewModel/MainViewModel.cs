using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Record_Book_MVVM.Commands;
using Record_Book_MVVM.Models;
using Record_Book_MVVM.Views;

namespace Record_Book_MVVM.ViewModel;

public class MainViewModel
{
    public ObservableCollection<User> Users { get; set; }
    public ICommand ShowWindowCommand { get; set; }

    public MainViewModel()
    {
        Users = UserManager.GetUsers();
        ShowWindowCommand = new RelayCommand(ShowWindow, CanShowWindow);
    }

    private static bool CanShowWindow(object obj) => true;

    private static void ShowWindow(object obj)
    {
        var mainWindow = obj as Window;

        var addUserWin = new AddUser
        {
            Owner = mainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        addUserWin.Show();
    }
}