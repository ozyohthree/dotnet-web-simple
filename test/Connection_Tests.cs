using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;

namespace web.test;
public class Connection_Tests
{
    private const string DbName = "TESTDB";
    private const string MasterConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";
    private const string TestDbConnectionString = $"Server=localhost,1433;Database={DbName};User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";

    private async Task EnsureDatabaseExists()
    {
        using var connection = new SqlConnection(MasterConnectionString);
        await connection.OpenAsync();

        var checkDbCommand = new SqlCommand($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName}", connection);
        await checkDbCommand.ExecuteNonQueryAsync();
    }

    [Fact(DisplayName = "Test Database Connection")]
    public async Task ConnectToMssqlAndSelectAll()
    {
        await EnsureDatabaseExists();

        using var connection = new SqlConnection(TestDbConnectionString);
        await connection.OpenAsync();

        // Create a table for testing
        using var createTableCmd = new SqlCommand("IF OBJECT_ID('dbo.Inventory', 'U') IS NULL CREATE TABLE dbo.Inventory (id INT, name NVARCHAR(50), quantity INT);", connection);
        await createTableCmd.ExecuteNonQueryAsync();

        // Insert some data
        using var insertCmd = new SqlCommand("INSERT INTO dbo.Inventory VALUES (1, 'banana', 150);", connection);
        await insertCmd.ExecuteNonQueryAsync();

        using var command = new SqlCommand("SELECT * FROM dbo.Inventory", connection);
        using var reader = await command.ExecuteReaderAsync();

        Assert.True(reader.HasRows, "The query should return rows.");
    }
}