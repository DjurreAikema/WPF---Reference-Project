using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7._2.UI;

public partial class SnacksGridSevenTwo
{
    // --- Dependency Properties
    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<SnackV2>>), typeof(SnacksGridSevenTwo),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnacksGridSevenTwo c) return;
            c.Disposables.Add(c.SnacksObs.Subscribe(snacks => { c.Snacks = new ObservableCollection<SnackV2>(snacks); }));
        }));

    public IObservable<IEnumerable<SnackV2>> SnacksObs
    {
        get => (IObservable<IEnumerable<SnackV2>>) GetValue(SnacksObsProperty);
        set => SetValue(SnacksObsProperty, value);
    }

    // --- Events
    public event Action<SnackV2>? SnackSelected;
    public event Action? AddSnack;
    public event Action? Reload;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<SnackV2>? _snacks;

    // --- Constructor
    public SnacksGridSevenTwo()
    {
        InitializeComponent();
    }

    private void SnacksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SnacksDataGrid.SelectedItem is not SnackV2 selectedSnack) return;
        SnackSelected?.Invoke(selectedSnack);
    }

    private void New_OnClick(object sender, RoutedEventArgs e) => AddSnack?.Invoke();

    private void Reload_OnClick(object sender, RoutedEventArgs e) => Reload?.Invoke();
}