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

        public async Task AddAuthorData(Author author)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAuthorExists(Author author)
        {
            return await _context.Authors.AnyAsync(a => a.FirstName == author.FirstName && a.LastName == author.LastName);
        }

        public async Task<int> GetAuthorId(Author author)
        {
            return await _context.Authors.Where(a => a.FirstName == author.FirstName && a.LastName == author.LastName).Select(au => au.AuthorID).FirstAsync();
        }
        public async Task SaveAuthor(Author author)
        {
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAuthorById(int authorId)
        {
            var authorToDelete = await _context.Authors.SingleAsync(a => a.AuthorID == authorId);
            _context.Remove(authorToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
