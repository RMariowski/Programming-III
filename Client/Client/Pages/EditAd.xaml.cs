using Client.Models;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for EditAd.xaml
    /// </summary>
    public partial class EditAd : Page
    {
        private Ad _ad;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ad">Ogloszenie do edycji</param>
        public EditAd(Ad ad)
        {
            InitializeComponent();

            _ad = ad;
            tbTitle.Text = ad.Title;
            tbDescription.Text = ad.Description;
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku zatwierdzenia edycji ogloszenia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            _ad.Title = tbTitle.Text;
            _ad.Description = tbDescription.Text;

            var app = App.Current as App;
            app.Client.SendAcceptEditAdPacket(_ad);
        }
    }
}
