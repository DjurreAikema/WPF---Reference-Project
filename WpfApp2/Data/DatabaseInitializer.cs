using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes;

namespace WpfApp2.Data;

public class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=Inventory.db";

    public static void InitializeDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Create tables if they don't exist
        CreateCountriesTable(connection);
        CreateWarehousesTable(connection);
        CreateSnacksTable(connection);
        CreateUnitSizesTable(connection);
        CreateInventoriesTable(connection);
    }

    // Countries table
    private static void CreateCountriesTable(SqliteConnection connection)
    {
        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS Countries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // Warehouses table
    private static void CreateWarehousesTable(SqliteConnection connection)
    {
        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS Warehouses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CountryId INTEGER NULL,
                Name TEXT NOT NULL,
                City TEXT NOT NULL,
                ZipCode TEXT NOT NULL,
                Address TEXT NOT NULL,
                HouseNumber TEXT NOT NULL,
                FOREIGN KEY (CountryId) REFERENCES Countries (Id)
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // Snacks table
    private static void CreateSnacksTable(SqliteConnection connection)
    {
        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS Snacks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Brand TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL,
                MultipleUnitSizes INTEGER NOT NULL
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // Unit sizes table
    private static void CreateUnitSizesTable(SqliteConnection connection)
    {
        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS UnitSizes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SnackId INTEGER NULL,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL,
                Description TEXT,
                FOREIGN KEY (SnackId) REFERENCES Snacks (Id)
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // Inventories table
    private static void CreateInventoriesTable(SqliteConnection connection)
    {
        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS Inventories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SnackId INTEGER NOT NULL,
                WarehouseId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                FOREIGN KEY (SnackId) REFERENCES Snacks (Id),
                FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id)
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // --- Seeding
    public static void SeedData(AppDbContext context, bool forceReseed = false)
    {
        // Check if data already exists, unless forceReseed is true
        if (!forceReseed && context.Countries.Any()) return;
        if (forceReseed) ClearAllData(context);

        // Add countries
        var countries = new List<Country>
        {
            new() {Name = "United States"},
            new() {Name = "United Kingdom"},
            new() {Name = "Canada"},
            new() {Name = "Germany"},
            new() {Name = "Japan"}
        };
        context.Countries.AddRange(countries);
        context.SaveChanges();

        // Add warehouses
        var warehouses = new List<Warehouse>
        {
            new()
            {
                Name = "Main Warehouse",
                CountryId = 1,
                City = "New York",
                ZipCode = "10001",
                Address = "Broadway",
                HouseNumber = "123"
            },
            new()
            {
                Name = "Secondary Warehouse",
                CountryId = 1,
                City = "Los Angeles",
                ZipCode = "90001",
                Address = "Hollywood Blvd",
                HouseNumber = "456"
            },
            new()
            {
                Name = "European Warehouse",
                CountryId = 4,
                City = "Berlin",
                ZipCode = "10115",
                Address = "Brandenburg Gate",
                HouseNumber = "789"
            }
        };
        context.Warehouses.AddRange(warehouses);
        context.SaveChanges();

        // Add snacks
        var snacks = new List<Snack>
        {
            new()
            {
                Name = "Doritos",
                Brand = "Frito-Lay",
                Price = 1.50,
                Quantity = 100,
                MultipleUnitSizes = true
            },
            new()
            {
                Name = "Lay's",
                Brand = "Frito-Lay",
                Price = 1.25,
                Quantity = 150,
                MultipleUnitSizes = true
            },
            new()
            {
                Name = "Oreo",
                Brand = "Nabisco",
                Price = 2.00,
                Quantity = 75,
                MultipleUnitSizes = false
            },
            new()
            {
                Name = "Haribo",
                Brand = "Haribo",
                Price = 1.75,
                Quantity = 120,
                MultipleUnitSizes = true
            }
        };
        context.Snacks.AddRange(snacks);
        context.SaveChanges();

        // Add unit sizes
        var unitSizes = new List<UnitSize>
        {
            new() {Name = "Single", SnackId = 1, Price = 1.50, Quantity = 1},
            new() {Name = "Three-pack", SnackId = 1, Price = 4.00, Quantity = 3},
            new() {Name = "Single", SnackId = 2, Price = 1.25, Quantity = 1},
            new() {Name = "Three-pack", SnackId = 2, Price = 3.50, Quantity = 3},
            new() {Name = "Five-pack", SnackId = 2, Price = 5.50, Quantity = 5},
            new() {Name = "Single", SnackId = 4, Price = 1.75, Quantity = 1},
            new() {Name = "Three-pack", SnackId = 4, Price = 4.50, Quantity = 3}
        };
        context.UnitSizes.AddRange(unitSizes);
        context.SaveChanges();

        // Add inventory records
        var inventories = new List<Inventory>
        {
            new() {SnackId = 1, WarehouseId = 1, Quantity = 100}, // New York
            new() {SnackId = 1, WarehouseId = 2, Quantity = 50}, // Los Angeles
            new() {SnackId = 2, WarehouseId = 1, Quantity = 75}, // New York
            new() {SnackId = 2, WarehouseId = 2, Quantity = 75}, // Los Angeles
            new() {SnackId = 2, WarehouseId = 3, Quantity = 50}, // Berlin
            new() {SnackId = 3, WarehouseId = 2, Quantity = 75}, // Los Angeles
            new() {SnackId = 4, WarehouseId = 3, Quantity = 180} // Berlin
        };
        context.Inventories.AddRange(inventories);
        context.SaveChanges();
    }

    private static void ClearAllData(AppDbContext context)
    {
        // Use a transaction to ensure all operations succeed or fail together
        using var transaction = context.Database.BeginTransaction();
        try
        {
            // Disable foreign key checks temporarily
            context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

            // Clear tables in correct order
            context.Database.ExecuteSqlRaw("DELETE FROM Inventories");
            context.Database.ExecuteSqlRaw("DELETE FROM UnitSizes");
            context.Database.ExecuteSqlRaw("DELETE FROM Snacks");
            context.Database.ExecuteSqlRaw("DELETE FROM Warehouses");
            context.Database.ExecuteSqlRaw("DELETE FROM Countries");

            // Reset primary keys (auto-increment)
            context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name IN ('Inventories', 'UnitSizes', 'Snacks', 'Warehouses', 'Countries')");

            // Re-enable foreign key checks
            context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

            // Commit the transaction
            transaction.Commit();
        }
        catch
        {
            // If anything fails, roll back all changes
            transaction.Rollback();
            throw;
        }
    }

    public static void DeleteDatabase()
    {
        // Close all connections to the database
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Delete the database file
        string dbFilePath = "Inventory.db";
        if (File.Exists(dbFilePath))
        {
            try
            {
                File.Delete(dbFilePath);
                Console.WriteLine("Database deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting database: {ex.Message}");
                throw;
            }
        }
        // Database will be recreated by EF Core when next accessed
    }
}