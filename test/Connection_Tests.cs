using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace web.test;
public class Connection_Tests
{
    /* In the mssql-container terminal, run these commands to set up the test DB and table:
    $ /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P P@ssword1 -q "select @@Version"
    > USE TABLE TESTDB;
    > CREATE TABLE dbo.Inventory (id INT,name NVARCHAR (50),quantity INT,PRIMARY KEY (id));
    > INSERT INTO dbo.Inventory VALUES (1, 'banana', 150); INSERT INTO dbo.Inventory VALUES (2, 'orange', 154);
    > GO
    > SELECT * FROM dbo.Inventory;
    > SELECT * FROM dbo.Inventory WHERE quantity > 151;
    > EXIT
    */
  
     [Fact(DisplayName = "Test Database Connection")]
    public void ConnectToMssqlAndSelectAll()
    {
        // Connection string for local SQL Server
        var connectionString = "Server=localhost,1433;Database=TESTDB;User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // Use INFORMATION_SCHEMA.TABLES for a generic SELECT *
        using var command = new SqlCommand("SELECT * FROM dbo.Inventory", connection);
        using var reader = command.ExecuteReader();

        // Assert that at least one row is returned
        Assert.True(reader.HasRows, "No tables found in the database.");
    }
    
}