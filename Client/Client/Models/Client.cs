using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace Client.Models
{
    /// <summary>
    /// Glowna czesc klienta
    /// </summary>
    public partial class Client
    {
        private const string HOST = "localhost";
        private const int PORT = 42069;

        private TcpClient _tcp;
        private Thread _thread;
        private StreamWriter _streamWriter;

        public Account Account { get; private set; }

        /// <summary>
        /// Destruktor
        /// </summary>
        ~Client()
        {
            Close();
        }

        /// <summary>
        /// Nawiazuje polaczenie z serwerem.
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                _tcp = new TcpClient();
                _tcp.Connect(HOST, PORT);
                _thread = new Thread(new ThreadStart(HandleConnection));

                _thread.Start();
            }
            catch (SocketException e)
            {
                Close();
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Zamyka wszystkie strumienie.
        /// </summary>
        public void Close()
        {
            _streamWriter?.Close();
            _tcp?.Close();

            _tcp = null;
            _thread = null;
            _streamWriter = null;
        }

        /// <summary>
        /// Sprawdza, czy klient polaczony jest z serwerem.
        /// </summary>
        /// <returns>True, jesli klient polaczony jest z serwerem</returns>
        public bool IsConnected()
        {
            if (_tcp == null)
                return false;
            return _tcp.Connected;
        }
    }
}
