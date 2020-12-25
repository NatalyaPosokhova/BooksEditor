﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BooksEditor.ActiveRecord
{
    internal class CustomDateAttribute : RangeAttribute
    {
        private const int minYear = 1800;
        public CustomDateAttribute()
        : base(typeof(int),
          minYear.ToString(),
          DateTime.Now.Year.ToString())
        { }
    }
}