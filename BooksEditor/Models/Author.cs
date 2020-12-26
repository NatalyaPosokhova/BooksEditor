using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksEditor.Models
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

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Required]
        public Book Book { get; set; }

    }
}
