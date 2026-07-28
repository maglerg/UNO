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
        public string Password { get; set; }
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Points { get; set; }
        public int AvatarId { get; set; }
        public string Status { get; set; }
        public UserAccount(Guid id, string username, string password, string name, int avatarId, string status)
        {
            Id = id;
            Username = username;
            Password = password;
            Name = name;
            Wins = 0;
            Points = 0;
            AvatarId = avatarId;
            Status = status;
        }
        
    }
}
