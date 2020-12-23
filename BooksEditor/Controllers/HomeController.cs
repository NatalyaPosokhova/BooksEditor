﻿using System;
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
                    if (upload != null)
                    {
                        //string fileName = System.IO.Path.GetFileName(upload.FileName);
                        ////upload.SaveAs(Server.MapPath("~/img/" + fileName));
                        //var filePath = "~/img/" + fileName;
                        //using (var stream = System.IO.File.Create(filePath))
                        //{
                        //    await upload.CopyToAsync(stream);
                        //}

                        
                    }

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
    }
}
