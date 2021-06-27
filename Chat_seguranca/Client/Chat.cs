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
        private const int NUMBER_OF_ITERATIONS = 8;
        private const int PORT = 10000;
        

        NetworkStream networkStream;
        ProtocolSI protocolSI;
        TcpClient client;

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
            conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Danie\OneDrive\Ambiente de Trabalho\Chat_Seguranca\Chat_Seguranca\Client\Database.mdf';Integrated Security=True");

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
            ApresentarAmigos();
            ApresentarMensagens();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, PORT);
            client = new TcpClient();
            client.Connect(endpoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();
            SqlDataReader reader = LerBaseDados(username);
            //Obter Nickname
            string nick = (string)reader["Name"];
            labelNick.Text = nick;
            labelUsername.Text = username;
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
        private static byte[] CifrarDados(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
            return rfc2898.GetBytes(32);
        }

        private void EnviarMensagem(string msg)
        {
            byte[] chat = protocolSI.Make(ProtocolSICmdType.DATA, msg); //cria uma mensagem/pacote de um tipo específico
            networkStream.Write(chat, 0, chat.Length);
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
        }
        private void GuardarMensagem(string message, string username, DateTime time, byte[] mensagemCifrada)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Danie\OneDrive\Ambiente de Trabalho\Chat_Seguranca\Chat_Seguranca\Client\Database.mdf';Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração dos parâmetros do comando SQL
                SqlParameter paramMensagem = new SqlParameter("@MSG", message);
                SqlParameter paramMensagemCifrada = new SqlParameter("@Mensagem", mensagemCifrada);
                SqlParameter paramUsername = new SqlParameter("@username", username);
                SqlParameter paramTime = new SqlParameter("@time", time);


                // Declaração do comando SQL
                String sql = "INSERT INTO Mensagem (msg, username, time, mensagem) VALUES (@MSG,@username,@time,@Mensagem)";

                // Prepara comando SQL para ser executado na Base de Dados
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Introduzir valores aos parâmentros registados no comando SQL
                cmd.Parameters.Add(paramMensagem);
                cmd.Parameters.Add(paramMensagemCifrada);
                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramTime);
     
                // Executar comando SQL
                int lines = cmd.ExecuteNonQuery();

                // Fechar ligação
                conn.Close();
                if (lines == 0)
                {
                    // Se forem devolvidas 0 linhas alteradas então o não foi executado com sucesso
                    throw new Exception("Error while sending message");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while sending message:" + e.Message);
            }
        }


        // Método do botão enviar
        private void buttonSend_Click(object sender, EventArgs e)
        {
            string text = textBoxMessage.Text;
            SqlDataReader reader = LerBaseDados(labelUsername.Text);
            byte[] salt = (byte[])reader["Salt"];
            byte[] msgCifrada = CifrarDados(text, salt);
            EnviarMensagem(text);
            GuardarMensagem(text, labelUsername.Text, DateTime.Now, msgCifrada);
            textBoxMessage.Text = null;
            listBoxMensagens.Items.Clear();
            ApresentarMensagens();
        }


        // O envio de bits/bytes é trivial recorrendo ao ProtocolSI.

        //Método para fechar o Client
        private void CloseClient()
        {
            try
            {
                byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);

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
            string str = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Danie\OneDrive\Ambiente de Trabalho\Chat_Seguranca\Chat_Seguranca\Client\Database.mdf';Integrated Security=True");

            SqlConnection con = new SqlConnection(str);

            string query = "select * from Users";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader dbr;

            try

            {

                con.Open();

                dbr = cmd.ExecuteReader();

                while (dbr.Read())

                {

                    string sname = (string)dbr["username"] +"-   "+ (string) dbr["Name"];

                    listBoxAmigos.Items.Add(sname);

                }

            }

            catch (Exception es)

            {

                MessageBox.Show(es.Message);

            }

        }
        private void ApresentarMensagens()
        {
            string str = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Danie\OneDrive\Ambiente de Trabalho\Chat_Seguranca\Chat_Seguranca\Client\Database.mdf';Integrated Security=True");

            SqlConnection con = new SqlConnection(str);

            string query = "select * from Mensagem";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader dbr;

            try

            {

                con.Open();

                dbr = cmd.ExecuteReader();

                while (dbr.Read())

                {
                    string sname = (string)dbr["username"]+"("+dbr["time"] +")"+ " :: " + (string)dbr["MSG"];

                    listBoxMensagens.Items.Add(sname);

                }

            }

            catch (Exception es)

            {

                MessageBox.Show(es.Message);

            }

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
           // byte[] textoCifrado = CifrarDados(textoACifrar);
           // string text = Convert.ToBase64String(textoCifrado);

            string path = saveFileDialog_Cifrado.FileName;
            //File.WriteAllText(path, text);

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
            //string textoDecifrado = DecifrarTexto(textoCifrado);

            string path = saveFileDialog_decifrado.FileName;
            //File.WriteAllText(path, textoDecifrado);
        }

        private void listBoxMensagens_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
        
        
}

