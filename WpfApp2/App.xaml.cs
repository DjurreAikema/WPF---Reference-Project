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
        // Example: services.AddScoped<IInventoryService, InventoryService>();

        ServiceProvider = services.BuildServiceProvider();

        // Initialize and seed the database
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            DatabaseInitializer.SeedData(context);
        }
    }
}