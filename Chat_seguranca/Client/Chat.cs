using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Chat : Form
    {
        private const int PORT = 10000;
        NetworkStream networkStream;
        ProtocolSI protocolSI;
        TcpClient client;

        //para cifrar/decifrar as mensagens
        private byte[] key;
        private byte[] iv;
        AesCryptoServiceProvider aes;

        public Chat()
        {
            InitializeComponent();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            //lient = Cliente();
            client = new TcpClient();
            client.Connect(endpoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();

        }

        //gerar a chave simétrica
        private string GerarChavePrivada(string pass)
        {
            byte[] salt = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(pass, salt, 1000);

            //gerar a chave
            byte[] key = pwdGen.GetBytes(16);

            string passB64 = Convert.ToBase64String(key);
           
            return passB64;
        }


        //metodo para  vetor de inicialização
        private string GerarIV(string pass)
        {
            byte[] salt = new byte[] { 7, 6, 5, 4, 3, 2, 1, 0 };
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(pass, salt, 1000);

            //gerar o vetor
            byte[] iv = pwdGen.GetBytes(16);
            string ivB64 = Convert.ToBase64String(iv);

            return ivB64;
        }

        //metodo para cifrar a mensagem
        private string CifrarTexto (string txt)
        {
            //var para guardar as mensagens
            byte[] txtDecifrado = Encoding.UTF8.GetBytes(txt);

            //var para guardar as mensagens
        }

        // Método do botão enviar
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //guardar a chave e vetor
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;

            string keyB64 = Convert.ToBase64String(key);
            string ivB64 = Convert.ToBase64String(iv);
            //GerarChavePrivada();
            //GerarIV();

            networkStream.Write(key, 0, key.Length);
            networkStream.Write(iv, 0, iv.Length);

            //código para enviar mensagem
            string msg = textBoxMessage.Text;
            textBoxMessage.Clear();
            byte[] chat = protocolSI.Make(ProtocolSICmdType.DATA, msg); //cria uma mensagem/pacote de um tipo específico
            networkStream.Write(chat, 0, chat.Length);
            textBoxMensagens.AppendText(msg);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
        }


        // O envio de bits/bytes é trivial recorrendo ao ProtocolSI.

        //Método para fechar o Client
        private void CloseClient()
        {
            try
            {
                // Definição da variável eot (End of Transmission) do tipo array de byte.
                // Utilização do método Make. ProtocolSICmdType serve para enviar dados
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);

                // A classe NetworkStream disponibiliza métodos para enviar/receber dados através de socket Stream
                // O Socket de rede é um endpoint interno para envio e recepção de dados com um nó/computador presente na rede.
                networkStream.Write(eot, 0, eot.Length);
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                networkStream.Close();
                client.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Método para fechar o formulário
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Chamar a função para fechar o Client
            CloseClient();
        }

        //Método para o Botão para sair
        private void buttonQuit_Click(object sender, EventArgs e)
        {
            // Chamar a função para fechar o Client e associar a este próprio botão
            CloseClient();
            this.Close();
        }
        //Método para apresentar lista de amigos
        private void ApresentarAmigos()
        {

        }

        private void textBoxMensagens_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
