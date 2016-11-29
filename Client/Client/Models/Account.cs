namespace Client.Models
{
    /// <summary>
    /// Konto klienta
    /// </summary>
    public class Account
    {
        public enum AccType
        {
            ADMIN,
            NORMAL
        }

        public string Login { get; private set; }
        public AccType Type { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="type">Typ konta</param>
        public Account(string login, AccType type = AccType.NORMAL)
        {
            Login = login;
            Type = type;
        }
    }
}
