using System.Windows;
using System.Windows.Controls;

namespace WpfApp2.Shared.UI.Buttons;

public class SimplifiedNavigationButton : Grid
{
    private readonly Button _mainButton;
    private readonly Button _popupButton;

    // Event handlers
    public event RoutedEventHandler MainButtonClick;
    public event RoutedEventHandler PopupButtonClick;

    // Content property for the main button
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content), typeof(object), typeof(SimplifiedNavigationButton),
        new PropertyMetadata(null, OnContentChanged));

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimplifiedNavigationButton button)
        {
            button._mainButton.Content = e.NewValue;
        }
    }

    public SimplifiedNavigationButton()
    {
        // Set up the Grid
        ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
        ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});

        // Use the predefined styles
        Style = (Style) Application.Current.Resources["ManageButtonCompositeStyle"];

        // Create main button
        _mainButton = new Button
        {
            Style = (Style) Application.Current.Resources["ManageMainButtonStyle"]
        };
        _mainButton.Click += (s, e) => MainButtonClick?.Invoke(this, e);
        SetColumn(_mainButton, 0);
        Children.Add(_mainButton);

        // Create popup button
        _popupButton = new Button
        {
            Style = (Style) Application.Current.Resources["ManagePopupButtonStyle"]
        };
        _popupButton.Click += (s, e) => PopupButtonClick?.Invoke(this, e);
        SetColumn(_popupButton, 1);
        Children.Add(_popupButton);

        // Set tooltip
        ToolTip toolTip = new ToolTip {Content = "Open in new window"};
        _popupButton.ToolTip = toolTip;
    }
}