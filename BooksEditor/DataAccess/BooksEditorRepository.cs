using BooksEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public class BooksEditorRepository : IRepository
    {
        private BooksEditorContext _context { get; set; }
        public BooksEditorRepository(BooksEditorContext context)
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
        /// Finds book by Id
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

        /// <summary>
        /// Uodates Book in BookDb by id
        /// </summary>
        /// <param name="id"></param>
        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public void IncludeAuthors(int id)
        {
            //_context.Books.Include(x => x.Authors).Where(x => x.Id == id).Single();
            _context.Books
            .Include(x => x.Authors)
            .Where(x => x.Id == id).ToList();
        }
    }
}
