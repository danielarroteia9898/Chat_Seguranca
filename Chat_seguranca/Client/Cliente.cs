using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    class Cliente
    {
        public string Username { get; }

        public string Password { get; }

        public List<Cliente> ListaAmigos { get; set; }
        
        public List<Mensagem> ListaMensagens { get; set; }

        public Cliente(string username, string password, List<Cliente> listaAmigos, List<Mensagem> listaMensagens)
        {
            Username = username;
            Password = password;
            ListaAmigos = new List<Cliente>();
            ListaMensagens = new List<Mensagem>();
        }
    }
}
