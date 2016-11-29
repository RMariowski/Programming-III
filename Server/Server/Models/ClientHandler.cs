using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    /// <summary>
    /// Czesc klienta odpowiedzialna za obsluge odebranych pakietow
    /// </summary>
    public partial class Client
    {
        /// <summary>
        /// Obsluguje polaczenie klienta. 
        /// </summary>
        /// <param name="obj">Obiekt bedacy serwerem</param>
        private void HandleConnection(object obj)
        {
            _server = obj as Server;
            var clientStream = _tcp.GetStream();
            var reader = new StreamReader(clientStream, Encoding.UTF8);
            _streamWriter = new StreamWriter(clientStream, Encoding.UTF8);

            while (IsConnected)
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
                    IsConnected = false;

                    if (_server.IsRunning)
                        _server.HandleClientDisconnection(this);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            reader.Close();
            _streamWriter.Close();
            _tcp.Close();
        }

        /// <summary>
        /// Obsluguje odebrany pakiet parsujac jego id i wywolujac odpowiednie funkcje
        /// pod zparsowane id.
        /// </summary>
        /// <param name="data">String z danymi pakietu</param>
        private void HandlePacket(string data)
        {
            int index = data.IndexOf('`');
            
            // Sprawdza czy dane pakietu maja w sobie identyfikator, ktory jest wymagany.
            if (index < 1)
                return;

            string strId = data.Substring(0, index);
            var packetId = (PacketId)Enum.Parse(typeof(PacketId), strId);
            data = data.Substring(data.IndexOf('`') + 1);

            switch (packetId)
            {
                case PacketId.REGISTER:
                    HandleRegister(data);
                    break;

                case PacketId.LOGIN:
                    HandleLogin(data);
                    break;

                case PacketId.ADD_AD:
                    HandleAddAd(data);
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
                    HandleAcceptEditAd(data);
                    break;

                case PacketId.DELETE_AD:
                    HandleDeleteAd(data);
                    break;
            }
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi rejestracji.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleRegister(string data)
        {
            var accountDatabase = _server.App.AccountDatabase;
            var registerData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            string login = registerData["Login"];
            string password = accountDatabase.EncryptPassword(registerData["Password"]);
            string email = registerData["Email"];

            bool dataValid = true;
            if (!dataValid)
            {
                SendRegisterRefusedPacket();
                return;
            }

            bool accExists = accountDatabase.IsExists(login);
            if (accExists)
            {
                SendRegisterRefusedPacket();
                return;
            }

            accountDatabase.CreateAccount(login, password, email);
            SendRegisterPacket();
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi logowania.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleLogin(string data)
        {
            var accountDatabase = _server.App.AccountDatabase;
            var loginData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            string login = loginData["Login"];
            string password = accountDatabase.EncryptPassword(loginData["Password"]);

            bool accExists = accountDatabase.IsExists(login);
            if (!accExists)
            {
                SendLoginRefusedPacket();
                return;
            }

            bool accValid = accountDatabase.IsValid(login, password);
            if (!accValid)
            {
                SendLoginRefusedPacket();
                return;
            }

            Account = accountDatabase.GetAccount(login);

            SendLoginPacket();
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi dodania ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleAddAd(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            var addAdData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            string title = addAdData["Title"];
            string description = addAdData["Description"];

            var ad = adDatabase.AddAd(title, description, Account.Login);
            _server.SendAddedAdPacketToAll(ad);
            SendAdAddPacket();
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi przegladania ogloszen.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleBrowseAds(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            
            SendBrowseAdsPacket(adDatabase.Items);
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi pokazania ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleShowAd(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            var showAdData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (adDatabase.Items.Count < 1)
                return;

            var adId = Convert.ToInt64(showAdData["Id"]);
            var ad = adDatabase.Items.Where(x => x.Id == adId).First();
            if (ad == null)
                return;

            SendShowAdPacket(ad);
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi edycji ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleEditAd(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            var editAdData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (adDatabase.Items.Count < 1)
                return;

            var adId = Convert.ToInt64(editAdData["Id"]);
            var ad = adDatabase.Items.Where(x => x.Id == adId).First();
            if (ad == null)
                return;

            SendEditAdPacket(ad);
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi edycji ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleAcceptEditAd(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            var ad = JsonConvert.DeserializeObject<Ad>(data);

            adDatabase.EditAd(ad);
            _server.SendEditedAdPacketToAll(ad);
            SendAcceptEditAd();
        }

        /// <summary>
        /// Oblsuguje pakiet z danymi usuniecia ogloszenia.
        /// </summary>
        /// <param name="data">Dane pakietu</param>
        private void HandleDeleteAd(string data)
        {
            var adDatabase = _server.App.AdDatabase;
            var deleteAdData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            if (adDatabase.Items.Count < 1)
                return;

            var adId = Convert.ToInt64(deleteAdData["Id"]);
            var ad = adDatabase.Items.Where(x => x.Id == adId).First();
            if (ad == null)
                return;

            if (Account.Type != Account.AccType.ADMIN && Account.Login != ad.Author)
                return;

            adDatabase.DeleteAd(ad);
            _server.SendDeleteAdPacketToAll(ad);
        }
    }
}
