﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BooksEditor.Models
{
    public class Author
    {
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