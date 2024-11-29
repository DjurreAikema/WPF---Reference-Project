using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7.UI;

public partial class SnackDetailsSeven
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<Snack?>), typeof(SnackDetailsSeven),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsSeven c) return;
            c.Disposables.Add(c.SelectedSnackObs.Subscribe(snack =>
            {
                c.SelectedSnack = snack != null ? new Snack(snack) : null;
                c.OnPropertyChanged(nameof(SelectedSnack));
            }));
        }));

    public IObservable<Snack> SelectedSnackObs
    {
        get => (IObservable<Snack>) GetValue(SelectedSnackObsProperty);
        set => SetValue(SelectedSnackObsProperty, value);
    }

    // --- Events
    public event Action<Snack>? SnackSaved;
    public event Action<int>? SnackDeleted;

    // --- Internal Properties
    [ObservableProperty] private Snack? _selectedSnack;

    // --- Constructor
    public SnackDetailsSeven()
    {
        InitializeComponent();
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectedSnack == null) return;
        SnackSaved?.Invoke(SelectedSnack);
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectedSnack == null || SelectedSnack.Id is 0 or null) return;
        SnackDeleted?.Invoke(SelectedSnack.Id.Value);
    }
}