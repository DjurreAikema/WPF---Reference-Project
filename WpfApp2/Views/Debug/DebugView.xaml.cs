using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp2.Data;
using WpfApp2.Shared.Debugging;

namespace WpfApp2.Views.Debug;

public partial class DebugView
{
    // --- Constructor
    public DebugView()
    {
        InitializeComponent();

        RefreshVmStats_Click(null, null);
        UpdateTrackingUI();
    }

    // --- Methods
    private void ReseedDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var result = MessageBox.Show(
                "This will delete all existing data and recreate the sample data. Are you sure?",
                "Confirm Database Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            using var scope = ((App) Application.Current).ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            DatabaseInitializer.SeedData(context, forceReseed: true);

            ShowResult("Database successfully reseeded with sample data.", true);
        }
        catch (Exception ex)
        {
            ShowResult($"Error reseeding database: {ex.Message}", false);
        }
    }

    private void ShowResult(string message, bool success)
    {
        ResultText.Text = message;
        ResultBorder.BorderBrush = success ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        ResultBorder.Visibility = Visibility.Visible;
    }

    private void RefreshVmStats_Click(object? sender, RoutedEventArgs? e)
    {
        var stats = ViewModelTracker.Instance.GetAllStats();
        VmStatsListView.ItemsSource = stats;
    }

    private void ToggleTracking_Click(object sender, RoutedEventArgs e)
    {
        ViewModelTracker.Instance.IsEnabled = !ViewModelTracker.Instance.IsEnabled;
        UpdateTrackingUI();
    }

    private void UpdateTrackingUI()
    {
        var isEnabled = ViewModelTracker.Instance.IsEnabled;

        TrackingStatusText.Text = isEnabled ? "ON" : "OFF";
        TrackingStatusText.Foreground = isEnabled
            ? System.Windows.Media.Brushes.Green
            : System.Windows.Media.Brushes.Red;

        ToggleTrackingButton.Content = isEnabled ? "Turn Off" : "Turn On";
    }
}