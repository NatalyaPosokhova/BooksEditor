using BooksEditor.ActiveRecord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int PagesNumber { get; set; }

        public string Publisher { get; set; }

        public int ReleaseYear { get; set; }

        public string Image { get; set; }
        public List<string> Authors { get; set; }
        public string AuthorsString
        {
            get => String.Join(", ", Authors.ToArray());
            set => this.Authors = value.Split(", ").ToList();
        }
    }
}
