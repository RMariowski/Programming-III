using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Server.Models
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
        /// Wysyla pakiet z wiadomoscia pokazywana po stronie klienta jako msg box.
        /// </summary>
        /// <param name="msg">Tresc wiadomosci</param>
        private void SendMsgPacket(string msg)
        {
            SendPacketWithId(PacketId.MSG, msg);
        }

        /// <summary>
        /// Wysyla pakiet z potwierdzeniem utworzenia konta.
        /// </summary>
        private void SendRegisterPacket()
        {
            SendPacketWithId(PacketId.REGISTER);
        }

        /// <summary>
        /// Wysyla pakiet z odmowa utworzenia konta.
        /// </summary>
        private void SendRegisterRefusedPacket()
        {
            SendPacketWithId(PacketId.REGISTER_REFUSED);
        }

        /// <summary>
        /// Wysyla pakiet z potwierdzeniem logowania.
        /// </summary>
        private void SendLoginPacket()
        {
            var successfulLoginData = new
            {
                Login = Account.Login,
                Type = (byte)Account.Type
            };

            string json = JsonConvert.SerializeObject(successfulLoginData);
            SendPacketWithId(PacketId.LOGIN, json);
        }

        /// <summary>
        /// Wysyla pakiet z odmowa zalogowania.
        /// </summary>
        private void SendLoginRefusedPacket()
        {
            SendPacketWithId(PacketId.LOGIN_REFUSED);;
        }

        /// <summary>
        /// Wysyla pakiet z potwierdzeniem dodania ogloszenia.
        /// </summary>
        private void SendAdAddPacket()
        {
            SendPacketWithId(PacketId.ADD_AD);
        }

        /// <summary>
        /// Wysyla pakiet z wymuszeniem dodania ogloszenia na liscie.
        /// </summary>
        /// <param name="ad">Dodane ogloszenie</param>
        public void SendAddedAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.ADDED_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z lista wszystkich ogloszen. 
        /// Klient przechodzi do przegladania ogloszen.
        /// </summary>
        /// <param name="ads">Lista ogloszen</param>
        private void SendBrowseAdsPacket(List<Ad> ads)
        {
            string json = JsonConvert.SerializeObject(ads);
            SendPacketWithId(PacketId.BROWSE_ADS, json);
        }

        /// <summary>
        /// Wysyla pakiet z danymi ogloszenia do pokazania.
        /// </summary>
        /// <param name="ad">Ogloszenie do pokazaznia</param>
        private void SendShowAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.SHOW_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z danymi ogloszenia do edycji.
        /// </summary>
        /// <param name="ad">Ogloszenie do edycji</param>
        private void SendEditAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.EDIT_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z potwierdzeniem edycji ogloszenia.
        /// </summary>
        private void SendAcceptEditAd()
        {
            SendPacketWithId(PacketId.ACCEPT_EDIT_AD);
        }

        /// <summary>
        /// Wysyla pakiet z wymuszeniem edycji ogloszenia na liscie.
        /// </summary>
        /// <param name="ad">Zeedytowane ogloszenie</param>
        public void SendEditedAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.EDITED_AD, json);
        }

        /// <summary>
        /// Wysyla pakiet z potwierdzeniem usuniecia ogloszenia.
        /// </summary>
        /// <param name="ad">Ogloszenie do usuniecia</param>
        public void SendDeleteAdPacket(Ad ad)
        {
            string json = JsonConvert.SerializeObject(ad);
            SendPacketWithId(PacketId.DELETE_AD, json);
        }
    }
}
