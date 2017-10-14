using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartenPostgreSamples.Core.Users
{
    public class User : RootEntity
    {
        public User(string username)
        {
            Username = username;
        }

        public string Username { get; private set; }
        public bool IsAdmin { get; private set; }
        public Address Address { get; set; }

        public void MakeAdmin()
            => IsAdmin = true;

        public void MakeRegular()
            => IsAdmin = false;
    }
}
