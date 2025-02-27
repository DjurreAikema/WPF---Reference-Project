using Microsoft.Data.Sqlite;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Data;

/// <summary>
/// Updated database initializer with enhanced locking fields
/// </summary>
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
                    LockedAt TEXT NULL,
                    LockedBy TEXT NULL,
                    LockExpiresAt TEXT NULL,
                    LockReason TEXT NULL
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
            new()
            {
                Name = "Doritos V2",
                Price = 1.50,
                Quantity = 4,
                LockedAt = null,
                LockedBy = null,
                LockExpiresAt = null,
                LockReason = null
            },
            new()
            {
                Name = "Lays V2",
                Price = 1.00,
                Quantity = 3,
                LockedAt = null,
                LockedBy = null,
                LockExpiresAt = null,
                LockReason = null
            },
            new()
            {
                Name = "Pringles V2",
                Price = 2.00,
                Quantity = 2,
                LockedAt = null,
                LockedBy = null,
                LockExpiresAt = null,
                LockReason = null
            },
            new()
            {
                Name = "Cheetos V2",
                Price = 1.25,
                Quantity = 5,
                LockedAt = null,
                LockedBy = null,
                LockExpiresAt = null,
                LockReason = null
            },
            // Here we demonstrate an example that is already locked
            new()
            {
                Name = "Ruffles V2",
                Price = 1.75,
                Quantity = 1,
                LockedAt = DateTime.UtcNow,
                LockedBy = "SYSTEM_USER",
                LockExpiresAt = DateTime.UtcNow.AddHours(1),
                LockReason = "System maintenance"
            },
        });

        context.SaveChanges();
    }
}