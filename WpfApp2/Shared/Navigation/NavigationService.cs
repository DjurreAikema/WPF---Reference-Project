using System.Windows.Controls;

namespace WpfApp2.Shared.Navigation;

public class NavigationService
{
    // --- Properties
    private readonly ContentControl _contentControl;
    private readonly Stack<UserControl> _navigationStack = new();

    public IReadOnlyCollection<UserControl> NavigationStack => _navigationStack.ToArray();

    // --- Events
    public event EventHandler? NavigationChanged;

    // --- Constructor
    public NavigationService(ContentControl contentControl)
    {
        _contentControl = contentControl;
    }

    // --- Methods
    public void NavigateTo(UserControl view)
    {
        _contentControl.Content = view;
        _navigationStack.Push(view);

        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NavigateBack()
    {
        if (_navigationStack.Count <= 1) return;

        _navigationStack.Pop();
        var previous = _navigationStack.Peek();
        _contentControl.Content = previous;

        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerable<string> GetNavigationPath()
    {
        return _navigationStack
            .Reverse() // Reverse to get chronological order
            .Select(GetViewName);
    }

    private string GetViewName(UserControl view)
    {
        // Try to get a display name from the view
        var type = view.GetType();

        // Remove the "View" suffix if present
        var name = type.Name;
        if (name.EndsWith("View"))
            name = name.Substring(0, name.Length - 4);

        // Add spaces before capital letters for better readability
        var result = string.Empty;
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                result += " ";
            result += name[i];
        }

        return result;
    }
}