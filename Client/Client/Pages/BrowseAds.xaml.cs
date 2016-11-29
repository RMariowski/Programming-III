using Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for BrowseAds.xaml
    /// </summary>
    public partial class BrowseAds : Page
    {
        private GridViewColumnHeader _sortColumn = null;
        private SortAdorner _sortAdorner = null;
        private List<Ad> _ads;
        private Ad _selectedAd;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ads">Lista z ogloszeniami</param>
        public BrowseAds(List<Ad> ads)
        {
            InitializeComponent();

            btnEditAd.Visibility = Visibility.Hidden;
            btnDeleteAd.Visibility = Visibility.Hidden;

            _ads = ads;
            AddAllAdsFromList();
        }

        /// <summary>
        /// Dodaje wszystkie ogloszenia do listview
        /// </summary>
        private void AddAllAdsFromList()
        {
            lvAds.Items.Clear();
            foreach (var ad in _ads)
                lvAds.Items.Add(ad);
        }

        /// <summary>
        /// Obsluga eventu klikniecia kolumny listview.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var column = sender as GridViewColumnHeader;
            string sortBy = column.Tag.ToString();
            if (_sortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(_sortColumn).Remove(_sortAdorner);
                lvAds.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;
            if (_sortColumn == column && _sortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _sortColumn = column;
            _sortAdorner = new SortAdorner(_sortColumn, newDir);
            AdornerLayer.GetAdornerLayer(_sortColumn).Add(_sortAdorner);
            lvAds.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku filtrowania ogloszen.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

            DateTime? createDate = null;
            if (dpCreateDate.SelectedDate != null)
                createDate = dpCreateDate.SelectedDate.Value.Date;

            string author = tbAuthor.Text;
            string title = tbTitle.Text;

            if (createDate == null && author == "" && title == "")
            {
                AddAllAdsFromList();
                return;
            }

            Filter(createDate, author, title);
        }

        /// <summary>
        /// Filtruje ogloszenia
        /// </summary>
        /// <param name="createDate">Data dodania ogloszenia</param>
        /// <param name="author">Autor ogloszenia</param>
        /// <param name="title">Tytul ogloszenia</param>
        private void Filter(DateTime? createDate, string author, string title)
        {
            lvAds.Items.Clear();
            foreach (var ad in _ads)
            {
                bool found = false;

                if (createDate != null)
                {
                    if (createDate == ad.CreateDate.Date)
                    {
                        found = true;
                    }
                    else
                        continue;
                }


                if (author != "")
                {
                    if (ad.Author.ToLower().IndexOf(author.ToLower()) > -1)
                    {
                        found = true;
                    }
                    else
                        continue;
                }

                if (title != "")
                {
                    if (ad.Title.ToLower().IndexOf(title.ToLower()) > -1)
                    {
                        found = true;
                    }
                    else
                        continue;
                }

                if (found)
                    lvAds.Items.Add(ad);
            }
        }

        /// <summary>
        /// Obsluga eventu zaznaczenia wiersza z listview.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void lvAds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedAd = lvAds.SelectedItem as Ad;

            var app = App.Current as App;
            var account = app.Client.Account;

            if (_selectedAd == null || 
                (account.Type != Account.AccType.ADMIN && _selectedAd.Author != account.Login))
            {
                btnEditAd.Visibility = Visibility.Hidden;
                btnDeleteAd.Visibility = Visibility.Hidden;
            }
            else
            {
                btnEditAd.Visibility = Visibility.Visible;
                btnDeleteAd.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Obsluga eventu podwojnego klikniecia na wiersz z listview.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void lvAds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selectedAd == null)
                return;

            var app = App.Current as App;
            app.Window.GoToShowAd(_selectedAd.Id);
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku edycji ogloszenia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnEditAd_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAd == null)
                return;

            var app = App.Current as App;
            var client = app.Client;
            client.SendEditAdPacket(_selectedAd.Id);
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku usuniecia ogloszenia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnDeleteAd_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAd == null)
                return;

            var app = App.Current as App;
            var client = app.Client;
            client.SendDeleteAdPacket(_selectedAd.Id);
        }

        /// <summary>
        /// Obsluguje edycje ogloszenia.
        /// </summary>
        /// <param name="ad">Ogloszenie do zedytowania</param>
        public void HandleAddAd(Ad ad)
        {
            _ads.Add(ad);
            AddAllAdsFromList();

            lvAds.SelectedItem = null;
            lvAds_SelectionChanged(null, null);
        }

        /// <summary>
        /// Obsluguje edycje ogloszenia.
        /// </summary>
        /// <param name="ad">Ogloszenie do zedytowania</param>
        public void HandleEditAd(Ad ad)
        {
            if (_ads.Count < 1)
                return;

            var adInList = _ads.Where(x => x.Id == ad.Id).First();
            int index = _ads.IndexOf(adInList);
            _ads[index] = ad;
            AddAllAdsFromList();

            lvAds.SelectedItem = null;
            lvAds_SelectionChanged(null, null);
        }

        /// <summary>
        /// Obsluguje usuniecie ogloszenia.
        /// </summary>
        /// <param name="ad">Ogloszenie do usuniecia</param>
        public void HandleDeleteAd(Ad ad)
        {
            if (_ads.Count < 1)
                return;

            var adInList = _ads.Where(x => x.Id == ad.Id).First();
            _ads.Remove(adInList);
            AddAllAdsFromList();

            lvAds.SelectedItem = null;
            lvAds_SelectionChanged(null, null);
        }
    }
}
