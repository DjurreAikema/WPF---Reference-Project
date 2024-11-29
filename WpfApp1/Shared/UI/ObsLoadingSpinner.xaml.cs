using System.Windows;

namespace WpfApp1.Shared.UI;

public partial class ObsLoadingSpinner
{
    // --- Dependency Properties
    public static readonly DependencyProperty IsLoadingObsProperty = DependencyProperty.Register(
        nameof(IsLoadingObs), typeof(IObservable<bool>), typeof(ObsLoadingSpinner),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not ObsLoadingSpinner c) return;
            c.Disposables.Add(c.IsLoadingObs.Subscribe(isLoading =>
            {
                c.IsLoading = isLoading;
                c.OnPropertyChanged(nameof(IsLoading));
            }));
        }));

    public IObservable<bool> IsLoadingObs
    {
        get => (IObservable<bool>) GetValue(IsLoadingObsProperty);
        set => SetValue(IsLoadingObsProperty, value);
    }

    // --- Internal Properties
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    // --- Constructor
    public ObsLoadingSpinner()
    {
        InitializeComponent();
    }
}