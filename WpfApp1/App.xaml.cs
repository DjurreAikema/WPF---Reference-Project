using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.Data;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.Interfaces;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set up dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Set global service provider reference
        Shared.Utils.ServiceProvider = _serviceProvider;

        // Initialize and seed database
        InitializeDatabase();

        // Show main window
        var mainWindow = _serviceProvider.GetRequiredService<Windows.Window7._1.WindowSevenOneV2>();
        mainWindow.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Register DbContext
        services.AddDbContext<SnackDbContextV2>(options =>
            options.UseSqlite("Data Source=SnacksV2.db"));

        // Register lock service
        services.AddSingleton<ILockService>(provider =>
            new SqliteLockService(
                provider,
                new LockServiceOptions
                {
                    DefaultLockDuration = TimeSpan.FromMinutes(30),
                    CleanupInterval = TimeSpan.FromMinutes(5)
                }
            )
        );

        // Register snack service
        services.AddTransient<SnackServiceV3>();

        // Register windows
        services.AddTransient<Windows.Window7._1.WindowSevenOneV2>();
    }

    private void InitializeDatabase()
    {
        // Create database schema if it doesn't exist
        DatabaseInitializerV2.InitializeDatabase();

        // Seed initial data if needed
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SnackDbContextV2>();
            DatabaseInitializerV2.SeedData(context);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Clean up resources
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}