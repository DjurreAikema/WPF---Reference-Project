using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp2.Shared.Abstract;

namespace WpfApp2.Shared.UI.Stamdata;

public class StamdataListBase : ADumbComponent
{
    private readonly ListView _listView;
    private readonly Button _newButton;
    private readonly Button _refreshButton;

    // --- Dependency Properties
    public static readonly DependencyProperty ItemsObsProperty = DependencyProperty.Register(
        nameof(ItemsObs), typeof(IObservable<IEnumerable<object>>), typeof(StamdataListBase),
        new PropertyMetadata(null, OnItemsObsChanged));

    public IObservable<IEnumerable<object>> ItemsObs
    {
        get => (IObservable<IEnumerable<object>>) GetValue(ItemsObsProperty);
        set => SetValue(ItemsObsProperty, value);
    }

    private static void OnItemsObsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not StamdataListBase list) return;

        list.Disposables.Add(list.ItemsObs.Subscribe(items => list.Items = new ObservableCollection<object>(items)));
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items), typeof(ObservableCollection<object>), typeof(StamdataListBase),
        new PropertyMetadata(null));

    public ObservableCollection<object>? Items
    {
        get => (ObservableCollection<object>?) GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty AdditionalButtonsProperty = DependencyProperty.Register(
        nameof(AdditionalButtons), typeof(UIElement), typeof(StamdataListBase),
        new PropertyMetadata(null));

    public UIElement? AdditionalButtons
    {
        get => (UIElement?) GetValue(AdditionalButtonsProperty);
        set => SetValue(AdditionalButtonsProperty, value);
    }

    // --- Events
    public event Action<object>? Selected;
    public event Action? Add;
    public event Action? Refresh;

    // --- Properties
    public ObservableCollection<GridViewColumn> Columns { get; }

    // --- Constructor
    protected StamdataListBase()
    {
        Columns = [];

        // Create the UI structure
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
        grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

        // Toolbar
        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5)
        };

        _newButton = new Button
        {
            Content = "New",
            Margin = new Thickness(0, 0, 5, 0),
            Style = (Style) Application.Current.Resources["PrimaryButton"]!
        };
        _newButton.Click += (_, _) => Add?.Invoke();

        _refreshButton = new Button
        {
            Content = "Refresh",
            Margin = new Thickness(0, 0, 5, 0),
            Style = (Style) Application.Current.Resources["PrimaryButton"]!
        };
        _refreshButton.Click += (_, _) => Refresh?.Invoke();

        var additionalButtonsPresenter = new ContentPresenter();
        additionalButtonsPresenter.SetBinding(ContentPresenter.ContentProperty,
            new Binding(nameof(AdditionalButtons)) {Source = this});

        toolbar.Children.Add(_newButton);
        toolbar.Children.Add(_refreshButton);
        toolbar.Children.Add(additionalButtonsPresenter);

        Grid.SetRow(toolbar, 0);
        grid.Children.Add(toolbar);

        // ListView
        var gridView = new GridView();
        _listView = new ListView
        {
            Margin = new Thickness(5),
            View = gridView
        };

        _listView.PreviewKeyDown += (_, e) =>
        {
            if (e.Key != Key.Down && e.Key != Key.Up) return;

            var currentIndex = _listView.SelectedIndex;

            var newIndex = e.Key switch
            {
                Key.Down when currentIndex < _listView.Items.Count - 1 => currentIndex + 1,
                Key.Up when currentIndex > 0 => currentIndex - 1,
                _ => currentIndex
            };

            if (newIndex == currentIndex) return;

            _listView.SelectedIndex = newIndex;
            _listView.ScrollIntoView(_listView.Items[newIndex]!);
            e.Handled = true;
        };

        _listView.SetBinding(ItemsControl.ItemsSourceProperty,
            new Binding(nameof(Items)) {Source = this});

        _listView.SelectionChanged += (_, _) =>
        {
            if (_listView.SelectedItem != null)
            {
                Selected?.Invoke(_listView.SelectedItem);
            }
        };

        Grid.SetRow(_listView, 1);
        grid.Children.Add(_listView);

        Content = grid;

        // When columns are added, add them to the GridView
        Columns.CollectionChanged += (_, e) =>
        {
            if (e.NewItems == null) return;

            foreach (GridViewColumn column in e.NewItems)
            {
                gridView.Columns.Add(column);
            }
        };
    }
}