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

        public Utilizador(string username)
        {
            Username = username;

        }

        public override string ToString()
        {
            return base.ToString() + Username;
        }
    }
}
