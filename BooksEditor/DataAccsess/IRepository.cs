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
        public void AddBookData(Book book);
        public Book FindBookById(int id);
        public void Update(Book book);
        public void Remove(int id);
    }
}
