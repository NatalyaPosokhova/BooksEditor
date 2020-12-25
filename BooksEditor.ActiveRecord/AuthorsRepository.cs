using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private BooksEditorContext _context { get; set; }
        public AuthorsRepository(BooksEditorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all Authors from Authors DB
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Author>> GetAllAuthors()
        {
            var authors = await _context.Authors.ToListAsync();
            return authors;
        }

        /// <summary>
        /// Returns List of Authors by Book Id
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public  IEnumerable<Author> FindAuthorsByBookId(int bookId)
        {
            return _context.Authors.Where(author => author.BookID == bookId); 
        }
    }
}
