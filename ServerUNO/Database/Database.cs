using System;
using Npgsql;


namespace ServerUNO.Database
{
    internal class Database
    {
        
            private readonly string _connectionString;

            public Database(string connectionString)
            {
                _connectionString = connectionString;
            }

            public NpgsqlConnection GetConnection() => new NpgsqlConnection(_connectionString);
        
    }
}
