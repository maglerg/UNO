using System;
using Npgsql;


namespace ServerUNO.Database
{
    internal class Database
    {
        private readonly string connectionString =
            "Host=localhost;Port=5432;Database=UnoDB;Username=postgres;Password=losh20ad";
        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
