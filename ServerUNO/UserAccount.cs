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
        public int Wins { get; set; }
        public int Points { get; set; }
        public UserAccount(Guid id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
            Wins = 0;
            Points = 0;
        }
    }
}
