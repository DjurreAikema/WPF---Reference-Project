using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Snacks.UI;

public partial class SnacksList
{
    // --- Dependency Properties
    public static readonly DependencyProperty ListObsProperty = DependencyProperty.Register(
        nameof(ListObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksList),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnacksList c) return;
            c.Disposables.Add(c.ListObs.Subscribe(list => { c.Snacks = new ObservableCollection<Snack>(list); }));
        }));

    public IObservable<IEnumerable<Snack>> ListObs
    {
        get => (IObservable<IEnumerable<Snack>>) GetValue(ListObsProperty);
        set => SetValue(ListObsProperty, value);
    }

    // --- Events
    public event Action<Snack>? Selected;
    public event Action? Add;
    public event Action? Reload;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Snack>? _snacks;

    // --- Constructor
    public SnacksList()
    {
        InitializeComponent();

        ListView.SelectionChanged += (_, _) =>
        {
            if (ListView.SelectedItem is Snack selected)
            {
                Selected?.Invoke(selected);
            }
        };
    }

    private void New_Click(object sender, RoutedEventArgs e) => Add?.Invoke();

    private void Refresh_Click(object sender, RoutedEventArgs e) => Reload?.Invoke();
}