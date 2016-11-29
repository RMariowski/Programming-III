namespace Server.Models
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
        public string Password { get; private set; }
        public string Email { get; private set; }
        public AccType Type { get; private set; }
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="password">Haslo konta</param>
        /// <param name="email">Email konta</param>
        /// <param name="type">Typ konta</param>
        public Account(string login, string password, string email, 
            AccType type = AccType.NORMAL)
        {
            Login = login;
            Password = password;
            Email = email;
            Type = type;
        }
    }
}
