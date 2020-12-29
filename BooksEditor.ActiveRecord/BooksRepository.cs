using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord
{
    public class BooksRepository : IBooksRepository
    {
        private BooksEditorContext _context { get; set; }
        public BooksRepository(BooksEditorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds Book to BookDB
        /// </summary>
        /// <param name="book"></param>
        public async Task AddBookData(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds book by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book</returns>
        public async Task<Book> FindBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        /// <summary>
        /// Gets all Books from BookDB
        /// </summary>
        /// <returns>Books</returns>
        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }
       
        /// <summary>
        /// Removes Book from BookDB
        /// </summary>
        /// <param name="id"></param>
        public async Task RemoveBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task SaveBook(Book book)
        {
            await _context.SaveChangesAsync();
        }

    }
}
