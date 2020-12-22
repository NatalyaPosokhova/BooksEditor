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
        public Task AddBookData(Book book);
        public Task<Book> FindBookById(int id);
        public Task UpdateBook(Book book);
        public Task RemoveBook(int id);
        public void IncludeAuthors(int id);
    }
}
