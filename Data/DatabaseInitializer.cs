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
}