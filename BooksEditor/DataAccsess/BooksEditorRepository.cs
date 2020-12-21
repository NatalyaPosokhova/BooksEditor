using BooksEditor.Models;
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

        public void AddBookData(Book book)
        {
            _context.Add(book);
        }

        public Book Find(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
