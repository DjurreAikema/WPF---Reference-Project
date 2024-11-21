using Microsoft.Data.Sqlite;
using WpfApp1.Classes;

namespace WpfApp1.Data;

public static class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=Snacks.db";

    public static void InitializeDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var tableCommand = @"
            CREATE TABLE IF NOT EXISTS Snacks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Quantity INTEGER NOT NULL
            );
        ";

        using var createTable = new SqliteCommand(tableCommand, connection);
        createTable.ExecuteNonQuery();
    }

    public static void SeedData(SnackDbContext context)
    {
        if (context.Snacks.Any()) return;
        context.Snacks.AddRange(new List<Snack>
        {
            new Snack {Name = "Doritos", Price = 1.50, Quantity = 4},
            new Snack {Name = "Lays", Price = 1.00, Quantity = 3},
            new Snack {Name = "Pringles", Price = 2.00, Quantity = 2},
            new Snack {Name = "Cheetos", Price = 1.25, Quantity = 5},
            new Snack {Name = "Ruffles", Price = 1.75, Quantity = 1},
            new Snack {Name = "Tostitos", Price = 1.50, Quantity = 6},
            new Snack {Name = "Sun Chips", Price = 1.25, Quantity = 7},
        });

        context.SaveChanges();
    }
}