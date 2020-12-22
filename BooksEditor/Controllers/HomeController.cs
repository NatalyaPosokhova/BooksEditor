using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BooksEditor.Models;
using BooksEditor.DataAccsess;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FluentAssertions.Common;

namespace BooksEditor.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repository;
        public HomeController(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var books = await _repository.GetAllBooks();

            books.ToList().ForEach(book => _repository.IncludeAuthors(book.Id));
           
            switch (sortOrder)
            {
                case "name_desc":
                    books = books.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    books = books.OrderBy(s => s.ReleaseYear);
                    break;
                case "date_desc":
                    books = books.OrderByDescending(s => s.ReleaseYear);
                    break;
                default:
                    books = books.OrderBy(s => s.Title);
                    break;
            }
            return View(books);
        }

        /// <summary>
        /// GET for Create
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST for Create
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Title, Authors, PagesNumber, Publisher, ReleaseYear, Image")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repository.AddBookData(book);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /*ex*/ )
{
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
