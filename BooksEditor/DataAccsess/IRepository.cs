using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public interface IRepository
    {
        public Task<IEnumerable<Book>> GetAllBooks();
        public void AddBookData(Book book);
        public Task<Book> FindBookById(int id);
        public void Update(Book book);
        public void Remove(int id);
    }
}
