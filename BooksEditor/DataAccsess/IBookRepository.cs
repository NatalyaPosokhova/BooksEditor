using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public interface IBookRepository
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Author> Authors { get; set; }
        public int PagesNumber { get; set; }
        public string Publisher { get; set; }
        public int ReleaseDate { get; set; }
        public string Image { get; set; }
    }
}
