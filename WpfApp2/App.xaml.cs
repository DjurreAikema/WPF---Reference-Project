using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfApp2.Data;

namespace WpfApp2;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SQLitePCL.Batteries.Init();

        var services = new ServiceCollection();

        // Configure DbContext with SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=Inventory.db"));

        // Add other services as needed
        services.AddScoped<DatabaseInitializer>();

        ServiceProvider = services.BuildServiceProvider();

        // Initialize database STRUCTURE only
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();

            // Only initialize database structure, don't seed data automatically
            DatabaseInitializer.InitializeDatabase();

            // Only seed data if database is completely empty
            if (!context.Countries.Any() && !context.Warehouses.Any() && !context.Snacks.Any() && !context.Inventories.Any())
            {
                DatabaseInitializer.SeedData(context);
            }
        }
    }
}