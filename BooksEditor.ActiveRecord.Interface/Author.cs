using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksEditor.ActiveRecord
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public int BookID { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

       [Required]
        public Book Book { get; set; }

    }
}
