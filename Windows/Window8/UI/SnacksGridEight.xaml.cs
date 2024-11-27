using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window8.UI;

public partial class SnacksGridEight
{
    // --- Dependency Properties
    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksGridEight),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnacksGridEight c) return;
            c.Disposables.Add(c.SnacksObs.Subscribe(snacks => { c.Snacks = new ObservableCollection<Snack>(snacks); }));
        }));

    public IObservable<IEnumerable<Snack>> SnacksObs
    {
        get => (IObservable<IEnumerable<Snack>>) GetValue(SnacksObsProperty);
        set => SetValue(SnacksObsProperty, value);
    }

    // --- Events
    public event Action<Snack>? SnackSelected;
    public event Action? AddSnack;
    public event Action? Reload;


    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Snack>? _snacks;

    // --- Constructor
    public SnacksGridEight()
    {
        InitializeComponent();

        SnacksDataGrid.SelectionChanged += (_, _) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack) SnackSelected?.Invoke(selectedSnack);
        };
    }

    private void New_OnClick(object sender, RoutedEventArgs e)
    {
        AddSnack?.Invoke();
    }

    private void Reload_OnClick(object sender, RoutedEventArgs e)
    {
        Reload?.Invoke();
    }
}