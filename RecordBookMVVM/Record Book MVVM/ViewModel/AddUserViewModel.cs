using System.Windows.Input;
using Record_Book_MVVM.Commands;
using Record_Book_MVVM.Models;

namespace Record_Book_MVVM.ViewModel;

public class AddUserViewModel
{
    public ICommand AddUserCommand { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public AddUserViewModel()
    {
        AddUserCommand = new RelayCommand(AddUser, CanAddUser);
    }

    private static bool CanAddUser(object obj) => true;

    private void AddUser(object obj)
    {
        UserManager.AddUser(new User {Name = Name, Email = Email});
    }
}