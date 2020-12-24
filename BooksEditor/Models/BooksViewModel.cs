using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.Models
{
    public class BooksViewModel
    {
        public Book Book { get; set; }
        public List<Author> Authors 
        { 
            get; 
            set; 
        }

        public string AuthorsNames { get; set; }
    }
}
