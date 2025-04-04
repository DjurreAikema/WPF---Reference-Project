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

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<UnitSize>? _unitSizes;
    [ObservableProperty] private UnitSize? _selected;

    // --- Constructor
    public SnackUnitSizes()
    {
        InitializeComponent();

        ListView.SelectionChanged += (_, _) =>
        {
            if (ListView.SelectedItem is UnitSize selected)
            {
                Selected = selected;
            }
        };
    }

    private void New_Click(object sender, RoutedEventArgs e) => Selected = new UnitSize();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
    }
}