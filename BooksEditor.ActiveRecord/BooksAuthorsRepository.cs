using BooksEditor.ActiveRecord.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksEditor.ActiveRecord
{
    public class BooksAuthorsRepository : IBooksAuthorsRepository
    {
        private BooksEditorContext _context { get; set; }
        public BooksAuthorsRepository(BooksEditorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns Books and Authors relations
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BookAuthor>> GetAllBooksAuthors()
        {
            return await _context.BookAuthors.ToListAsync();
        }

        /// <summary>
        /// Returns all authors for book by bookId
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetAuthorsIdsByBookId(int bookId)
        {
            return await _context.BookAuthors.Where(x => x.BookId == bookId).Select(x => x.AuthorId).ToListAsync();
        }

        /// <summary>
        /// Returns all books for author by authorId
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetBooksIdsByAuthorid(int authorId)
        {
            return await _context.BookAuthors.Where(x => x.AuthorId == authorId).Select(x => x.BookId).ToListAsync();
        }

        public async Task AddBookAuthor(BookAuthor bookAuthor)
        {
            await _context.AddAsync(bookAuthor);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAuthorForBook(BookAuthor bookAuthor)
        {
            _context.BookAuthors.Remove(bookAuthor);
            await _context.SaveChangesAsync();
        }

    }
}
