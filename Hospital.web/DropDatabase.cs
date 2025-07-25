using System;
using System.Data.SqlClient;

class DropDatabase
{
    static void Main()
    {
        // Update YOUR_SERVER_NAME if needed
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand("DROP DATABASE HospitalDb", connection);
        command.ExecuteNonQuery();
        Console.WriteLine("Database 'HospitalDb' dropped successfully.");
    }
} 