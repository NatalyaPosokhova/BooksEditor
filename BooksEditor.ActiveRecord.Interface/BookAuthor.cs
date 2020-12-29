using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BooksEditor.ActiveRecord.Interface
{
    public class BookAuthor
    {
        public BookAuthor(int bookId, int authorId)
        {
            BookId = bookId;
            AuthorId = authorId;
        }

        [Key]
        public int BookAuthorId { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; private set; }

        [Required]
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; private set; }
    
    }
}
