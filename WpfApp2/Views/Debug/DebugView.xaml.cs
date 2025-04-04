using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp2.Data;

namespace WpfApp2.Views.Debug;

public partial class DebugView
{
    public DebugView()
    {
        InitializeComponent();
    }

    private void ReseedDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var result = MessageBox.Show(
                "This will delete all existing data and recreate the sample data. Are you sure?",
                "Confirm Database Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var scope = ((App) Application.Current).ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                DatabaseInitializer.SeedData(context, forceReseed: true);

                ShowResult("Database successfully reseeded with sample data.", true);
            }
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
}