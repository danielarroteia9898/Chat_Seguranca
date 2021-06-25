using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        private const int NUMBER_OF_ITERATIONS = 1000;
        private const int PORT = 10000;
        NetworkStream networkStream;
        ProtocolSI protocolSI;
        TcpClient client;
        private string Username;


        //para cifrar/decifrar as mensagens
        private byte[] key;
        private byte[] iv;
        AesCryptoServiceProvider aes;
        private SqlDataReader LerBaseDados(string username)
        {
            // Abrir ligação à Base de Dados
            SqlConnection conn;
            // Configurar ligação à Base de Dados
            conn = new SqlConnection();
            conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='E:\IPLeiria\1ano\2º semestre\topicos de segurança\projeto\reposito_projeto\Chat_Seguranca\Chat_Seguranca\cliente\Database.mdf';Integrated Security=True");

            // Abrir ligação à Base de Dados
            conn.Open();

            // Declaração do comando SQL
            String sql = "SELECT * FROM Users WHERE Username = @username";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;

            // Declaração dos parâmetros do comando SQL
            SqlParameter param = new SqlParameter("@username", username);

            // Introduzir valor ao parâmentro registado no comando SQL
            cmd.Parameters.Add(param);

            // Associar ligação à Base de Dados ao comando a ser executado
            cmd.Connection = conn;

            // Executar comando SQL
            SqlDataReader reader = cmd.ExecuteReader();
            // Ler resultado da pesquisa
            reader.Read();
            return reader;
        }

        public Chat(string username)
        {
            InitializeComponent();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            client = new TcpClient();
            client.Connect(endpoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();
            SqlDataReader reader = LerBaseDados(username);
            labelUsername.Text = username;
            //Obter Nickname
            string nick = (string)reader["Name"];
            labelNick.Text = nick;
        }
        //gerar a chave simétrica
        private string GerarChavePrivada(string pass, byte[] salt)
        {
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(pass, salt, NUMBER_OF_ITERATIONS);

            //gerar a chave
            byte[] key = pwdGen.GetBytes(16);

            string passB64 = Convert.ToBase64String(key);

            return passB64;
        }


        //metodo para  vetor de inicialização
        private string GerarIV(string pass, byte[] salt)
        {
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(pass, salt, NUMBER_OF_ITERATIONS);

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

        private void EnviarMensagem(string msg)
        {
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
            byte[] chat = protocolSI.Make(ProtocolSICmdType.DATA, msg); //cria uma mensagem/pacote de um tipo específico
            networkStream.Write(chat, 0, chat.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
        }
        // Método do botão enviar
        private void buttonSend_Click(object sender, EventArgs e)
        {
            SqlDataReader reader = LerBaseDados(labelUsername.Text);
            byte[] salt = (byte[])reader["Salt"];
            //guardar a chave e vetor
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
            string text = GerarChavePrivada(textBoxMessage.Text, salt);
            GerarIV(textBoxMessage.Text, salt);


            MessageBox.Show(text);
            //networkStream.Write(key, 0, key.Length);
            // networkStream.Write(iv, 0, iv.Length);

            //código para enviar mensagem
            EnviarMensagem(text);

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

            saveFileDialog_chavePublica.Title = "Export public key";
            saveFileDialog_chavePublica.DefaultExt = "txt";

            saveFileDialog_chavePublica.Filter = "Text files (*.txt)|*.txt";

            if (! Directory.Exists("c:\\ChavePublica\\"))
            {
                Directory.CreateDirectory("c:\\ChavePublica\\");
            }
            

            saveFileDialog_chavePublica.ShowDialog();

        }

        private void mensagensCifradasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog_Cifrado.Title = "Export texto cifrado";
            saveFileDialog_Cifrado.DefaultExt = "txt";

            saveFileDialog_Cifrado.Filter = "Text files (*.txt)|*.txt";

            if (!Directory.Exists("c:\\Encrypt\\"))
            {
                Directory.CreateDirectory("c:\\Encrypt\\");
            }


            saveFileDialog_Cifrado.ShowDialog();
        }


        private void saveFileDialog_chavePublica_FileOk(object sender, CancelEventArgs e)
        {
            byte[] salt = { 1, 2, 3, 4, 5 };
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
            string text = GerarChavePrivada(textBoxMessage.Text, salt);
            GerarIV(textBoxMessage.Text, salt);

         
            string path = saveFileDialog_chavePublica.FileName;
            File.WriteAllText(path, text);

        }
        private void saveFileDialog_Cifrado_FileOk(object sender, CancelEventArgs e)
        {
            //IR BUSCAR CHAVE E IV
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;


            //IR BUSCAR O TEXTO DA TEXTBOX TextoACifrar
            string textoACifrar = textBoxMessage.Text;

            //CHAMAR A FUNÇÃO CifrarTexto E ENVIAR O TEXTO GUARDADO ANTES E GUARDÁ-LO NA VARÍAVEL TEXTOCIFRADO
            string textoCifrado = CifrarTexto(textoACifrar);

            string path = saveFileDialog_Cifrado.FileName;
            File.WriteAllText(path, textoCifrado);

        }

        private void saveFileDialog_decifrado_FileOk(object sender, CancelEventArgs e)
        {
            //IR BUSCAR CHAVE E IV
             aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
           

            //IR BUSCAR O TEXTO DA TEXTBOX TextoACifrar
            string textoCifrado = textBoxMessage.Text;

            //CHAMAR A FUNÇÃO CifrarTexto E ENVIAR O TEXTO GUARDADO ANTES E GUARDÁ-LO NA VARÍAVEL TEXTOCIFRADO
            string textoDecifrado = DecifrarTexto(textoCifrado);

            string path = saveFileDialog_decifrado.FileName;
            File.WriteAllText(path, textoDecifrado);
        }

       
    }
        
        
}

