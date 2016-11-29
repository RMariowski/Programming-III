using System;

namespace Server.Models
{
    /// <summary>
    /// Ogloszenie
    /// </summary>
    public class Ad
    {
        public long Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Id ogloszenia</param>
        /// <param name="title">Tytul ogloszenia</param>
        /// <param name="description">Opis ogloszenia</param>
        /// <param name="author">Autor ogloszenia</param>
        /// <param name="createDate">Data utworzenia ogloszenia</param>
        public Ad(long id, string title, string description, string author, DateTime createDate)
        {
            Id = id;
            Title = title;
            Description = description;
            Author = author;
            CreateDate = createDate;
        }
    }
}
