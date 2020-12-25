using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BooksEditor.ActiveRecord
{
    public class Book
    {
        public Book()
        {
            Authors = new List<Author>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        public ICollection<Author> Authors { get; set; }

        [Required]
        [Range(0, 10000)]
        public int PagesNumber { get; set; }

        [StringLength(30)]
        public string Publisher { get; set; }

        [Required]
        [CustomDate]
        public int ReleaseYear { get; set; }

        public string Image { get; set; }

    }
}
