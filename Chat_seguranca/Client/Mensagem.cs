using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Mensagem : Cliente
    {
        public string textMensagem { get; }

        public DateTime data_envio { get; }

        public Cliente cliente;

        public Mensagem(string textMensagem, DateTime data_envio, Cliente cliente)
        {
            this.textMensagem = textMensagem;
            this.data_envio = data_envio;
            this.cliente = cliente;
        }
    }
}
