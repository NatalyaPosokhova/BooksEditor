using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public interface IBooksEditorRepository
    {
        public Task<IEnumerable<Book>> GetAllBooks();
        public Task<IEnumerable<Author>> GetAllAuthors();
        public Task AddBookData(Book book);
        public Task<Book> FindBookById(int id);
        public Task UpdateBook(Book book);
        public Task RemoveBook(Book book);
    }
}
