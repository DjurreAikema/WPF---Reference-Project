using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Views.StamData.Snacks.UI;

public partial class SnackDetails
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedObsProperty = DependencyProperty.Register(
        nameof(SelectedObs), typeof(IObservable<Snack?>), typeof(SnackDetails),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackDetails c) return;
            c.Disposables.Add(c.SelectedObs.Subscribe(obj =>
            {
                c.Selected = obj != null ? new Snack(obj) : null;
                c.OnPropertyChanged(nameof(Selected));
            }));
        }));

    public IObservable<Snack?> SelectedObs
    {
        get => (IObservable<Snack?>) GetValue(SelectedObsProperty);
        set => SetValue(SelectedObsProperty, value);
    }

    // --- Events
    public event Action<Snack>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private Snack? _selected;
    [ObservableProperty] private bool _hasId;

    // --- Constructor
    public SnackDetails()
    {
        InitializeComponent();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null) return;
        Saved?.Invoke(Selected);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || Selected.Id is 0 or null) return;
        Deleted?.Invoke(Selected.Id.Value);
    }
}