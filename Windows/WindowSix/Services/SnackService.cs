using Dapper;
using Microsoft.Data.Sqlite;
using WpfApp1.Classes;
using WpfApp1.Windows.WindowSix.Interfaces;

namespace WpfApp1.Windows.WindowSix.Services;

public class SnackService : ISnackService
{
    private const string ConnectionString = "Data Source=snacks.db";

    public async Task<IEnumerable<Snack>> GetAllSnacksAsync()
    {
        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        var snacks = await connection.QueryAsync<Snack>("SELECT * FROM Snacks");
        return snacks;
    }

    public async Task AddSnackAsync(Snack snack)
    {
        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        const string command = "INSERT INTO Snacks (Name, Price, Quantity) VALUES (@Name, @Price, @Quantity)";
        await connection.ExecuteAsync(command, snack);
    }

    public async Task UpdateSnackAsync(Snack snack)
    {
        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        const string command = "UPDATE Snacks SET Name = @Name, Price = @Price, Quantity = @Quantity WHERE Id = @Id";
        await connection.ExecuteAsync(command, snack);
    }

    public async Task DeleteSnackAsync(int id)
    {
        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        const string command = "DELETE FROM Snacks WHERE Id = @Id";
        await connection.ExecuteAsync(command, new {Id = id});
    }
}