using System;
using Server.Models;
using System.Linq;

namespace Server
{
    /// <summary>
    /// Baza danych z ogloszeniami klientow.
    /// </summary>
    public class AdDatabase : Database<Ad>
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public AdDatabase()
            : base("ads.json")
        {
        }

        /// <summary>
        /// Dodaje ogloszenie do bazy danych.
        /// </summary>
        /// <param name="title">Tytul ogloszenia</param>
        /// <param name="description">Opis ogloszenia</param>
        /// <param name="author">Autor ogloszenia</param>
        /// <return>Utworzone ogloszenie</return>
        public Ad AddAd(string title, string description, string author)
        {
            long id = 0;
            if (Items.Count != 0)
                id = Items[Items.Count - 1].Id + 1;

            var ad = new Ad(id, title, description, author, DateTime.Now);
            AddItem(ad);
            return ad;
        }

        /// <summary>
        /// Usuwa ogloszenie z bazy danych.
        /// </summary>
        /// <param name="ad">Ogloszenie do usuniecia</param>
        public void DeleteAd(Ad ad)
        {
            if (ad == null)
                return;

            RemoveItem(ad);
        }

        /// <summary>
        /// Edytuje ogloszenie w bazie danych.
        /// </summary>
        /// <param name="ad">Zedytowane ogloszenie</param>
        public void EditAd(Ad ad)
        {
            if (ad == null)
                return;

            var oldAd = Items.Where(x => x.Id == ad.Id).First();
            int index = Items.IndexOf(oldAd);
            EditItem(index, ad);
        }
    }
}
