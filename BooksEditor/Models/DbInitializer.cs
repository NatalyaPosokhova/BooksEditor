using System.Linq;
using BooksEditor.ActiveRecord;
using BooksEditor.ActiveRecord.Interface;

namespace BooksEditor.Models
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
            new Book{ Title = "The Adventures of Tom Sawyer",  PagesNumber = 1000, Publisher = "HarperCollins Publishers", ReleaseYear = 1876, Image = "TomSawyer.jpg"},
            new Book{ Title = "Winnie the Pooh", PagesNumber = 300, Publisher = "Random House Inc.", ReleaseYear = 1926, Image = "Winnie.jpg" }
            };
            foreach (Book book in books)
            {
                context.Books.Add(book);
            }
            context.SaveChanges();

            var authors = new Author[]
            {
                new Author {
                    FirstName = "Mark",
                    LastName = "Twain"
                },
                new Author {
                    FirstName = "Alan",
                    LastName = "Keller"
                },
                new Author {
                    FirstName = "Tom",
                    LastName = "Andersen"
                },
                new Author {
                    FirstName = "Alan",
                    LastName = "Miln"
                }
            };

            foreach (Author author in authors)
            {
                context.Authors.Add(author);
            }

            var bookAuthors = new BookAuthor[]
            {
                new BookAuthor(1, 1),
                new BookAuthor(1, 2),
                new BookAuthor(1, 3),
                new BookAuthor(2, 4),
                new BookAuthor(2, 1)
            };

            foreach (BookAuthor bookAuthor in bookAuthors)
            {
                context.BookAuthors.Add(bookAuthor);
            }

            context.SaveChanges();
        }
    }
}