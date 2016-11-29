namespace Server
{
    /// <summary>
    /// Komenda wyjscia
    /// </summary>
    public class ExitCommand : ICommand
    {
        /// <summary>
        /// Wykonuje komende wyjscia
        /// </summary>
        /// <param name="server">Instancja glownego obiektu aplikacji serwera</param>
        public void Execute(ServerApp server)
        {
            server.RequestExit();
        }
    }
}