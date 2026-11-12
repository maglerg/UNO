using Npgsql;

namespace ServerUNO
{
    public class AccountManager
    {
        private readonly string _connectionString;

        public AccountManager(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public void Add(UserAccount user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
    INSERT INTO Users
    (Id, Username, PasswordHash, Name, Status, AvatarId, Wins, CountGames)
    VALUES
    (@Id, @Username, @PasswordHash, @Name, @Status, @AvatarId, @Wins, @CountGames);";

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("Id", user.Id);
            command.Parameters.AddWithValue("Username", user.Username);
            command.Parameters.AddWithValue("PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("Name", user.Name);
            command.Parameters.AddWithValue("Status", user.Status);
            command.Parameters.AddWithValue("AvatarId", user.AvatarId);
            command.Parameters.AddWithValue("Wins", user.Wins);
            command.Parameters.AddWithValue("CountGames", user.CountGames);

            command.ExecuteNonQuery();

        }
        public UserAccount? Find(string username, string passwordHash)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
    SELECT Id, Username, PasswordHash, Name, Status,
           AvatarId, Wins, CountGames
    FROM Users
    WHERE Username = @Username
      AND PasswordHash = @PasswordHash;";

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("Username", username);
            command.Parameters.AddWithValue("PasswordHash", passwordHash);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new UserAccount(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetInt32(5),
                reader.GetInt32(6),
                reader.GetInt32(7));
        }

        public UserAccount? Get(UserAccount account)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
    SELECT Id, Username, PasswordHash, Name, Status,
           AvatarId, Wins, CountGames
    FROM Users
    WHERE Id = @Id;";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("Id", account.Id);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new UserAccount(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetInt32(5),
                reader.GetInt32(6),
                reader.GetInt32(7));
        }

        public bool UsernameExists(string username, Guid exceptId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Username = @Username
                  AND Id <> @Id;";

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("Username", username);
            command.Parameters.AddWithValue("Id", exceptId);

            return (long)command.ExecuteScalar() > 0;
        }

        public bool Register(UserAccount user)
        {
            if (UsernameExists(user.Username, Guid.Empty))
                return false;

            Add(user);
            return true;
        }
    }
}