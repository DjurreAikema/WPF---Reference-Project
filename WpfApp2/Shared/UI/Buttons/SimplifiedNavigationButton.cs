using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp2.Shared.UI.Buttons;

public class SimplifiedNavigationButton : Grid
{
    private Button _mainButton = null!;
    private Button _popupButton = null!;
    private StackPanel _contentPanel = null!;
    private Path _iconPath = null!;
    private TextBlock _textBlock = null!;

    // Event handlers
    public event RoutedEventHandler MainButtonClick = null!;
    public event RoutedEventHandler PopupButtonClick = null!;

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
        if (d is not SimplifiedNavigationButton button) return;
        if (e.NewValue is string text)
            button._textBlock.Text = text;
        else
            button._textBlock.Text = e.NewValue?.ToString() ?? string.Empty;
    }

    // Icon property
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon), typeof(Geometry), typeof(SimplifiedNavigationButton),
        new PropertyMetadata(null, OnIconChanged));

    public Geometry Icon
    {
        get => (Geometry) GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SimplifiedNavigationButton button) return;
        button._iconPath.Data = e.NewValue as Geometry;
        button._iconPath.Visibility = e.NewValue != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public SimplifiedNavigationButton()
    {
        InitializeGrid();
        InitializeContentPanel();
        InitializeMainButton();
        InitializePopupButton();
    }

    private void InitializeGrid()
    {
        // Set up the Grid
        ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
        ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});

        // Use the predefined styles
        Style = (Style) Application.Current.Resources["ManageButtonCompositeStyle"]! ?? throw new InvalidOperationException();
    }

    private void InitializeContentPanel()
    {
        // Create content panel for icon and text
        _contentPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Create icon
        _iconPath = new Path
        {
            Fill = Brushes.White,
            Width = 16,
            Height = 16,
            Stretch = Stretch.Uniform,
            Margin = new Thickness(0, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = Visibility.Collapsed
        };
        _contentPanel.Children.Add(_iconPath);

        // Create text block
        _textBlock = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center
        };
        _contentPanel.Children.Add(_textBlock);
    }

    private void InitializeMainButton()
    {
        // Create main button
        _mainButton = new Button
        {
            Style = (Style) Application.Current.Resources["ManageMainButtonStyle"]! ?? throw new InvalidOperationException(),
            Content = _contentPanel
        };
        _mainButton.Click += (_, e) => MainButtonClick.Invoke(this, e);
        SetColumn(_mainButton, 0);
        Children.Add(_mainButton);
    }

    private void InitializePopupButton()
    {
        // Create popup button
        _popupButton = new Button
        {
            Style = (Style) Application.Current.Resources["ManagePopupButtonStyle"]! ?? throw new InvalidOperationException()
        };
        _popupButton.Click += (_, e) => PopupButtonClick.Invoke(this, e);
        SetColumn(_popupButton, 1);
        Children.Add(_popupButton);

        // Set tooltip
        ToolTip toolTip = new ToolTip {Content = "Open in new window"};
        _popupButton.ToolTip = toolTip;
    }
}