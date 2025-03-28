using System.Windows;
using System.Windows.Input;

namespace WpfApp2.Shared.UI.Buttons;

/// <summary>
/// Interaction logic for CompositeButton.xaml
/// </summary>
public partial class CompositeNavigationButton
{
    // --- Dependency properties
    public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(
        nameof(ButtonContent), typeof(object), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public object ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    public static readonly DependencyProperty MainCommandProperty = DependencyProperty.Register(
        nameof(MainCommand), typeof(ICommand), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public ICommand MainCommand
    {
        get => (ICommand) GetValue(MainCommandProperty);
        set => SetValue(MainCommandProperty, value);
    }

    public static readonly DependencyProperty PopupCommandProperty = DependencyProperty.Register(
        nameof(PopupCommand), typeof(ICommand), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public ICommand PopupCommand
    {
        get => (ICommand) GetValue(PopupCommandProperty);
        set => SetValue(PopupCommandProperty, value);
    }

    public static readonly DependencyProperty PopupTooltipProperty = DependencyProperty.Register(
        nameof(PopupTooltip), typeof(string), typeof(CompositeNavigationButton),
        new PropertyMetadata("Open in new window"));

    public string PopupTooltip
    {
        get => (string) GetValue(PopupTooltipProperty);
        set => SetValue(PopupTooltipProperty, value);
    }

    public static readonly DependencyProperty GridStyleProperty = DependencyProperty.Register(
        nameof(GridStyle), typeof(Style), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public Style GridStyle
    {
        get => (Style) GetValue(GridStyleProperty);
        set => SetValue(GridStyleProperty, value);
    }

    public static readonly DependencyProperty MainButtonStyleProperty = DependencyProperty.Register(
        nameof(MainButtonStyle), typeof(Style), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public Style MainButtonStyle
    {
        get => (Style) GetValue(MainButtonStyleProperty);
        set => SetValue(MainButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PopupButtonStyleProperty = DependencyProperty.Register(
        nameof(PopupButtonStyle), typeof(Style), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public Style PopupButtonStyle
    {
        get => (Style) GetValue(PopupButtonStyleProperty);
        set => SetValue(PopupButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter), typeof(object), typeof(CompositeNavigationButton),
        new PropertyMetadata(null));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    // --- Events
    public event RoutedEventHandler MainButtonClick;
    public event RoutedEventHandler PopupButtonClick;

    // --- Constructor
    public CompositeNavigationButton()
    {
        InitializeComponent();
    }

    private void MainButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainCommand != null && MainCommand.CanExecute(CommandParameter))
            MainCommand.Execute(CommandParameter);

        MainButtonClick.Invoke(this, e);
    }

    private void PopupButton_Click(object sender, RoutedEventArgs e)
    {
        if (PopupCommand != null && PopupCommand.CanExecute(CommandParameter))
            PopupCommand.Execute(CommandParameter);

        PopupButtonClick?.Invoke(this, e);
    }
}