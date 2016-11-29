using Client.Models;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for ShowAd.xaml
    /// </summary>
    public partial class ShowAd : Page
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ad">Ogloszenie do pokazania</param>
        public ShowAd(Ad ad)
        {
            InitializeComponent();

            lblTitle.Content = ad.Title;
            lblAuthorAndCreateDate.Content = 
                "Dodano " + ad.CreateDate + " przez " + ad.Author;
            tbDescription.Text = ad.Description;
        }
    }
}
