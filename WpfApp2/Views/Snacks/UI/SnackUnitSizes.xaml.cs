using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.Snacks.UI;

public partial class SnackUnitSizes
{
    // --- Dependency Properties
    public static readonly DependencyProperty ListObsProperty = DependencyProperty.Register(
        nameof(ListObs), typeof(IObservable<IEnumerable<UnitSize>>), typeof(SnackUnitSizes),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackUnitSizes c) return;
            c.Disposables.Add(c.ListObs.Subscribe(list => { c.UnitSizes = new ObservableCollection<UnitSize>(list); }));
        }));

    public IObservable<IEnumerable<UnitSize>> ListObs
    {
        get => (IObservable<IEnumerable<UnitSize>>) GetValue(ListObsProperty);
        set => SetValue(ListObsProperty, value);
    }

    public static readonly DependencyProperty SelectedIdObsProperty = DependencyProperty.Register(
        nameof(SelectedIdObs), typeof(IObservable<int?>), typeof(SnackUnitSizes),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackUnitSizes c) return;
            c.Disposables.Add(c.SelectedIdObs.Subscribe(id =>
            {
                c.SelectedId = id;
                c.Selected = new UnitSize
                {
                    SnackId = c.SelectedId
                };
            }));
        }));

    public IObservable<int?> SelectedIdObs
    {
        get => (IObservable<int?>) GetValue(SelectedIdObsProperty);
        set => SetValue(SelectedIdObsProperty, value);
    }

    // --- Events
    public event Action<UnitSize>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<UnitSize>? _unitSizes;
    [ObservableProperty] private UnitSize? _selected;
    [ObservableProperty] private int? _selectedId;

    // --- Constructor
    public SnackUnitSizes()
    {
        InitializeComponent();

        Dg.SelectionChanged += (_, _) =>
        {
            if (Dg.SelectedItem is not UnitSize selected) return;
            Selected = selected;
        };
    }

    // --- UI Event Handlers
    private void New_Click(object sender, RoutedEventArgs e)
    {
        if (!SelectedId.HasValue) return;

        Selected = new UnitSize
        {
            SnackId = SelectedId
        };
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || !SelectedId.HasValue) return;
        Saved?.Invoke(Selected);
        Dg.SelectedItem = null;
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || Selected.Id is 0 or null) return;
        Deleted?.Invoke(Selected.Id.Value);
        Selected = null;
    }
}