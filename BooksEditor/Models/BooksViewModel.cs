using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.Models
{
    public class BooksViewModel
    {

        public ActiveRecord.Book Book { get; set; }
        public List<ActiveRecord.Author> Authors 
        { 
            get; 
            set; 
        }

        public string AuthorsNames { get; set; }
    }
}
