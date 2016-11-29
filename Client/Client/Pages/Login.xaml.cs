using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Login()
        {
            InitializeComponent();

            var app = App.Current as App;
            app.Window.HideTabBar();
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku logowania.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var app = App.Current as App;
            var client = app.Client;

            // Rejestracja nie rozlacza polaczenia.
            if (!client.IsConnected())
                client.ConnectToServer();

            if (client.IsConnected())
                client.SendLoginPacket(tbLogin.Text, tbPassword.Password);
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku rejestracji.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var app = App.Current as App;
            app.MainFrame.NavigationService.Navigate(new Register());
        }
    }
}
