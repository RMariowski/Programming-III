using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    /// <summary>
    /// Baza pod zorientowana baze danych.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Database<T>
    {
        private string _databaseFile;
        private Object _lockObj;

        public List<T> Items { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="databaseFile">Sciezka do pliku, ktory bedzie zawierac dane z bazy.</param>
        public Database(string databaseFile)
        {
            _databaseFile = databaseFile;
            Items = new List<T>();
            _lockObj = new Object();

            if (!File.Exists(_databaseFile))
                File.Create(_databaseFile);
        }

        /// <summary>
        /// Destruktor
        /// </summary>
        ~Database()
        {
            Save();
        }

        /// <summary>
        /// Wczytuje wszystkie dane z pliku.
        /// </summary>
        public void Load()
        {
            using (var reader = new StreamReader(_databaseFile))
            {
                string json;
                while ((json = reader.ReadLine()) != null)
                {
                    var item = JsonConvert.DeserializeObject<T>(json);
                    Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Zapisuje wszystkie dane do pliku.
        /// </summary>
        public void Save()
        {
            using (var writer = new StreamWriter(_databaseFile))
            {
                foreach (var item in Items)
                {
                    string json = JsonConvert.SerializeObject(item);
                    writer.WriteLine(json);
                }
                writer.Flush();
            }
        }

        /// <summary>
        /// Dodaje obiekt do bazy danych.
        /// </summary>
        /// <param name="item">Obiekt do dodania</param>
        protected void AddItem(T item)
        {
            lock (_lockObj)
            {
                Items.Add(item);
            }
        }

        /// <summary>
        /// Edytuje obiekt w bazie danych.
        /// </summary>
        /// <param name="index">Indeks starego obiektu w bazie danych.</param>
        /// <param name="item">Nowy obiekt</param>
        protected void EditItem(int index, T item)
        {
            if (index < 0)
                return;

            lock (_lockObj)
            {
                Items[index] = item;
            }
        }

        /// <summary>
        /// Usuwa obiekt z bazy danych.
        /// </summary>
        /// <param name="item">Obiekt do usuniecia</param>
        protected void RemoveItem(T item)
        {
            lock (_lockObj)
            {
                Items.Remove(item);
            }
        }
    }
}
