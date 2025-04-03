using System.Windows;
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

    // IsExpanded
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(SidebarButton),
        new PropertyMetadata(true));

    public bool IsExpanded
    {
        get => (bool) GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    // IsActive
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(SidebarButton),
        new PropertyMetadata(false));

    public bool IsActive
    {
        get => (bool) GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    // Click event
    public event RoutedEventHandler? Click;

    public SidebarButton()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Click?.Invoke(this, e);
}