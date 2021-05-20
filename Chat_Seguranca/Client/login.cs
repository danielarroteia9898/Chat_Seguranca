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
        public const int PORT = 10000;
        ProtocolSI protocolSI;
        TcpClient client;
        
        NetworkStream networkStream;
        public login()
        {
            InitializeComponent();
            
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            protocolSI = new ProtocolSI();
            client = new TcpClient();
            client.Connect(endPoint);
            networkStream = client.GetStream();

            
            string username = textBox_username.Text;

            //para guardar o utilizador
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, username);

            string password = textBox_password.Text;

            //para guardar a password
            byte[] pass = protocolSI.Make(ProtocolSICmdType.DATA, password);

            networkStream.Write(packet, 0, packet.Length);
            networkStream.Write(pass, 0, pass.Length);

           // Cliente nove_cliente = new Cliente(username, password);
            
            Chat form = new Chat();
            form.Show();
            

        }
    }
}
