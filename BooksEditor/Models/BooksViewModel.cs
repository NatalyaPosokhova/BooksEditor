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
        

        public IList<string> Authors
        {
            get
            {
                IList<String> result = new List<String>();
                foreach (var item in Book.Authors)
                {
                    result.Add(item.FirstName + " " + item.LastName);
                }
                return result;
            }
        }
    }
}
