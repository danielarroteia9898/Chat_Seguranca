using EI.SI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        //lista de socket dos cliente - para geri-los
        private static List<Socket> clientesSocket = new List<Socket>();

        //buffer global (socket)
        private static byte[] _buffer = new byte[1024];

        //Criar novamente uma constante, tal como feito do lado do cliente.
        private const int PORT = 10000;

        //definição de um socket para haver comunição entre cliente - Socket universal para o servidor
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Lista de clientes com as chaves púbicas de cada um
        //passar as mensagens cifradas do cliente
        //
       

        static void Main(string[] args)
        {
            // Definição das variáveis na função principal.
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endpoint);

            
            // Iniciar o listener; apresentação da primeira mensagem na linha de comandos e inicialização do contador.
            listener.Start();
            Console.WriteLine("SERVER READY");
            int clientCounter = 0;


            //Criação do ciclo infinito de forma a que este esteja sempre em execução até ordem em contrário
            while (true)
            {
                // Definição da variável client do tipo TcpClient
                TcpClient client = listener.AcceptTcpClient();
                // Definição da variável clientHandler do tipo TcpClient
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }
        }


        //método para aceitar o cliente
        private static void AcceptCallBack(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            clientesSocket.Add(socket);
            //para aceitar a coneccao do cliente
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);

            //para receber a mensagem
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }


        //método para receber a mensagem
        private static void ReceiveCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            //dados recebidos
            int received = socket.EndReceive(AR);

            byte[] databuf = new byte[received];
            Array.Copy(_buffer, databuf, received);
            string text = Encoding.ASCII.GetString(databuf);
            Console.WriteLine("Texto recebido: " + text);



        }
    }


    class ClientHandler
    {
        // Definição das variáveis client e clientID.
        private TcpClient client;
        private int clientID;
        public ClientHandler(TcpClient client, int clientID)
        {
            this.client = client;
            this.clientID = clientID;   
        }
        public void Handle()
        {
            // Definição da variável thread e arranque da mesma
            // Para relembrar: Threads são unidades de execução dentro de um processo; um conjunto de instruções.
            Thread thread = new Thread(threadHandler);
            thread.Start();
        }
        private void threadHandler()
        {
            // Definição das variáveis networkStream e protocolSI
            NetworkStream networkStream = this.client.GetStream();
            ProtocolSI protocolSI = new ProtocolSI();
            
            // Ciclo a ser executado até ao fim da transmissão.
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                byte[] ack;

                // "Alteração"/mudança entre a apresentação da mensagem e o fim da tranmissão.
                switch (protocolSI.GetCmdType())
                {
                    //Dica do ALT
                    case ProtocolSICmdType.DATA:
                        Console.WriteLine( clientID + " : " + DateTime.Now.ToShortTimeString() +" "+ protocolSI.GetStringFromData());
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                        break;

                    case ProtocolSICmdType.EOT:
                        Console.WriteLine("Ending Thread from Client {0}", clientID);
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(ack, 0, ack.Length);
                        break;
                }
            }

            // Fecho do networkStream e do cliente (TcpClient)
            networkStream.Close();
            client.Close();
        }

        
    }
}