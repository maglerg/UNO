using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerUNO
{
    public class AccountManager
    {
        public List<UserAccount> _users = new();
        private readonly string _filePath = "accounts.json";

        public AccountManager()
        {
            if (!File.Exists(_filePath))
                return;

            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                _users = new List<UserAccount>();
                return;
            }

            _users = JsonSerializer.Deserialize<List<UserAccount>>(json)
                     ?? new List<UserAccount>();

        }
        public void Add(UserAccount user)
        {
            _users.Add(user);
        }
        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                _users = new List<UserAccount>();
                return;
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                _users = new List<UserAccount>();
                return;
            }
            _users = JsonSerializer.Deserialize<List<UserAccount>>(json)
                     ?? new List<UserAccount>();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions
            {
                WriteIndented = true 
            });

            File.WriteAllText(_filePath, json);
        }

        public UserAccount? Find(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);
        }
        public UserAccount Get(UserAccount account)
        {
            return _users.FirstOrDefault(u => ReferenceEquals(u, account));
        }
        public bool UsernameExists(string username, Guid exceptId)
        {
            return _users.Any(u =>
                u.Username == username &&
                u.Id != exceptId);
        }
        public bool Register(UserAccount user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return false;

            _users.Add(user);
            Save();
            return true;
        }
    }
}
