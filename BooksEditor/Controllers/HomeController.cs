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
using BooksEditor.ActiveRecord.Interface;

namespace BooksEditor.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBooksAuthorsRepository _booksAuthorsRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IBooksRepository booksRepository, IWebHostEnvironment environment, IAuthorsRepository authorsRepository, IBooksAuthorsRepository booksAuthorsRepository)
        {
            _booksRepository = booksRepository;
            _authorsRepository = authorsRepository;
            _booksAuthorsRepository = booksAuthorsRepository;
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
            var booksAuthors = await _booksAuthorsRepository.GetAllBooksAuthors();

            var booksViewModels = new List<BookViewModel>();
            //int id = 0;

            foreach (var book in books)
            {

                var authorsIds = await _booksAuthorsRepository.GetAuthorsIdsByBookId(book.Id);

                booksViewModels.Add(new BookViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    PagesNumber = book.PagesNumber,
                    Publisher = book.Publisher,
                    ReleaseYear = book.ReleaseYear,
                    Image = book.Image,
                    Authors = authors.Where(x => authorsIds.Contains(x.AuthorID)).Select(a => a.FullName).ToList()
                });
            }

            switch (sortOrder)
            {
                case "name_desc":
                    booksViewModels = booksViewModels.OrderByDescending(s => s.Title).ToList();
                    break;
                case "Date":
                    booksViewModels = booksViewModels.OrderBy(s => s.ReleaseYear).ToList();
                    break;
                case "date_desc":
                    booksViewModels = booksViewModels.OrderByDescending(s => s.ReleaseYear).ToList();
                    break;
                default:
                    booksViewModels = booksViewModels.OrderBy(s => s.Title).ToList();
                    break;
            }

            return View(booksViewModels);
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
        [Bind("Title, Authors, PagesNumber, Publisher, ReleaseYear")] BookViewModel bookViewModel, [FromForm(Name = "uploadImage")] IFormFile uploadImage)
        {
            int authorId;
            bool IsAuthorExists;

            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadImage != null)
                    {
                        bookViewModel.Image = UploadImage(uploadImage);
                    }

                    var book = new Book()
                    {
                        Title = bookViewModel.Title,
                        PagesNumber = bookViewModel.PagesNumber,
                        Publisher = bookViewModel.Publisher,
                        ReleaseYear = bookViewModel.ReleaseYear,
                        Image = bookViewModel.Image
                    };
                    await _booksRepository.AddBookData(book);

                    var authorsArray = String.Join(", ", bookViewModel.Authors.ToArray()).Split(", ");
                    foreach (var author in authorsArray)
                    {
                        Author authorToDownload = new Author
                        {
                            FirstName = author.Split(" ")[0],
                            LastName = author.Split(" ")[1]
                        };

                        IsAuthorExists = await _authorsRepository.IsAuthorExists(authorToDownload);
                        if (!IsAuthorExists)
                        {
                            await _authorsRepository.AddAuthorData(authorToDownload);
                            authorId = authorToDownload.AuthorID;
                        }
                        else
                        {
                            authorId = await _authorsRepository.GetAuthorId(authorToDownload);
                        }

                        var bookAuthor = new BookAuthor(book.Id, authorId);
                        await _booksAuthorsRepository.AddBookAuthor(bookAuthor);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /*ex*/ )
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(bookViewModel);
        }


        /// <summary>
        /// GET for Edit Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int Id)
        {
            var book = await _booksRepository.FindBookById(Id);
            if (book == null)
            {
                return NotFound();
            }

            var authors = await _authorsRepository.GetAllAuthors();
            var authorsIds = await _booksAuthorsRepository.GetAuthorsIdsByBookId(Id);

            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                PagesNumber = book.PagesNumber,
                Publisher = book.Publisher,
                ReleaseYear = book.ReleaseYear,
                //Image = book.Image,
                Authors = authors.Where(x => authorsIds.Contains(x.AuthorID)).Select(a => a.FullName).ToList()
            };

            return View(bookViewModel);
        }

        /// <summary>
        /// POST for Edit Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("Title, AuthorsString, PagesNumber, Publisher, ReleaseYear")] BookViewModel bookViewModel)
        {
            try
            {
                var bookToUpdate = await _booksRepository.FindBookById(id);

                bookToUpdate.Title = bookViewModel.Title;
                bookToUpdate.PagesNumber = bookViewModel.PagesNumber;
                bookToUpdate.Publisher = bookViewModel.Publisher;
                bookToUpdate.ReleaseYear = bookViewModel.ReleaseYear;

                await _booksRepository.SaveBook(bookToUpdate);
                await UpdateAuthorsListInDatabase(id, bookViewModel.Authors);
                
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return View(bookViewModel);
        }

        /// <summary>
        /// GET for Delete Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id, bool? saveChangesError = false)
        {
            var book = await _booksRepository.FindBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            var authors = await _authorsRepository.GetAllAuthors();
            var authorsIds = await _booksAuthorsRepository.GetAuthorsIdsByBookId(book.Id);
            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                PagesNumber = book.PagesNumber,
                Publisher = book.Publisher,
                ReleaseYear = book.ReleaseYear,
                Image = book.Image,
                Authors = authors.Where(x => authorsIds.Contains(x.AuthorID)).Select(a => a.FullName).ToList()
            };

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(bookViewModel);
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
            try
            {
                var book = await _booksRepository.FindBookById(id);
                if (book == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                var authorsToDeleteIds = await _booksAuthorsRepository.GetAuthorsIdsByBookId(id);

                foreach (var idToDelete in authorsToDeleteIds)
                {
                    await _booksAuthorsRepository.RemoveAuthorForBook(new BookAuthor(id, idToDelete));

                    var relations = await _booksAuthorsRepository.GetBooksIdsByAuthorid(idToDelete);
                    if (relations.Count() == 0)
                    {
                        await _authorsRepository.RemoveAuthorById(idToDelete);
                    }
                }
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
        private string UploadImage(IFormFile uploadedImage)
        {
            string fileName = Path.GetFileName(uploadedImage.FileName);
            var folderPath = Path.Combine(_environment.WebRootPath, "img");
            var fullPath = Path.Combine(folderPath, fileName);

            if (!System.IO.File.Exists(fullPath))
            {
                uploadedImage.CopyTo(new FileStream(fullPath, FileMode.Create));
            }
            return fileName;
        }

        private async Task UpdateAuthorsListInDatabase(int bookId, List<string> authors)
        {
            var authorsToUpdateIds = await _booksAuthorsRepository.GetAuthorsIdsByBookId(bookId);

            foreach (var idToUpdate in authorsToUpdateIds)
            {
                await _booksAuthorsRepository.RemoveAuthorForBook(new BookAuthor(bookId, idToUpdate));

                var relations = await _booksAuthorsRepository.GetBooksIdsByAuthorid(idToUpdate);
                if (relations.Count() == 0)
                {
                    await _authorsRepository.RemoveAuthorById(idToUpdate);
                }
            }

            List<Author> newAuthors = new List<Author>();
            foreach (var authorToLoad in authors)
            {
                newAuthors.Add(new Author()
                {
                    FirstName = authorToLoad.Split(" ")[0],
                    LastName = authorToLoad.Split(" ")[1]
                });
            }

            List<int> newAuthorsIds = new List<int>();
            foreach (var newAuthor in newAuthors)
            {
                var IsAuthorExists = await _authorsRepository.IsAuthorExists(newAuthor);

                if (!IsAuthorExists)
                {
                    await _authorsRepository.AddAuthorData(newAuthor);
                }
                int authorId = await _authorsRepository.GetAuthorId(newAuthor);
                newAuthorsIds.Add(authorId);
            }

            foreach (var newAuthorsId in newAuthorsIds)
            {
                await _booksAuthorsRepository.AddBookAuthor(new BookAuthor(bookId, newAuthorsId));
            }
        }
       
    }
}
