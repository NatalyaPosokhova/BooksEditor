//using BooksEditor.Models;
using System.Linq;
using BooksEditor.ActiveRecord;


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

            var books = new ActiveRecord.Book[]
            {
            new ActiveRecord.Book{ Title = "The Adventures of Tom Sawyer",  PagesNumber = 1000, Publisher = "HarperCollins Publishers", ReleaseYear = 1876, Image = "TomSawyer.jpg"},
            new ActiveRecord.Book{ Title = "Winnie the Pooh", PagesNumber = 300, Publisher = "Random House Inc.", ReleaseYear = 1926, Image = "Winnie.jpg" }
            };
            foreach (ActiveRecord.Book book in books)
            {
                context.Books.Add(book);
            }
            context.SaveChanges();

            var authors = new ActiveRecord.Author[]
{
                new ActiveRecord.Author {
                    BookID = books.Single(s => s.Title == "The Adventures of Tom Sawyer").Id,
                     FirstName = "Mark",
                     LastName = "Twain"
                },
                new ActiveRecord.Author {
                    BookID = books.Single(s => s.Title == "The Adventures of Tom Sawyer").Id,
                     FirstName = "Alan",
                     LastName = "Keller"
                },
                new ActiveRecord.Author {
                    BookID = books.Single(s => s.Title == "The Adventures of Tom Sawyer").Id,
                     FirstName = "Tom",
                     LastName = "Andersen"
                },
                new ActiveRecord.Author {
                BookID = books.Single(s => s.Title == "Winnie the Pooh").Id,
                FirstName = "Alan",
                LastName = "Miln"
                }
};

            foreach (ActiveRecord.Author author in authors)
            {
                var authorInDataBase = context.Authors.Where(
                    s => s.Book.Id == author.BookID).SingleOrDefault();
                if (authorInDataBase == null)
                {
                    context.Authors.Add(author);
                }
            }
            context.SaveChanges();
        }
    }
}
