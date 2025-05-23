using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Data.Classes.Orders;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Data.Enums;

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
        CreateSuppliersTable(connection);
        CreateInboundOrdersTable(connection);
        CreateInboundOrderLinesTable(connection);
        CreateInboundReceiptsTable(connection);
        CreateInboundReceiptLinesTable(connection);
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
                UnitSizeId INTEGER NULL,
                Quantity INTEGER NOT NULL,
                FOREIGN KEY (SnackId) REFERENCES Snacks (Id),
                FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id),
                FOREIGN KEY (UnitSizeId) REFERENCES UnitSizes (Id)
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    private static void CreateSuppliersTable(SqliteConnection connection)
    {
        var tableCommand = @"
        CREATE TABLE IF NOT EXISTS Suppliers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CountryId INTEGER NULL,
            Name TEXT NOT NULL,
            ContactPerson TEXT NOT NULL,
            Email TEXT NOT NULL,
            Phone TEXT NOT NULL,
            Address TEXT NOT NULL,
            FOREIGN KEY (CountryId) REFERENCES Countries (Id)
        );
    ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    private static void CreateInboundOrdersTable(SqliteConnection connection)
    {
        var tableCommand = @"
        CREATE TABLE IF NOT EXISTS InboundOrders (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SupplierId INTEGER NULL,
            OrderNumber TEXT NOT NULL,
            OrderDate TEXT NOT NULL,
            ExpectedDeliveryDate TEXT NULL,
            Status INTEGER NOT NULL,
            Notes TEXT NOT NULL,
            FOREIGN KEY (SupplierId) REFERENCES Suppliers (Id)
        );
    ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    private static void CreateInboundOrderLinesTable(SqliteConnection connection)
    {
        var tableCommand = @"
        CREATE TABLE IF NOT EXISTS InboundOrderLines (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            InboundOrderId INTEGER NOT NULL,
            SnackId INTEGER NOT NULL,
            UnitSizeId INTEGER NULL,
            Quantity INTEGER NOT NULL,
            UnitPrice REAL NOT NULL,
            FOREIGN KEY (InboundOrderId) REFERENCES InboundOrders (Id),
            FOREIGN KEY (SnackId) REFERENCES Snacks (Id),
            FOREIGN KEY (UnitSizeId) REFERENCES UnitSizes (Id)
        );
    ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    private static void CreateInboundReceiptsTable(SqliteConnection connection)
    {
        var tableCommand = @"
        CREATE TABLE IF NOT EXISTS InboundReceipts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            InboundOrderId INTEGER NOT NULL,
            WarehouseId INTEGER NOT NULL,
            ReceiptDate TEXT NOT NULL,
            ReceivedBy TEXT NOT NULL,
            Notes TEXT NOT NULL,
            FOREIGN KEY (InboundOrderId) REFERENCES InboundOrders (Id),
            FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id)
        );
    ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    private static void CreateInboundReceiptLinesTable(SqliteConnection connection)
    {
        var tableCommand = @"
        CREATE TABLE IF NOT EXISTS InboundReceiptLines (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            InboundReceiptId INTEGER NOT NULL,
            InboundOrderLineId INTEGER NOT NULL,
            QuantityReceived INTEGER NOT NULL,
            Notes TEXT NOT NULL,
            FOREIGN KEY (InboundReceiptId) REFERENCES InboundReceipts (Id),
            FOREIGN KEY (InboundOrderLineId) REFERENCES InboundOrderLines (Id)
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
            new() {SnackId = 1, WarehouseId = 1, UnitSizeId = 1, Quantity = 60}, // New York, Single
            new() {SnackId = 1, WarehouseId = 1, UnitSizeId = 2, Quantity = 40}, // New York, Three-pack
            new() {SnackId = 1, WarehouseId = 2, UnitSizeId = 1, Quantity = 30}, // Los Angeles, Single
            new() {SnackId = 1, WarehouseId = 2, UnitSizeId = 2, Quantity = 20}, // Los Angeles, Three-pack
            new() {SnackId = 2, WarehouseId = 1, UnitSizeId = 3, Quantity = 40}, // New York, Single
            new() {SnackId = 2, WarehouseId = 1, UnitSizeId = 4, Quantity = 35}, // New York, Three-pack
            new() {SnackId = 2, WarehouseId = 2, UnitSizeId = 3, Quantity = 45}, // Los Angeles, Single
            new() {SnackId = 2, WarehouseId = 2, UnitSizeId = 4, Quantity = 30}, // Los Angeles, Three-pack
            new() {SnackId = 2, WarehouseId = 3, UnitSizeId = 3, Quantity = 25}, // Berlin, Single
            new() {SnackId = 2, WarehouseId = 3, UnitSizeId = 4, Quantity = 25}, // Berlin, Three-pack
            new() {SnackId = 3, WarehouseId = 2, UnitSizeId = null, Quantity = 75}, // Los Angeles (no unit sizes)
            new() {SnackId = 4, WarehouseId = 3, UnitSizeId = 6, Quantity = 100}, // Berlin, Single
            new() {SnackId = 4, WarehouseId = 3, UnitSizeId = 7, Quantity = 80} // Berlin, Three-pack
        };
        context.Inventories.AddRange(inventories);
        context.SaveChanges();

        // Add suppliers
        var suppliers = new List<Supplier>
        {
            new()
            {
                Name = "Frito-Lay Distributors",
                CountryId = 1, // United States
                ContactPerson = "John Smith",
                Email = "john.smith@fritolay.com",
                Phone = "+1-555-0101",
                Address = "123 Snack Avenue, Dallas, TX"
            },
            new()
            {
                Name = "Nabisco Supply Co",
                CountryId = 1, // United States
                ContactPerson = "Sarah Johnson",
                Email = "sarah.johnson@nabisco.com",
                Phone = "+1-555-0102",
                Address = "456 Cookie Street, Chicago, IL"
            },
            new()
            {
                Name = "European Snack Import",
                CountryId = 4, // Germany
                ContactPerson = "Hans Mueller",
                Email = "hans.mueller@eurosnack.de",
                Phone = "+49-30-12345678",
                Address = "789 Snack Strasse, Berlin"
            }
        };
        context.Suppliers.AddRange(suppliers);
        context.SaveChanges();

        // Add inbound orders
        var inboundOrders = new List<InboundOrder>
        {
            new()
            {
                SupplierId = 1, // Frito-Lay Distributors
                OrderNumber = "PO-2024-001",
                OrderDate = DateTime.Now.AddDays(-30),
                ExpectedDeliveryDate = DateTime.Now.AddDays(-25),
                Status = OrderStatusEnum.FullyReceived,
                Notes = "Regular monthly order for Doritos and Lay's"
            },
            new()
            {
                SupplierId = 2, // Nabisco Supply Co
                OrderNumber = "PO-2024-002",
                OrderDate = DateTime.Now.AddDays(-20),
                ExpectedDeliveryDate = DateTime.Now.AddDays(-15),
                Status = OrderStatusEnum.PartiallyReceived,
                Notes = "Oreo shipment - partial delivery due to transport delays"
            },
            new()
            {
                SupplierId = 3, // European Snack Import
                OrderNumber = "PO-2024-003",
                OrderDate = DateTime.Now.AddDays(-10),
                ExpectedDeliveryDate = DateTime.Now.AddDays(5),
                Status = OrderStatusEnum.InTransit,
                Notes = "Haribo gummy bears - international shipment"
            }
        };
        context.InboundOrders.AddRange(inboundOrders);
        context.SaveChanges();

        // Add inbound order lines
        var inboundOrderLines = new List<InboundOrderLine>
        {
            // Order 1 lines (Frito-Lay order)
            new()
            {
                InboundOrderId = 1,
                SnackId = 1, // Doritos
                UnitSizeId = 1, // Single pack
                Quantity = 100,
                UnitPrice = 1.25 // Wholesale price
            },
            new()
            {
                InboundOrderId = 1,
                SnackId = 1, // Doritos
                UnitSizeId = 2, // Three-pack
                Quantity = 50,
                UnitPrice = 3.50
            },
            new()
            {
                InboundOrderId = 1,
                SnackId = 2, // Lay's
                UnitSizeId = 3, // Single pack
                Quantity = 150,
                UnitPrice = 1.00
            },

            // Order 2 lines (Nabisco order)
            new()
            {
                InboundOrderId = 2,
                SnackId = 3, // Oreo
                UnitSizeId = null, // No unit sizes for Oreo
                Quantity = 200,
                UnitPrice = 1.75
            },

            // Order 3 lines (European order)
            new()
            {
                InboundOrderId = 3,
                SnackId = 4, // Haribo
                UnitSizeId = 6, // Single pack
                Quantity = 120,
                UnitPrice = 1.50
            },
            new()
            {
                InboundOrderId = 3,
                SnackId = 4, // Haribo
                UnitSizeId = 7, // Three-pack
                Quantity = 60,
                UnitPrice = 4.00
            }
        };
        context.InboundOrderLines.AddRange(inboundOrderLines);
        context.SaveChanges();

        // Add inbound receipts
        var inboundReceipts = new List<InboundReceipt>
        {
            new()
            {
                InboundOrderId = 1,
                WarehouseId = 1, // Main Warehouse
                ReceiptDate = DateTime.Now.AddDays(-25),
                ReceivedBy = "Mike Johnson",
                Notes = "All items received in good condition"
            },
            new()
            {
                InboundOrderId = 1,
                WarehouseId = 2, // Secondary Warehouse
                ReceiptDate = DateTime.Now.AddDays(-24),
                ReceivedBy = "Lisa Chen",
                Notes = "Partial shipment to West Coast warehouse"
            },
            new()
            {
                InboundOrderId = 2,
                WarehouseId = 2, // Secondary Warehouse
                ReceiptDate = DateTime.Now.AddDays(-15),
                ReceivedBy = "Tom Wilson",
                Notes = "Partial delivery - 150 units only, waiting for remaining 50"
            }
        };
        context.InboundReceipts.AddRange(inboundReceipts);
        context.SaveChanges();

        // Add inbound receipt lines
        var inboundReceiptLines = new List<InboundReceiptLine>
        {
            // Receipt 1 lines (Main Warehouse - Frito-Lay)
            new()
            {
                InboundReceiptId = 1,
                InboundOrderLineId = 1, // Doritos Single
                QuantityReceived = 60,
                Notes = "Good condition"
            },
            new()
            {
                InboundReceiptId = 1,
                InboundOrderLineId = 2, // Doritos Three-pack
                QuantityReceived = 30,
                Notes = "Good condition"
            },
            new()
            {
                InboundReceiptId = 1,
                InboundOrderLineId = 3, // Lay's Single
                QuantityReceived = 80,
                Notes = "Good condition"
            },

            // Receipt 2 lines (Secondary Warehouse - Frito-Lay)
            new()
            {
                InboundReceiptId = 2,
                InboundOrderLineId = 1, // Doritos Single
                QuantityReceived = 40,
                Notes = "Remaining quantity from order"
            },
            new()
            {
                InboundReceiptId = 2,
                InboundOrderLineId = 2, // Doritos Three-pack
                QuantityReceived = 20,
                Notes = "Remaining quantity from order"
            },
            new()
            {
                InboundReceiptId = 2,
                InboundOrderLineId = 3, // Lay's Single
                QuantityReceived = 70,
                Notes = "Remaining quantity from order"
            },

            // Receipt 3 lines (Secondary Warehouse - Nabisco partial)
            new()
            {
                InboundReceiptId = 3,
                InboundOrderLineId = 4, // Oreo
                QuantityReceived = 150,
                Notes = "Partial delivery - 50 units still pending"
            }
        };
        context.InboundReceiptLines.AddRange(inboundReceiptLines);
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
            context.Database.ExecuteSqlRaw("DELETE FROM InboundReceiptLines");
            context.Database.ExecuteSqlRaw("DELETE FROM InboundReceipts");
            context.Database.ExecuteSqlRaw("DELETE FROM InboundOrderLines");
            context.Database.ExecuteSqlRaw("DELETE FROM InboundOrders");
            context.Database.ExecuteSqlRaw("DELETE FROM Suppliers");
            context.Database.ExecuteSqlRaw("DELETE FROM Inventories");
            context.Database.ExecuteSqlRaw("DELETE FROM UnitSizes");
            context.Database.ExecuteSqlRaw("DELETE FROM Snacks");
            context.Database.ExecuteSqlRaw("DELETE FROM Warehouses");
            context.Database.ExecuteSqlRaw("DELETE FROM Countries");

            // Reset primary keys (auto-increment)
            context.Database.ExecuteSqlRaw(
                "DELETE FROM sqlite_sequence WHERE name IN ('InboundReceiptLines', 'InboundReceipts', 'InboundOrderLines', 'InboundOrders', 'Suppliers', 'Inventories', 'UnitSizes', 'Snacks', 'Warehouses', 'Countries')");

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