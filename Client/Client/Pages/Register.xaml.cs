using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public Register()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku rejestracji.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnRegister_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var app = App.Current as App;
            var client = app.Client;
            client.ConnectToServer();

            if (client.IsConnected())
                client.SendRegisterPacket(tbLogin.Text, tbPassword.Password, tbEmail.Text);
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku cofniecia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// Cofa sie do poprzedniej strony.
        /// </summary>
        private void GoBack()
        {
            var app = App.Current as App;
            app.MainFrame.NavigationService.GoBack();
        }
    }
}
