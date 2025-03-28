using System.Windows;
using System.Windows.Controls;

namespace WpfApp2.Shared.Navigation;

public class WindowFactory
{
    /// <summary>
    /// Opens a UserControl in a new window
    /// </summary>
    /// <param name="content">The UserControl to display in the window</param>
    /// <param name="title">The window title</param>
    /// <param name="width">Optional window width</param>
    /// <param name="height">Optional window height</param>
    public static Window CreateWindow(UserControl content, string title, double width = 800, double height = 600)
    {
        var window = new Window
        {
            Title = title,
            Content = content,
            Width = width,
            Height = height,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Add app icon and other settings if needed
        if (Application.Current.MainWindow != null)
        {
            window.Owner = Application.Current.MainWindow;
            window.Icon = Application.Current.MainWindow.Icon;
        }

        return window;
    }
}