using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.Data;
using WpfApp1.Shared.DataAccess;
using WpfApp1.Shared.Interfaces;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // --- V1 DB and Service ---
        // Configure DbContext with SQLite
        services.AddDbContext<SnackDbContext>(options =>
            options.UseSqlite("Data Source=snacks.db"));
        // Register Services
        services.AddScoped<ISnackService, SnackService>();

        // --- V2 DB and Service ---
        services.AddDbContext<SnackDbContextV2>(options =>
            options.UseSqlite("Data Source=snacksV2.db"));
        services.AddScoped<SnackServiceV2>();

        ServiceProvider = services.BuildServiceProvider();


        using (var scope = ServiceProvider.CreateScope())
        {
            // --- Initialize and seed the V1 database
            var contextV1 = scope.ServiceProvider.GetRequiredService<SnackDbContext>();
            contextV1.Database.EnsureCreated();
            DatabaseInitializer.SeedData(contextV1);

            // --- Initialize and seed the V2 database
            var contextV2 = scope.ServiceProvider.GetRequiredService<SnackDbContextV2>();
            contextV2.Database.EnsureCreated();
            DatabaseInitializerV2.SeedData(contextV2);
        }
    }
}