using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BooksEditor.Models;
using BooksEditor.ActiveRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;



namespace BooksEditor.Controllers
{
    public class HomeController : Controller
    {
        private ActiveRecord.IBooksRepository _booksRepository;
        private ActiveRecord.IAuthorsRepository _authorsRepository;
        private readonly IWebHostEnvironment _environment;
        public HomeController(IBooksRepository booksRepository, IWebHostEnvironment environment, IAuthorsRepository authorsRepository)
        {
            _booksRepository = booksRepository;
            _authorsRepository = authorsRepository;
            _environment = environment;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var books = await _booksRepository.GetAllBooks();
            var authors = await _authorsRepository.GetAllAuthors();

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
        [Bind("Title, PagesNumber, Publisher, ReleaseYear, Image")] Models.Book book, string AuthorsNames, [FromForm(Name = "uploadImage")] IFormFile uploadImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UploadImage(uploadImage, book);

                    if (!string.IsNullOrEmpty(AuthorsNames))
                    {
                        SetAuthorsInitials(AuthorsNames, book);
                    }
                    ActiveRecord.Book new_book = new ActiveRecord.Book()
                    {
                        Title = book.Title,
                        Publisher = book.Publisher,
                        ReleaseYear = book.ReleaseYear,
                        PagesNumber = book.PagesNumber,
                        Image = book.Image,
                        Authors = (ICollection<ActiveRecord.Author>)book.Authors                   
                    };
                    //SetAuthorsInitials(AuthorsNames, new_book);

                    await _booksRepository.AddBookData(new_book);
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
            var book = await _booksRepository.FindBookById(id);
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
        public async Task<IActionResult> EditPost(int id, string AuthorsNames)
        {
           var bookToUpdate = await _booksRepository.FindBookById(id);

            if (await TryUpdateModelAsync<ActiveRecord.Book>(
                bookToUpdate,
                "",
                s => s.Title, s => s.PagesNumber, s => s.Publisher, s => s.ReleaseYear))
            {
                try
                {
                    await _booksRepository.UpdateBook(bookToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
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
            var books = await _booksRepository.FindBookById(id);
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
            var book = await _booksRepository.FindBookById(id);
            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _booksRepository.RemoveBook(book);
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
        private void UploadImage(IFormFile upload, Models.Book book)
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

        /// <summary>
        /// Sets Authors FirstName and LastName in Authors DB
        /// </summary>
        /// <param name="AuthorsNames"></param>
        /// <param name="book"></param>
        private void SetAuthorsInitials(string AuthorsNames, Models.Book book)
        {
            var fullNames = AuthorsNames.Split(", ");
            foreach (var fullName in fullNames)
            {
                var firstName = fullName.Split(" ")[0];
                var lastName = fullName.Split(" ")[1];

                if (!book.Authors.Any())
                    book.Authors.Add(new Models.Author { FirstName = firstName, LastName = lastName });
            }
        }

        /// <summary>
        /// Writes Authors' Full Names in string
        /// </summary>
        /// <param name="authors"></param>
        /// <returns></returns>
        private string GetAuthorsNamesString(IEnumerable<Models.Author> authors)
        {
            return String.Join(", ", authors.Select(author => author.FullName));
        }
    }
}
