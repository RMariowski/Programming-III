using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client.Models
{
    /// <summary>
    /// Czesc klienta odpowiedzialna za obsluge odebranych pakietow
    /// </summary>
    public partial class Client
    {
        /// <summary>
        /// Obsluguje polaczenie z serwerem. 
        /// </summary>
        private void HandleConnection()
        {
            var clientStream = _tcp.GetStream();
            var reader = new StreamReader(clientStream, Encoding.UTF8);
            _streamWriter = new StreamWriter(clientStream, Encoding.UTF8);

            while (IsConnected())
            {
                try
                {
                    string data = reader.ReadLine();

                    // W przypadku gwaltownego zakonczenia.
                    if (data == null || data.Length == 0)
                        throw new IOException();

                    HandlePacket(data);
                }
                catch (IOException e)
                {
                    Close();

                    var app = App.Current as App;
                    app.Invoke(() =>
                    {
                        app.MainFrame.NavigationService.Navigate(new Login());
                    });
                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            reader.Close();
        }

        /// <summary>
        /// Obsluguje odebrany pakiet parsujac jego id i wywolujac odpowiednie funkcje
        /// pod zparsowane id.
        /// </summary>
        /// <param name="data">String z danymi pakietu</param>
        private void HandlePacket(string data)
        {
            string strId = data.Substring(0, data.IndexOf('`'));
            var packetId = (PacketId)Enum.Parse(typeof(PacketId), strId);
            data = data.Substring(data.IndexOf('`') + 1);

            switch (packetId)
            {
                case PacketId.MSG:
                    HandleMessage(data);
                    break;

                case PacketId.REGISTER:
                    HandleRegister(data);
                    break;

                case PacketId.REGISTER_REFUSED:
                    HandleRegisterRefused();
                    break;

                case PacketId.LOGIN:
                    HandleLogin(data);
                    break;

                case PacketId.LOGIN_REFUSED:
                    HandleLoginRefused(data);
                    break;

                case PacketId.ADD_AD:
                    HandleAddAd();
                    break;

                case PacketId.ADDED_AD:
                    HandleAddedAd(data);
                    break;

                case PacketId.BROWSE_ADS:
                    HandleBrowseAds(data);
                    break;

                case PacketId.SHOW_AD:
                    HandleShowAd(data);
                    break;

                case PacketId.EDIT_AD:
                    HandleEditAd(data);
                    break;

                case PacketId.ACCEPT_EDIT_AD:
                    HandleAcceptEditAd();
                    break;

                case PacketId.EDITED_AD:
                    HandleEditedAd(data);
                    break;

                case PacketId.DELETE_AD:
                    HandleDeleteAd(data);
                    break;
            }
        }

        /// <summary>
        /// Oblsuguje pakiet z wiadomoscia.
        /// </summary>
        private void HandleMessage(string data)
        {
            MessageBox.Show(data);
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego utworzenia konta.
        /// </summary>
        /// /// <param name="data">Dane pakietu</param>
        private void HandleRegister(string data)
        {
            HandleMessage("Konto zostało pomyślnie utworzone");

            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.MainFrame.NavigationService.GoBack();
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z odmowa rejestracji.
        /// </summary>
        private void HandleRegisterRefused()
        {
            HandleMessage("Podano dane w niewłaściwym formacie lub " +
                          "konto o podanym loginie już istnieje.");
            _tcp.Close();
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego logowania.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleLogin(string data)
        {
            var successfulLoginData = JsonConvert.DeserializeObject<
                Dictionary<string, string>>(data);

            string login = successfulLoginData["Login"];
            var accountType = (Account.AccType)Enum.Parse(typeof(Account.AccType), 
                successfulLoginData["Type"]);

            Account = new Account(login, accountType);

            var app = App.Current as App;
            app.Window.GoToBrowseAds();
        }

        /// <summary>
        /// Oblsuguje pakiet z odmowa logowania.
        /// </summary>
        private void HandleLoginRefused(string data)
        {
            HandleMessage("Konto o podanym loginie nie istnieje lub hasło " +
                          "jest nieprawidłowe.");
            _tcp.Close();
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem przejscia do przegladania ogloszen.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleBrowseAds(string data)
        {
            var browseAdsData = JsonConvert.DeserializeObject<List<Ad>>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.MainFrame.NavigationService.Navigate(new BrowseAds(browseAdsData));
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego dodania ogloszenia.
        /// </summary>
        private void HandleAddAd()
        {
            HandleMessage("Ogłoszenie zostało dodane");

            var app = App.Current as App;
            app.Window.GoToBrowseAds();
        }

        /// <summary>
        /// Oblsuguje pakiet z wymuszeniem dodania ogloszenia do listy.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleAddedAd(string data)
        {
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                dynamic currentPage = app.MainFrame.NavigationService.Content;
                if (currentPage is BrowseAds)
                {
                    currentPage = currentPage as BrowseAds;
                    currentPage.HandleAddAd(ad);
                }
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego przejscia do ogladania ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleShowAd(string data)
        {
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.Window.EnableAllTabs();
                app.MainFrame.NavigationService.Navigate(new ShowAd(ad));
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego przejscia do edycji ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleEditAd(string data)
        {
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.Window.EnableAllTabs();
                app.MainFrame.NavigationService.Navigate(new EditAd(ad));
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego zedytowania ogloszenia.
        /// </summary>
        private void HandleAcceptEditAd()
        {
            HandleMessage("Ogłoszenie zostało zedytowane");

            var app = App.Current as App;
            app.Invoke(() =>
            {
                app.Window.GoToBrowseAds();
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z wymuszeniem edycji ogloszenia na liscie.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleEditedAd(string data)
        {
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                dynamic currentPage = app.MainFrame.NavigationService.Content;
                if (currentPage is BrowseAds)
                {
                    currentPage = currentPage as BrowseAds;
                    currentPage.HandleEditAd(ad);
                }
            });
        }

        /// <summary>
        /// Oblsuguje pakiet z potwierdzeniem pomyslnego usuniecia ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleDeleteAd(string data)
        {
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            var app = App.Current as App;
            app.Invoke(() =>
            {
                dynamic currentPage = app.MainFrame.NavigationService.Content;
                if (currentPage is BrowseAds)
                {
                    currentPage = currentPage as BrowseAds;
                    currentPage.HandleDeleteAd(ad);
                }
            });
        }
    }
}
