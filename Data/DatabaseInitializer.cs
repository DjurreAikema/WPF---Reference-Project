using Dapper;
using Microsoft.Data.Sqlite;

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

    public static void SeedData()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var count = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Snacks");
        if (count == 0)
        {
            var insertCommand = @"
            INSERT INTO Snacks (Name, Price, Quantity) VALUES
            ('Doritos', 1.50, 4),
            ('Lays', 1.00, 3),
            ('Pringles', 2.00, 2),
            ('Cheetos', 1.25, 5),
            ('Ruffles', 1.75, 1),
            ('Tostitos', 1.50, 6),
            ('Sun Chips', 1.25, 7);
        ";
            using var insert = new SqliteCommand(insertCommand, connection);
            insert.ExecuteNonQuery();
        }
    }
}