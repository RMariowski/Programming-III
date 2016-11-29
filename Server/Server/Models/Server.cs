using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Models
{
    /// <summary>
    /// Sieciowa czesc serwera.
    /// </summary>
    public class Server
    {
        private TcpListener _listener;
        private Thread _listenThread;
        private List<Client> _clients;
        private Object _lockObj;

        public ServerApp App { get; private set; }
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="app">Instancja glownego obiektu aplikacji serwera</param>
        public Server(ServerApp app)
        {
            App = app;
            _listener = new TcpListener(IPAddress.Any, 42069);
            _listenThread = new Thread(new ThreadStart(ListenForClients));
            _clients = new List<Client>();
            _lockObj = new Object();
        }

        /// <summary>
        /// Zaczyna nasluchiwac polaczenia w oddzielnym watku.
        /// </summary>
        public void Listen()
        {
            IsRunning = true;
            _listener.Start();
            _listenThread.Start();
        }

        /// <summary>
        /// Nasluchuje w oczekiwaniu na polaczenie ze strony klienta.
        /// </summary>
        private void ListenForClients()
        {
            try
            {
                while (IsRunning)
                {
                    // Blokuje sie do czasu polaczenia ze strony klienta. 
                    var tcp = _listener.AcceptTcpClient();
                    var client = new Client(this, tcp);
                    Console.WriteLine($"Klient {client.IP}:{client.Port} polaczyl sie " +
                                      "z serwerem");
                    _clients.Add(client);
                }
            }
            catch (SocketException e)
            {
                // Interrupted wystepuje w przypadku gdy sprobuje 
                // sie zatrzymac blokowanie przy AcceptTcpClient().
                if (e.SocketErrorCode != SocketError.Interrupted)
                    Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Obsluguje rozlaczenie klienta.
        /// </summary>
        /// <param name="client">Klient</param>
        public void HandleClientDisconnection(Client client)
        {
            lock (_lockObj)
            {
                Console.WriteLine($"Klient {client.IP}:{client.Port} zakonczyl polaczenie");
                _clients.Remove(client);
            }
            
        }

        /// <summary>
        /// Wysyla pakiet z dodanym ogloszeniem do wszystkich klientow w celu 
        /// aktualizacji listy.
        /// </summary>
        /// <param name="ad">Ogloszenie do dodania</param>
        public void SendAddedAdPacketToAll(Ad ad)
        {
            foreach (var client in _clients)
            {
                if (client.IsLogged)
                    client.SendAddedAdPacket(ad);
            }
        }

        /// <summary>
        /// Wysyla pakiet z edycja ogloszenia do wszystkich klientow w celu 
        /// aktualizacji listy.
        /// </summary>
        /// <param name="ad">Ogloszenie do zedytowania</param>
        public void SendEditedAdPacketToAll(Ad ad)
        {
            foreach (var client in _clients)
            {
                if (client.IsLogged)
                    client.SendEditedAdPacket(ad);
            }
        }

        /// <summary>
        /// Wysyla pakiet z usunieciem ogloszenia do wszystkich klientow w celu 
        /// aktualizacji listy.
        /// </summary>
        /// <param name="ad">Ogloszenie do usuniecia</param>
        public void SendDeleteAdPacketToAll(Ad ad)
        {
            foreach (var client in _clients)
            {
                if (client.IsLogged)
                    client.SendDeleteAdPacket(ad);
            }
        }

        /// <summary>
        /// Przestaje nasluchiwac nowych polaczen.
        /// </summary>
        public void StopListen()
        {
            IsRunning = false;

            foreach (var client in _clients)
                client.Close();
            _clients.Clear();

            _listener.Stop();
        }
    }
}
