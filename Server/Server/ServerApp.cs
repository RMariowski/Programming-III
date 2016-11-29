using System;

namespace Server
{
    public class ServerApp
    {
        private Models.Server _server;
        private bool _exit = false;

        public AccountDatabase AccountDatabase { get; private set; }
        public AdDatabase AdDatabase { get; private set; }

        /// <summary>
        /// Punkt startowy aplikacji
        /// </summary>
        /// <param name="args">Argumenty aplikacji</param>
        static void Main(string[] args)
        {
            var server = new ServerApp();
            server.Run();
        }

        /// <summary>
        /// Uruchamia cala logike aplikacji.
        /// </summary>
        public void Run()
        {
            InitAccountDatabase();
            InitAdDatabase();
            InitServerListener();
            MainLoop();
            Exit();
        }

        /// <summary>
        /// Inicjalizuje baze kont.
        /// </summary>
        private void InitAccountDatabase()
        {
            AccountDatabase = new AccountDatabase();
            AccountDatabase.Load();
        }

        /// <summary>
        /// Inicjalizuje baze ogloszen.
        /// </summary>
        private void InitAdDatabase()
        {
            AdDatabase = new AdDatabase();
            AdDatabase.Load();
        }

        /// <summary>
        /// Inicjalizuje nasluchiwacza serwera.
        /// </summary>
        private void InitServerListener()
        {
            _server = new Models.Server(this);
            _server.Listen();
        }

        /// <summary>
        /// Rozpoczyna glowna petle aplikacji.
        /// </summary>
        private void MainLoop()
        {
            while (!_exit)
            {
                ProcessCommand();
            }
        }

        /// <summary>
        /// Przetwarza komende z wejscia.
        /// </summary>
        private void ProcessCommand()
        {
            var command = CommandParser.Parse(Console.ReadLine());
            if (command == null)
            {
                Console.WriteLine("Unknown command!");
                return;
            }
            command.Execute(this);
        }

        /// <summary>
        /// Ustawia zadanie zamkniecia aplikacji.
        /// </summary>
        public void RequestExit()
        {
            _exit = true;
        }

        /// <summary>
        /// Zatrzymuje i zwalnia wszelkie obiekty/zasoby.
        /// </summary>
        private void Exit()
        {
            _server.StopListen();
        }
    }
}
