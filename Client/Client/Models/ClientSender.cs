using Newtonsoft.Json;
using System;

namespace Client.Models
{
    /// <summary>
    /// Czesc klienta odpowiedzialna za tworzenie i w szczegolnosci wysylanie pakietow.
    /// </summary>
    public partial class Client
    {
        /// <summary>
        /// Tworzy string pakietu dodajac identyfikator do opcjonalnego stringu danych.
        /// </summary>
        /// <param name="packetId">Identyfikator pakietu</param>
        /// <param name="data">String danych</param>
        /// <returns>String pakietu</returns>
        private string CreatePacket(PacketId packetId, string data = "")
        {
            data = data.Insert(0, Convert.ToString((byte)packetId) + "`");
            return data;
        }

        /// <summary>
        /// Wysyla string danych.
        /// </summary>
        /// <param name="data">String danych</param>
        private void SendData(string data)
        {
            _streamWriter.WriteLine(data);
            _streamWriter.Flush();
        }

        /// <summary>
        /// Wysyla pakiet z identyfikatorem i opcjonalnie danymi.
        /// </summary>
        /// <param name="packetId">Identyfikator pakietu</param>
        /// <param name="data">String danych</param>
        private void SendPacketWithId(PacketId packetId, string data = "")
        {
            string packet = CreatePacket(packetId, data);
            SendData(packet);
        }

        /// <summary>
        /// Wysyla pakiet z danymi rejestracji.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="password">Haslo konta</param>
        /// <param name="email">Email konta</param>
        public void SendRegisterPacket(string login, string password, string email)
        {
            var registerData = new
            {
                Login = login,
                Password = password,
                Email = email
            };

            string json = JsonConvert.SerializeObject(registerData);
            SendPacketWithId(PacketId.REGISTER, json);
        }

        /// <summary>
        /// Wysyla pakiet z danymi logowania.
        /// </summary>
        /// <param name="login">Login konta</param>
        /// <param name="password">Haslo konta</param>
        public void SendLoginPacket(string login, string password)
        {
            var loginData = new
            {
                Login = login,
                Password = password
            };

            string json = JsonConvert.SerializeObject(loginData);
            SendPacketWithId(PacketId.LOGIN, json);
        }

        /// <summary>
        /// Wysyla pakiet z danymi ogloszenia do dodania.
        /// </summary>
        /// <param name="title">Tytul ogloszenia</param>
        /// <param name="description">Opis ogloszenia</param>
        public void SendAddAdPacket(string title, string description)
        {
            var addAdData = new
            {
                Title = title,
                Description = description
            };

            string json = JsonConvert.SerializeObject(addAdData);
            SendPacketWithId(PacketId.ADD_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z prosba o przejscie do przegladania ogloszen z ich aktualna lista.
        /// </summary>
        public void SendBrowseAdsPacket()
        {
            SendPacketWithId(PacketId.BROWSE_ADS);
        }

        /// <summary>
        /// Wysyla pakiet z prosba o zobaczenie ogloszenia.
        /// </summary>
        /// <param name="id">Id ogloszenia do zobaczenia.</param>
        public void SendShowAdPacket(long id)
        {
            var showAdDate = new
            {
                Id = id
            };

            string json = JsonConvert.SerializeObject(showAdDate);
            SendPacketWithId(PacketId.SHOW_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z prosba o edycje ogloszenia.
        /// </summary>
        /// <param name="id">Id ogloszenia do edycji.</param>
        public void SendEditAdPacket(long id)
        {
            var editAdDate = new
            {
                Id = id
            };

            string json = JsonConvert.SerializeObject(editAdDate);
            SendPacketWithId(PacketId.EDIT_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z prosba o akceptacje edycji ogloszenia. 
        /// </summary>
        /// <param name="ad">Zedytowane ogloszenie</param>
        public void SendAcceptEditAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.ACCEPT_EDIT_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z prosba o usuniecie ogloszenia.
        /// </summary>
        /// <param name="id">Id ogloszenia do usuniecia.</param>
        public void SendDeleteAdPacket(long id)
        {
            var deleteAdDate = new
            {
                Id = id
            };

            string json = JsonConvert.SerializeObject(deleteAdDate);
            SendPacketWithId(PacketId.DELETE_AD, json);
        }
    }
}
