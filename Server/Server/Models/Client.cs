using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Models
{
    /// <summary>
    /// Glowna czesc klienta
    /// </summary>
    public partial class Client
    {
        private Server _server;
        private TcpClient _tcp;
        private Thread _thread;
        private StreamWriter _streamWriter;
        
        public string IP => ((IPEndPoint)_tcp.Client.RemoteEndPoint).Address.ToString();
        public int Port => ((IPEndPoint)_tcp.Client.RemoteEndPoint).Port;
        public bool IsLogged => (Account != null);

        public Account Account { get; private set; }
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="server">Instancja serwera</param>
        /// <param name="tcp">Polaczenie TCP klienta</param>
        public Client(Server server, TcpClient tcp)
        {
            _tcp = tcp;
            _thread = new Thread(new ParameterizedThreadStart(HandleConnection));
            IsConnected = true;

            _thread.Start(server);
        }

        /// <summary>
        /// Destruktor
        /// </summary>
        ~Client()
        {
            Close();
        }

        /// <summary>
        /// Zamyka wszystkie strumienie.
        /// </summary>
        public void Close()
        {
            _streamWriter?.Close();
            _tcp?.Close();

            Account = null;
        }
    }
}
