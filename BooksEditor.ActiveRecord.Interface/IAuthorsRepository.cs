using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord
{
    public interface IAuthorsRepository
    {
        public Task<IEnumerable<Author>> GetAllAuthors();
        public Task AddAuthorData(Author author);
        public Task<bool> IsAuthorExists(Author author);
        public Task SaveAuthor(Author author);
        public Task<int> GetAuthorId(Author author);
        public Task RemoveAuthorById(int authorId);
    }
}
