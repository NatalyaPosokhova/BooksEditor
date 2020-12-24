using BooksEditor.DataAccsess;
using BooksEditor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccess
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private BooksEditorContext _context { get; set; }
        public AuthorsRepository(BooksEditorContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Author>> GetAllAuthors()
        {
            var authors = await _context.Authors.ToListAsync();
            return authors;
        }
        public  IEnumerable<Author> FindAuthorsByBookId(int bookId)
        {
            //var authors = await GetAllAuthors();

            return _context.Authors.Where(author => author.BookID == bookId); 
        }
    }
}
