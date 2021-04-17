using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Utilizador

    {
        string Username { get; set; }
        string Password { get; set; }

        public Utilizador(string username, string password)
        {
            Username = username;
            Password = password;

        }

        public override string ToString()
        {
            return "Username: " + Username + Password ;
        }
    }
}
