using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        //variaveis para ficheiros
        const string EncrFolder = @"c:\Encrypt\";
        const string DecrFolder = @"c:\Decrypt\";
        const string SrcFolder = @"c:\ChavePublica\";

        //var para guardar os ficheiro e abri-los

        private OpenFileDialog openFileDialog = new OpenFileDialog();


        public Chat()
        {
            InitializeComponent();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
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
        private string CifrarTexto(string txt)
        {
            //var para guardar as mensagens
            byte[] txtDecifrado = Encoding.UTF8.GetBytes(txt);

            //var para guardar as mensagens
            byte[] txtCifrado;

            //reserva de memória  para o texto a cifrar
            MemoryStream ms = new MemoryStream();

            //Iniciar o sistema de cifragem
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);

            //para cifrar os dados
            cs.Write(txtDecifrado, 0, txtDecifrado.Length);
            //cs.Close();

            //guardar os dados cifrados
            txtCifrado = ms.ToArray();

            //converter os bytes para texto
            string txtCifradoB64 = Convert.ToBase64String(txtCifrado);

            return txtCifradoB64;
        }

        //método para decifrar
        private string DecifrarTexto(string txtCifradoB64)
        {
            //var para guardar o texto cifrado
            byte[] txtCifrado = Convert.FromBase64String(txtCifradoB64);

            MemoryStream ms = new MemoryStream(txtCifrado);

            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);

            //var para guardar o texto decifrado
            byte[] txtDecifrado = new byte[ms.Length];

            int bytesLidos = 0;

            //decifrar os dados
            bytesLidos = cs.Read(txtDecifrado, 0, txtDecifrado.Length);
            cs.Close();

            string textoDecifrado = Encoding.UTF8.GetString(txtDecifrado, 0, bytesLidos);

            return textoDecifrado;

        }


        // Método do botão enviar
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //guardar a chave e vetor
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
            string text = GerarChavePrivada(textBoxMessage.Text);
            GerarIV(textBoxMessage.Text);


            MessageBox.Show(text);
            //networkStream.Write(key, 0, key.Length);
            // networkStream.Write(iv, 0, iv.Length);

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

            //codigo cifrar
           // string textoACifrar = textBoxMensagens.Text;

            //string textoCifrado = CifrarTexto(textoACifrar);

            // byte[] textCifrado = 

            //Console.WriteLine(textoCifrado);
            //enviar o teto cifrado por ficheiro
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

        private void chavePúblicaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //para criar a diretória
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //chaves
            key = aes.Key;
            iv = aes.IV;



            //var do texto a cifrar
            string textoACifrar = textBoxMessage.Text;
            //
            string textoCifrado = CifrarTexto(textoACifrar);

            openFileDialog1.InitialDirectory = SrcFolder;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                /*textoCifrado = openFileDialog1.FileName;
                if (textoCifrado != null)
                {

                    FileInfo fInfo = new FileInfo(textoCifrado);
                    // Pass the file name without the path.
                    string name = fInfo.FullName;
                    CifrarTexto(name);
                  */
                try
                {
                    if(textoCifrado != null)
                    {
                        FileStream fParameter = new FileStream(SrcFolder, FileMode.Create, FileAccess.Write);
                        StreamWriter m_WriterParameter = new StreamWriter(fParameter);
                        m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
                        m_WriterParameter.Write(textoCifrado);
                        m_WriterParameter.Flush();
                        m_WriterParameter.Close();
                    }
                }catch(Exception er)
                {
                    MessageBox.Show(er.ToString());
                }
                    
             }

        }
    }
        
        
}

