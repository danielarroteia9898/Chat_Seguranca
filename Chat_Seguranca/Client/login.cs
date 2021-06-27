using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{

    public partial class login : Form
    {
        public const int PORT = 10000;
        private const int NUMBER_OF_ITERATIONS = 1000;
        private const int SALTSIZE = 8;

        private RSACryptoServiceProvider rsa;

        public login()
        {
            InitializeComponent();
            textBoxUsernameR.Hide();
            textBoxPasswordR.Hide();
            textBoxName.Hide();
            button_registo.Hide();
            labelNick.Hide();
        }
        
        private void Register(string username, byte[] saltedPasswordHash, byte[] salt, string name, string chavePublica)
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
                SqlParameter paramUsername = new SqlParameter("@username", username);
                SqlParameter paramPassHash = new SqlParameter("@saltedPasswordHash", saltedPasswordHash);
                SqlParameter paramSalt = new SqlParameter("@salt", salt);
                SqlParameter paramName = new SqlParameter("@name", name);
                SqlParameter paramChavePublica = new SqlParameter("@chavePublica", chavePublica);


                // Declaração do comando SQL
                String sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt, Name, ChavePublica) VALUES (@username,@saltedPasswordHash,@salt,@name,@chavePublica)";

                // Prepara comando SQL para ser executado na Base de Dados
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Introduzir valores aos parâmentros registados no comando SQL
                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramPassHash);
                cmd.Parameters.Add(paramSalt);
                cmd.Parameters.Add(paramName);
                cmd.Parameters.Add(paramChavePublica);

                // Executar comando SQL
                int lines = cmd.ExecuteNonQuery();

                // Fechar ligação
                conn.Close();
                if (lines == 0)
                {
                    // Se forem devolvidas 0 linhas alteradas então o não foi executado com sucesso
                    throw new Exception("Error while inserting an user");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while inserting an user:" + e.Message);
            }
        }
        private bool VerifyLogin(string username, string password)
        {
            SqlConnection conn;
            try
            {
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

                if (!reader.HasRows)
                {
                    throw new Exception("Error while trying to access an user");
                }

                // Ler resultado da pesquisa
                reader.Read();

                // Obter Hash (password + salt)
                byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];

                // Obter salt
                byte[] saltStored = (byte[])reader["Salt"];

                conn.Close();
                
                byte[] hash = GenerateSaltedHash(password, saltStored);

                return saltedPasswordHashStored.SequenceEqual(hash);

                //TODO: verificar se a password na base de dados 
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
                return false;
            }
        }
        private void btn_submit_Click(object sender, EventArgs e)
        {
            string username = textBox_username.Text;
            string password = textBox_password.Text;
            
     
            if (VerifyLogin(username, password))
            {
                Form Chat = new Chat(username);
                Chat.Show();
                
               
                
            }
            else
            {
                MessageBox.Show("Apresenta Credênciais Erradas !! \nOu então não está Registado !!");
            } 

        }
        private static byte[] GenerateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[8];
            rng.GetBytes(buff);
            return buff;
        }


        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
            return rfc2898.GetBytes(32);
        }


        public bool CheckUsername(string username)
        {
            //Regex biblioteca de verificação de strings
            int x = new Regex("[A-Z]").Matches(username).Count;
            int y = new Regex("[0-9]").Matches(username).Count;
            int z = username.Length;
            if (x == 0 || y == 0 || z < 4 || z > 20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CheckPassword(string password)
        {
            int y = new Regex("[0-9]").Matches(password).Count;
            int z = password.Length;
            if (y == 0 || z < 4 || z > 20)
            {
                return false;
            }
            else
            {
                return true;
            }
           
        }
        private void button_registo_Click(object sender, EventArgs e)
        {
            string username = textBoxUsernameR.Text;
            string password = textBoxPasswordR.Text;
            string name = textBoxName.Text;


            byte[] salt = GenerateSalt(SALTSIZE);
            byte[] saltedPass = GenerateSaltedHash(password, salt);

            rsa = new RSACryptoServiceProvider();
            string chavePublica = rsa.ToXmlString(false);
            string bothkeys = rsa.ToXmlString(true);



            bool passwordVer = CheckPassword(password);
            bool usernameVer = CheckUsername(username);
            

            if (usernameVer == false)
            {
                MessageBox.Show(" - O Username contém entre 4 a 20 caracteres; \n - O Username contém pelo menos um numero; \n - O Username contém pelo menos uma letra maiúscula.", "Erro no Username");
            }
            else if (passwordVer == false)
            {
                MessageBox.Show(" - A Password contém entre 4 a 20 caracteres; \n - A Password contém pelo menos um numero.", "Erro na Password");
            }
            else
            {
                Register(username, saltedPass, salt, name, chavePublica);
                MessageBox.Show(chavePublica);
                MessageBox.Show("Registado com Sucesso!");
            }
            



        }

        private void buttonRegistoLogin_Click(object sender, EventArgs e)
        {
            if (textBox_username.Visible)
            {
                textBox_username.Hide();
                textBox_password.Hide();
                btn_submit.Hide();
                buttonRegistoLogin.Text = "Login";
                label_title.Text = "REGISTO";
                textBoxUsernameR.Show();
                textBoxPasswordR.Show();
                textBoxName.Show();
                labelNick.Show();
                button_registo.Show();
            }
            else
            {
                textBox_username.Show();
                textBox_password.Show();
                btn_submit.Show();
                buttonRegistoLogin.Text = "Registar";
                label_title.Text = "LOGIN";
                textBoxUsernameR.Hide();
                textBoxPasswordR.Hide();
                textBoxName.Hide();
                labelNick.Hide();
                button_registo.Hide();
            }
            
        }

        private void textBoxPasswordR_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
