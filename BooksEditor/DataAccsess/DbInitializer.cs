using BooksEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.DataAccsess
{
    public class DbInitializer
    {
        public static void Initialize(BooksEditorContext context)
        {
            context.Database.EnsureCreated();

            if (context.Books.Any())
            {
                return;
            }

            var books = new Book[]
            {
            new Book{ Title = "The Adventures of Tom Sawyer",  PagesNumber = 1000, Publisher = "HarperCollins Publishers", ReleaseYear = 1876},
            new Book{ Title = "Winnie the Pooh", PagesNumber = 300, Publisher = "Random House Inc.", ReleaseYear = 1926}
            };
            foreach (Book book in books)
            {
                context.Books.Add(book);
            }
            context.SaveChanges();

            var authors = new Author[]
{
                new Author {
                    BookID = books.Single(s => s.Title == "The Adventures of Tom Sawyer").Id,
                     FirstName = "Mark",
                     LastName = "Twain"
                },
                    new Author {
                    BookID = books.Single(s => s.Title == "Winnie the Pooh").Id,
                     FirstName = "Alan",
                     LastName = "Miln"
                    }
};

            foreach (Author author in authors)
            {
                var enrollmentInDataBase = context.Authors.Where(
                    s => s.Book.Id == author.BookID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Authors.Add(author);
                }
            }
            context.SaveChanges();
        }
    }
}
