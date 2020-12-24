using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccess
{
    public interface IAuthorsRepository
    {
        public Task<IEnumerable<Author>> GetAllAuthors();
        public IEnumerable<Author> FindAuthorsByBookId(int bookId);
    }
}
