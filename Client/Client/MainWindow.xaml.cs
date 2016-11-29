using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void ShowTabBar() => spTab.Visibility = Visibility.Visible;
        public void HideTabBar() => spTab.Visibility = Visibility.Hidden;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CreateClockTask();

            var app = App.Current as App;
            app.Window = this;
            app.MainFrame = frmMain;
            GoToFirstPage();
        }

        /// <summary>
        /// Tworzy task do aktualizacji zegara.
        /// </summary>
        private async void CreateClockTask()
        {
            // WPF nie pozwala na operacje na kontrolkach w innych wątkach.
            // By to obejść korzystamy z klasy Progress. 
            var progress = new Progress<string>(s => lblDateAndTime.Content = s);
            await Task.Factory.StartNew(() => UpdateClock(progress), 
                TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Aktualizuje zegar.
        /// </summary>
        /// <param name="progress">Obiekt do aktualizacji</param>
        public static void UpdateClock(IProgress<string> progress)
        {
            while (true)
            {
                progress.Report(DateTime.Now.ToString("dd.MM.yy H:mm:ss"));
                Task.Delay(500).Wait();
            }
        }

        /// <summary>
        /// Przechodzi do pierwszej strony.
        /// </summary>
        private void GoToFirstPage()
        {
            frmMain.NavigationService.Navigate(new Login());
        }

        /// <summary>
        /// Obsluga eventu trzymania przycisku myszki.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void dpTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku minimalizacji okna aplikacji.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku zamkniecia aplikacji.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var app = App.Current as App;
            app.Client.Close();
            Close();
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku przegladania ogloszen.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnBrowseAds_Click(object sender, RoutedEventArgs e)
        {
            GoToBrowseAds();
        }

        /// <summary>
        /// Przechodzi do przegladania ogloszen poprzez wyslanie prosby o przegladanie.
        /// </summary>
        public void GoToBrowseAds()
        {
            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.Client.SendBrowseAdsPacket();
                ShowTabBar();
                btnBrowseAds.IsEnabled = false;
                btnAddAd.IsEnabled = true;
            });
        }

        /// <summary>
        /// Obsluga eventu klikniecia przycisku dodania ogloszenia.
        /// </summary>
        /// <param name="sender">Obiekt, ktory wyslal event.</param>
        /// <param name="e">Argumenty eventu</param>
        private void btnAddAd_Click(object sender, RoutedEventArgs e)
        {
            GoToAddAd();
        }

        /// <summary>
        /// Przechodzi do dodania ogloszenia.
        /// </summary>
        public void GoToAddAd()
        {
            frmMain.NavigationService.Navigate(new AddAd());
            ShowTabBar();
            btnBrowseAds.IsEnabled = true;
            btnAddAd.IsEnabled = false;
        }

        /// <summary>
        /// Przechodzi do przegladania ogloszenia poprzez wyslanie prosby o przegladanie.
        /// </summary>
        /// <param name="adId">Id ogloszenia do przegladania</param>
        public void GoToShowAd(long adId)
        {
            var app = App.Current as App;
            app.Client.SendShowAdPacket(adId);
        }

        /// <summary>
        /// Odblokowuje wszystkie zakladki.
        /// </summary>
        public void EnableAllTabs()
        {
            btnBrowseAds.IsEnabled = true;
            btnAddAd.IsEnabled = true;
        }
    }
}
