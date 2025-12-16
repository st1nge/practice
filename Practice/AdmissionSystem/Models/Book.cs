using System;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public string ISBN { get; set; }
        public int CategoryId { get; set; }

        public Book()
        {
            Title = string.Empty;
            Author = string.Empty;
            Genre = string.Empty;
            Description = string.Empty;
            Count = 1;
            ISBN = string.Empty;
            Year = 0;
            CategoryId = 0;
        }
    }
}
