using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public interface IRepository
    {
        public IEnumerable<Book> GetAll();
        public void Add(Book book);
        public Book Find(int id);
        public void Update(Book book);
        public void Remove(int id);
    }
}
