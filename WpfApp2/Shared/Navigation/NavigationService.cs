using System.Windows.Controls;

namespace WpfApp2.Shared.Navigation;

public class NavigationService
{
    private readonly ContentControl _contentControl;
    private readonly Stack<UserControl> _navigationStack = new();

    public NavigationService(ContentControl contentControl)
    {
        _contentControl = contentControl;
    }

    public void NavigateTo(UserControl view)
    {
        _contentControl.Content = view;
        _navigationStack.Push(view);
    }

    public void NavigateBack()
    {
        if (_navigationStack.Count <= 1) return;

        _navigationStack.Pop(); // Remove current
        var previous = _navigationStack.Peek();
        _contentControl.Content = previous;
    }
}