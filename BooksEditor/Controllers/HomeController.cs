using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BooksEditor.Models;
using BooksEditor.DataAccsess;
using System.Web;
using Microsoft.EntityFrameworkCore;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BooksEditor.Controllers
{
    public class HomeController : Controller
    {
        private IBooksRepository _repository;
        private readonly IHostingEnvironment _environment;
        public HomeController(IBooksRepository repository, IHostingEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var books = await _repository.GetAllBooks();
            var authors = await _repository.GetAllAuthors();

            var booksViewModel = (from book in books
                         join author in authors on book.Id equals author.BookID
                         into tempAuthors
                         select new BooksViewModel
                         {
                             Book = book,
                             Authors = tempAuthors.ToList()
                         });
          
            switch (sortOrder)
            {
                case "name_desc":
                    booksViewModel = booksViewModel.OrderByDescending(s => s.Book.Title);
                    break;
                case "Date":
                    booksViewModel = booksViewModel.OrderBy(s => s.Book.ReleaseYear);
                    break;
                case "date_desc":
                    booksViewModel = booksViewModel.OrderByDescending(s => s.Book.ReleaseYear);
                    break;
                default:
                    booksViewModel = booksViewModel.OrderBy(s => s.Book.Title);
                    break;
            }
            return View(booksViewModel);
        }

        /// <summary>
        /// GET for Create Book
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST for Create Book
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Title, Authors, PagesNumber, Publisher, ReleaseYear, Image")] Book book, [FromForm(Name = "upload")] IFormFile upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UploadImage(upload, book);
                
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

       
        /// <summary>
        /// GET for Edit Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _repository.FindBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        /// <summary>
        /// POST for Edit Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            var bookToUpdate = await _repository.FindBookById(id);
          
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "",
                s => s.Title, s => s.Authors, s => s.PagesNumber, s => s.Publisher, s => s.ReleaseYear, s => s.Image))
            {
                try
                {
                    await _repository.UpdateBook(bookToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(bookToUpdate);
        }

        /// <summary>
        /// GET for Delete Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id, bool? saveChangesError = false)
        {
            var books = await _repository.FindBookById(id);
            if (books == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(books);
        }

        /// <summary>
        /// POST for Delete Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _repository.FindBookById(id);
            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _repository.RemoveBook(book);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Uploads Images to Server and Images names to Books Database
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="book"></param>
        private void UploadImage(IFormFile upload, Book book)
        {
            if (upload != null)
            {
                string fileName = Path.GetFileName(upload.FileName);
                var uploads = Path.Combine(_environment.WebRootPath, "img");
                var fullPath = Path.Combine(uploads, fileName);
                book.Image = fileName;
                if (!System.IO.File.Exists(fullPath))
                {
                    upload.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
            }
        }
       
    }
}
