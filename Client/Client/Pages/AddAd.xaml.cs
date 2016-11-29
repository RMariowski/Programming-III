using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for AddAd.xaml
    /// </summary>
    public partial class AddAd : Page
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public AddAd()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku dodania ogloszenia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var app = App.Current as App;
            app.Client.SendAddAdPacket(tbTitle.Text, tbDescription.Text);
        }
    }
}
