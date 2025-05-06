using System.Windows;
using WpfApp2.Shared.Debugging;

namespace WpfApp2.Views.Debug.VmTracking;

public partial class VmTrackingView
{
    // --- Constructor
    public VmTrackingView()
    {
        InitializeComponent();

        RefreshVmStats_Click(null, null);
        UpdateTrackingUi();
    }

    // --- Methods
    private void RefreshVmStats_Click(object? sender, RoutedEventArgs? e)
    {
        var stats = ViewModelTracker.Instance.GetAllStats();
        VmStatsListView.ItemsSource = stats;

        var instances = ViewModelTracker.Instance.GetAllInstances();
        VmInstancesListView.ItemsSource = instances;
    }

    private void ToggleTracking_Click(object sender, RoutedEventArgs e)
    {
        ViewModelTracker.Instance.IsEnabled = !ViewModelTracker.Instance.IsEnabled;
        UpdateTrackingUi();
    }

    private void UpdateTrackingUi()
    {
        var isEnabled = ViewModelTracker.Instance.IsEnabled;

        TrackingStatusText.Text = isEnabled ? "ON" : "OFF";
        TrackingStatusText.Foreground = isEnabled
            ? System.Windows.Media.Brushes.Green
            : System.Windows.Media.Brushes.Red;

        ToggleTrackingButton.Content = isEnabled ? "Turn Off" : "Turn On";
    }
}