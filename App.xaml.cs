using System.Windows;
using WpfApp1.Data;
using SQLitePCL;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Batteries.Init(); // Initialize SQLite provider

        DatabaseInitializer.InitializeDatabase();
        DatabaseInitializer.SeedData();
    }
}