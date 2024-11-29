using System.Windows.Input;

namespace Record_Book_MVVM.Commands;

public class RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private Action<object> _Execute { get; set; } = executeMethod;
    private Predicate<object> _CanExecute { get; set; } = canExecuteMethod;

    public bool CanExecute(object? parameter)
    {
        return _CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _Execute(parameter);
    }
}