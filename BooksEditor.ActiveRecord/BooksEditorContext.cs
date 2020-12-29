using BooksEditor.ActiveRecord.Interface;
using Microsoft.EntityFrameworkCore;

namespace BooksEditor.ActiveRecord
{
    public class BooksEditorContext : DbContext
    {
        public BooksEditorContext(DbContextOptions<BooksEditorContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>().HasKey(ba => new { ba.BookId, ba.AuthorId });
        }
    }
}
