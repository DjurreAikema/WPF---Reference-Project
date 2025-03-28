using Microsoft.Data.Sqlite;
using WpfApp2.Shared.Classes;

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
                WarehouseId INTEGER NULL,
                Name TEXT NOT NULL,
                Brand TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL,
                MultipleUnitSizes INTEGER NOT NULL,
                FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id)
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
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL,
                FOREIGN KEY (SnackId) REFERENCES Snacks (Id)
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    // --- Seeding
    public static void SeedData(AppDbContext context)
    {
        // Only seed if the database is empty
        if (context.Countries.Any()) return;

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
                WarehouseId = 1,
                MultipleUnitSizes = true
            },
            new()
            {
                Name = "Lay's",
                Brand = "Frito-Lay",
                Price = 1.25,
                Quantity = 150,
                WarehouseId = 1,
                MultipleUnitSizes = true
            },
            new()
            {
                Name = "Oreo",
                Brand = "Nabisco",
                Price = 2.00,
                Quantity = 75,
                WarehouseId = 2,
                MultipleUnitSizes = false
            },
            new()
            {
                Name = "Haribo",
                Brand = "Haribo",
                Price = 1.75,
                Quantity = 120,
                WarehouseId = 3,
                MultipleUnitSizes = true
            }
        };
        context.Snacks.AddRange(snacks);
        context.SaveChanges();

        // Add unit sizes
        var unitSizes = new List<UnitSize>
        {
            new() {SnackId = 1, Price = 1.50, Quantity = 1},
            new() {SnackId = 1, Price = 4.00, Quantity = 3},
            new() {SnackId = 2, Price = 1.25, Quantity = 1},
            new() {SnackId = 2, Price = 3.50, Quantity = 3},
            new() {SnackId = 2, Price = 5.50, Quantity = 5},
            new() {SnackId = 4, Price = 1.75, Quantity = 1},
            new() {SnackId = 4, Price = 4.50, Quantity = 3}
        };
        context.UnitSizes.AddRange(unitSizes);
        context.SaveChanges();
    }
}