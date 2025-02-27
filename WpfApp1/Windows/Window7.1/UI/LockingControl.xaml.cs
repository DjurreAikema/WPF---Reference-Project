using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window7._1.UI;

public partial class LockingControl
{
    // --- Dependency Properties
    public static readonly DependencyProperty LockingObsProperty = DependencyProperty.Register(
        nameof(LockingObs), typeof(IObservable<SnackV2?>), typeof(LockingControl),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not LockingControl c) return;
            c.Disposables.Add(c.LockingObs.Subscribe(snack =>
            {
                c.SelectedSnack = snack != null ? new SnackV2(snack) : null;
                c.OnPropertyChanged(nameof(SelectedSnack));
            }));
        }));

    public IObservable<SnackV2?> LockingObs
    {
        get => (IObservable<SnackV2?>) GetValue(LockingObsProperty);
        set => SetValue(LockingObsProperty, value);
    }

    // --- Internal Properties
    [ObservableProperty] private SnackV2? _selectedSnack;

    // --- Constructor
    public LockingControl()
    {
        InitializeComponent();
    }
}