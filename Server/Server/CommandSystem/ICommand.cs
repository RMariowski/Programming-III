namespace Server
{
    /// <summary>
    /// Interfejs komendy
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Wykonuje komende
        /// </summary>
        /// <param name="server">Instancja glownego obiektu aplikacji serwera</param>
        void Execute(ServerApp server);
    }
}