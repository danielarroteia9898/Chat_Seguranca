using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Mensagem
    {
        public string textMensagem { get; }

        public DateTime data_envio { get; }

        public Cliente cliente;
    }
}
