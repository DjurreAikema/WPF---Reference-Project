using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7._1.UI;

public partial class SnackDetailsSevenOne
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedSnackObsProperty = DependencyProperty.Register(
        nameof(SelectedSnackObs), typeof(IObservable<SnackV2?>), typeof(SnackDetailsSevenOne),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetailsSevenOne c) return;
            c.Disposables.Add(c.SelectedSnackObs.Subscribe(snack =>
            {
                c.SelectedSnack = snack != null ? new SnackV2(snack) : null;
                c.OnPropertyChanged(nameof(SelectedSnack));
            }));
        }));

    public IObservable<SnackV2?> SelectedSnackObs
    {
        get => (IObservable<SnackV2?>) GetValue(SelectedSnackObsProperty);
        set => SetValue(SelectedSnackObsProperty, value);
    }

    // --- Events
    public event Action<SnackV2>? SnackSaved;
    public event Action<int>? SnackDeleted;

    // --- Internal Properties
    [ObservableProperty] private SnackV2? _selectedSnack;

    // --- Constructor
    public SnackDetailsSevenOne()
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