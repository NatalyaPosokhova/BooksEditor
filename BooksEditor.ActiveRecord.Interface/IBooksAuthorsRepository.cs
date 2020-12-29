using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord.Interface
{
    public interface IBooksAuthorsRepository
    {
        public Task<IEnumerable<BookAuthor>> GetAllBooksAuthors();
        public Task<IEnumerable<int>> GetAuthorsIdsByBookId(int bookId);
        public Task<IEnumerable<int>> GetBooksIdsByAuthorid(int authorId);
        public Task AddBookAuthor(BookAuthor bookAuthor);
        public Task RemoveAuthorForBook(BookAuthor bookAuthor);
    }
}
