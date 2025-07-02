using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telerik.Data;
using telerik.Models;
using telerik.Services;

namespace telerik.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IBookService _bookService;

        public BookController(ApplicationDbContext db, IBookService bookService)
        {
            _db = db;
            _bookService = bookService;
        }
        public IActionResult Index()
        {
            var books = _db.Book.ToList();
            return View("Book", books);
        }

        public IActionResult Books_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_db.Book.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult Books_Create([DataSourceRequest] DataSourceRequest request, Book book)
        {
            if (ModelState.IsValid)
            {
                _db.Book.Add(book);
                _db.SaveChanges();
            }

            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }



        [HttpPost]
        public IActionResult Books_Update([DataSourceRequest] DataSourceRequest request, Book book)
        {
            if (ModelState.IsValid)
            {
                _db.Book.Update(book);
                _db.SaveChanges();
            }
            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult Books_Delete([DataSourceRequest] DataSourceRequest request, Book book)
        {
            if (ModelState.IsValid)
            {
                _db.Remove(book);
                _db.SaveChanges();
            }
            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<IActionResult> UploadCoverImage(IFormFile file)
        {
            var path = await _bookService.UploadCoverImageAsync(file);
            if (path == null)
                return BadRequest("File upload failed.");

            return Json(new { CoverImagePath = path });
        }

        [HttpPost]
        public IActionResult RemoveCoverImage([FromBody] string[] fileNames)
        {
            _bookService.RemoveFiles(fileNames);
            return Ok();
        }
    }
}
