using Server.Models;
using System.Text;

namespace Server
{
    /// <summary>
    /// Baze danych z kontami klientow.
    /// </summary>
    public class AccountDatabase : Database<Account>
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public AccountDatabase() 
            : base("accounts.json")
        {
        }

        /// <summary>
        /// Tworzy i dodaje konto do bazy danych.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="password">Haslo konta</param>
        /// <param name="email">Email konta</param>
        public void CreateAccount(string login, string password, string email)
        {
            var account = new Account(login, password, email);
            AddItem(account);
        }

        /// <summary>
        /// Szyfruje podane haslo.
        /// </summary>
        /// <param name="password">Haslo do zaszyfrowania</param>
        /// <returns>Zaszyfrowane haslo</returns>
        public string EncryptPassword(string password)
        {
            // Nic nadzwyczajnego - prosta sól. 
            var encryptedPassword = new StringBuilder(password.Substring(0, 2));
            encryptedPassword.Append(password);
            encryptedPassword.Append(password.Substring(0, 2));
            return encryptedPassword.ToString();
        }

        /// <summary>
        /// Sprawdza, czy konto o podanym loginie istnieje w bazie danych.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <returns></returns>
        public bool IsExists(string login)
        {
            return Items.Find(x => x.Login == login) != null;
        }

        /// <summary>
        /// Sprawdza, poprawnosc danych.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="password">Haslo konta</param>
        /// <returns>True, jesli dane sa prawidlowe.</returns>
        public bool IsValid(string login, string password)
        {
            int loginLen = login.Length;
            if (loginLen < 4 || loginLen > 16)
                return false;

            int passwordLen = password.Length;
            if (passwordLen < 4 || passwordLen > 32)
                return false;

            return true;
        }

        /// <summary>
        /// Pobiera konto z bazy danych, o podanym loginie.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <returns>Konto z bazy danych</returns>
        public Account GetAccount(string login)
        {
            return Items.Find(x => x.Login == login);
        }
    }
}
