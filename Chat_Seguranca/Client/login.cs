using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class login : Form
    {
        //para a comunicação com o server - definição de variaveis
        private const int PORT = 1000;
        NetworkStream networkStream;

        ProtocolSI protocolSI;
        TcpClient cliente;

        public login()
        {
            InitializeComponent();

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            cliente = new TcpClient();
            cliente.Connect(endpoint);
            networkStream = cliente.GetStream();

            protocolSI = new ProtocolSI();

        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            //variavel do username
            string username = textBox_username.Text;
            textBox_username.Clear();

            //conversão da variaveis num array de bytes o servidor a conseguir ler
            byte [] packet = protocolSI.Make(ProtocolSICmdType.DATA, username);

            networkStream.Write(packet, 0, packet.Length);

            //validação dos dados

            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }

        }
    }
}
