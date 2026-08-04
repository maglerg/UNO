using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUNO
{
    public class UserAccount
    {
        public Guid Id { get; private set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }

        public int AvatarId { get; set; }

        public int Wins { get; set; }

        public int CountGames { get; set; }

        public UserAccount(string username, string passwordHash, string name, int avatarId)
        {
            Id = Guid.NewGuid();

            Username = username;
            PasswordHash = passwordHash;

            Name = name;
            Status = "";
            AvatarId = avatarId;

            Wins = 0;
            CountGames = 0;
        }

        public UserAccount(
            Guid id,
            string username,
            string passwordHash,
            string name,
            string status,
            int avatarId,
            int wins,
            int countGames)
        {
            Id = id;

            Username = username;
            PasswordHash = passwordHash;

            Name = name;
            Status = status;

            AvatarId = avatarId;

            Wins = wins;
            CountGames = countGames;
        }
    }
}
