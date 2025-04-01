using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp2.Shared.UI.Navigation;

public partial class SidebarButton
{
    // Icon
    public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(
        nameof(IconData), typeof(Geometry), typeof(SidebarButton),
        new PropertyMetadata(null));

    public Geometry IconData
    {
        get => (Geometry) GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    // Text
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(SidebarButton),
        new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // Command
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(SidebarButton),
        new PropertyMetadata(null));

    public ICommand Command
    {
        get => (ICommand) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    // CommandParameter
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter), typeof(object), typeof(SidebarButton),
        new PropertyMetadata(null));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    // IsExpanded
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(SidebarButton),
        new PropertyMetadata(true));

    public bool IsExpanded
    {
        get => (bool) GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    // Background
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background), typeof(Brush), typeof(SidebarButton),
        new PropertyMetadata(Brushes.Transparent));

    public Brush Background
    {
        get => (Brush) GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    // IsActive
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(SidebarButton),
        new PropertyMetadata(false, OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool) GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    // Click event
    public event RoutedEventHandler Click;

    public SidebarButton()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // Execute command if available
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }

        // Raise Click event
        Click?.Invoke(this, e);
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SidebarButton button)
        {
            if ((bool) e.NewValue)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            }
        }
    }
}