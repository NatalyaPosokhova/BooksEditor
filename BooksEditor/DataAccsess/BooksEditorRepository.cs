using BooksEditor.Models;
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

        public async void AddBookData(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book> FindBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }

        public async void RemoveBook(int id)
        {
            var book = await FindBookById(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public void Update(Book book)
        {
            throw new NotImplementedException();
        }


    }
}
