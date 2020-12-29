using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord
{
    public interface IBooksRepository
    {
        public Task<IEnumerable<Book>> GetAllBooks();
        public Task AddBookData(Book book);
        public Task<Book> FindBookById(int id);
        public Task SaveBook(Book book);
        public Task RemoveBook(Book book);
    }
}
