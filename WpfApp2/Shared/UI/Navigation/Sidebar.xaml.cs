using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp2.Shared.UI.Navigation;

public partial class Sidebar
{
    private bool _isExpanded = true;
    private readonly Storyboard _expandStoryboard;
    private readonly Storyboard _collapseStoryboard;

    // IsExpanded property
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(Sidebar),
        new PropertyMetadata(true, OnIsExpandedChanged));

    public bool IsExpanded
    {
        get => (bool) GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    // SidebarItems collection
    public static readonly DependencyProperty SidebarItemsProperty = DependencyProperty.Register(
        nameof(SidebarItems), typeof(ObservableCollection<SidebarItem>), typeof(Sidebar),
        new PropertyMetadata(null, OnSidebarItemsChanged));

    public ObservableCollection<SidebarItem> SidebarItems
    {
        get => (ObservableCollection<SidebarItem>) GetValue(SidebarItemsProperty);
        set => SetValue(SidebarItemsProperty, value);
    }

    // Selected item
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem), typeof(string), typeof(Sidebar),
        new PropertyMetadata(null, OnSelectedItemChanged));

    public string SelectedItem
    {
        get => (string) GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // NavigationRequested event
    public event NavigationEventHandler NavigationRequested;

    public delegate void NavigationEventHandler(object sender, string destination);

    public Sidebar()
    {
        InitializeComponent();

        // Get animation storyboards from resources
        _expandStoryboard = (Storyboard) FindResource("ExpandStoryboard");
        _collapseStoryboard = (Storyboard) FindResource("CollapseStoryboard");

        // Initialize items collection
        SidebarItems = [];
    }

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Sidebar sidebar)
        {
            sidebar.UpdateExpandedState();
        }
    }

    private static void OnSidebarItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Sidebar sidebar)
        {
            sidebar.RebuildSidebarButtons();
        }
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Sidebar sidebar)
        {
            sidebar.UpdateSelectedButton();
        }
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    private void UpdateExpandedState()
    {
        _isExpanded = IsExpanded;

        // Update toggle button text
        ToggleText.Text = IsExpanded ? "Collapse" : "Expand";

        // Update toggle button icon (rotate the arrow)
        var rotateTransform = new RotateTransform();
        ToggleIcon.RenderTransform = rotateTransform;
        ToggleIcon.RenderTransformOrigin = new Point(0.5, 0.5);

        if (IsExpanded)
        {
            _expandStoryboard.Begin();
            rotateTransform.Angle = 0;
        }
        else
        {
            _collapseStoryboard.Begin();
            rotateTransform.Angle = 180;
        }

        // Update all sidebar buttons
        foreach (var child in ButtonsContainer.Children)
        {
            if (child is SidebarButton button)
            {
                button.IsExpanded = IsExpanded;
            }
        }
    }

    private void RebuildSidebarButtons()
    {
        // Clear existing buttons
        ButtonsContainer.Children.Clear();

        if (SidebarItems == null) return;

        // Add buttons for each item
        foreach (var item in SidebarItems)
        {
            var button = new SidebarButton
            {
                Text = item.Text,
                IconData = item.IconData,
                IsExpanded = IsExpanded,
                Tag = item.Destination
            };

            // Set active state based on selected item
            if (SelectedItem != null && SelectedItem == item.Destination)
            {
                button.IsActive = true;
            }

            // Wire up click event
            button.Click += SidebarButton_Click;

            ButtonsContainer.Children.Add(button);
        }
    }

    private void UpdateSelectedButton()
    {
        foreach (var child in ButtonsContainer.Children)
        {
            if (child is SidebarButton button)
            {
                button.IsActive = (string) button.Tag == SelectedItem;
            }
        }
    }

    private void SidebarButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is SidebarButton button && button.Tag is string destination)
        {
            // Update selected item
            SelectedItem = destination;

            // Raise navigation event
            NavigationRequested?.Invoke(this, destination);
        }
    }
}

// Class to define sidebar items
public class SidebarItem
{
    public string Text { get; set; }
    public Geometry IconData { get; set; }
    public string Destination { get; set; }
}