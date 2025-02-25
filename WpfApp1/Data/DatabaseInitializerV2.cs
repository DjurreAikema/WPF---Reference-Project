using Microsoft.Data.Sqlite;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Data;

public class DatabaseInitializerV2
{
    private const string ConnectionString = "Data Source=SnacksV2.db";

    public static void InitializeDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var tableCommand = @"
                CREATE TABLE IF NOT EXISTS SnacksV2 (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Locked TEXT NULL,
                    LockedBy TEXT NULL
                );
            ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    public static void SeedData(SnackDbContextV2 context)
    {
        if (context.Snacks.Any()) return;

        context.Snacks.AddRange(new List<SnackV2>
        {
            new() {Name = "Doritos V2", Price = 1.50, Quantity = 4, Locked = null, LockedBy = null},
            new() {Name = "Lays V2", Price = 1.00, Quantity = 3, Locked = null, LockedBy = null},
            new() {Name = "Pringles V2", Price = 2.00, Quantity = 2, Locked = null, LockedBy = null},
            new() {Name = "Cheetos V2", Price = 1.25, Quantity = 5, Locked = null, LockedBy = null},
            // Here we demonstrate an example that is already locked
            new() {Name = "Ruffles V2", Price = 1.75, Quantity = 1, Locked = DateTime.UtcNow, LockedBy = "SYSTEM_USER"},
        });

        context.SaveChanges();
    }
}