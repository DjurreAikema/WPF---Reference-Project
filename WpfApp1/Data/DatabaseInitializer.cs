using Microsoft.Data.Sqlite;
using WpfApp1.Shared.Classes;

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
            new() {Name = "Doritos", Price = 1.50, Quantity = 4},
            new() {Name = "Lays", Price = 1.00, Quantity = 3},
            new() {Name = "Pringles", Price = 2.00, Quantity = 2},
            new() {Name = "Cheetos", Price = 1.25, Quantity = 5},
            new() {Name = "Ruffles", Price = 1.75, Quantity = 1},
            new() {Name = "Tostitos", Price = 1.50, Quantity = 6},
            new() {Name = "Sun Chips", Price = 1.25, Quantity = 7},
        });

        context.SaveChanges();
    }
}