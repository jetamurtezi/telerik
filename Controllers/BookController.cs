using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using telerik.Models;
using telerik.Services;

namespace telerik.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public IActionResult GridView()
        {
            var categories = _bookService.GetAllCategories() ?? new List<BookCategory>();
            ViewData["Categories"] = categories;
            return View();
        }
        [HttpGet]
        public IActionResult GetBooks([DataSourceRequest] DataSourceRequest request)
        {
            var books = _bookService.GetAllBooks();
            var result = books.ToDataSourceResult(request);
            Console.WriteLine($"GetBooks result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([DataSourceRequest] DataSourceRequest request, Book book, IFormFile CoverImage)
        {
            Console.WriteLine($"Received CreateBook request: ModelState.IsValid = {ModelState.IsValid}");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }
            Console.WriteLine($"Book from model: {Newtonsoft.Json.JsonConvert.SerializeObject(book)}");
            Console.WriteLine($"CoverImage file: {CoverImage?.FileName}");
            Console.WriteLine($"Form data: {string.Join(", ", Request.Form.Keys.Select(k => $"{k}: {Request.Form[k]}"))}");

            if (book != null)
            {
                if (string.IsNullOrEmpty(book.Title) && !string.IsNullOrEmpty(Request.Form["Title"]))
                {
                    book.Title = Request.Form["Title"];
                    book.Author = Request.Form["Author"];
                    book.Genre = Request.Form["Genre"];
                    book.Price = decimal.TryParse(Request.Form["Price"], out var price) ? price : (decimal?)null;
                    book.Stock = int.TryParse(Request.Form["Stock"], out var stock) ? stock : (int?)null;
                    book.CategoryId = int.TryParse(Request.Form["CategoryId"], out var categoryId) ? categoryId : (int?)null;
                }

                if (CoverImage != null && CoverImage.Length > 0)
                {
                    book.CoverImage = await _bookService.UploadCoverImageAsync(CoverImage);
                    Console.WriteLine($"Assigned CoverImage: {book.CoverImage}");
                }
                else
                {
                    ModelState.Remove("CoverImage"); // Allow null if no file
                }

                if (ModelState.IsValid || true) // Temporary bypass
                {
                    _bookService.CreateBook(book);
                    Console.WriteLine($"Book saved with Id: {book.Id}");
                }
            }

            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBook([DataSourceRequest] DataSourceRequest request, Book book, IFormFile CoverImage)
        {
            if (book != null && book.Id != null && ModelState.IsValid)
            {
                if (CoverImage != null && CoverImage.Length > 0)
                {
                    book.CoverImage = await _bookService.UploadCoverImageAsync(CoverImage);
                }

                _bookService.UpdateBook(book);
            }

            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult DeleteBook([DataSourceRequest] DataSourceRequest request, Book book)
        {
            if (book != null)
            {
                _bookService.DeleteBook(book.Id.Value);
            }

            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> UploadCoverImageAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Json(new { path = $"/uploads/{fileName}" });
            }
            return Json(new { error = "No file uploaded" });
        }



        public IActionResult TreeListView()
        {
            return View();
        }

        public IActionResult GetBookCategories([DataSourceRequest] DataSourceRequest request)
        {
            var categories = _bookService.GetAllCategories();
            return Json(categories.ToTreeDataSourceResult(request, c => c.Id, c => c.ParentId));
        }

        [HttpPost]
        public IActionResult CreateCategory([DataSourceRequest] DataSourceRequest request, BookCategory category)
        {
            if (category != null && !string.IsNullOrWhiteSpace(category.Name))
            {
                _bookService.CreateCategory(category);
            }
            else
            {
                ModelState.AddModelError("Name", "Emri i kategorisë është i detyrueshëm.");
            }

            return Json(new[] { category }.ToTreeDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult UpdateCategory([DataSourceRequest] DataSourceRequest request, BookCategory category)
        {
            if (category != null && !string.IsNullOrWhiteSpace(category.Name))
            {
                _bookService.UpdateCategory(category);
            }
            else
            {
                ModelState.AddModelError("Name", "Emri i kategorisë është i detyrueshëm.");
            }

            return Json(new[] { category }.ToTreeDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult DeleteCategory([DataSourceRequest] DataSourceRequest request, BookCategory category)
        {
            if (category != null)
            {
                _bookService.DeleteCategory(category);
            }

            return Json(new[] { category }.ToTreeDataSourceResult(request, ModelState));
        }

        public JsonResult GetMainCategories()
        {
            var mainCategories = _bookService.GetAllCategories()
                                  .Where(c => c.ParentId == null)
                                  .Select(c => new { c.Id, c.Name })
                                  .ToList();
            return Json(mainCategories);
        }

        public JsonResult GetSubCategories(int parentId)
        {
            var subCategories = _bookService.GetAllCategories()
                                  .Where(c => c.ParentId == parentId)
                                  .Select(c => new { c.Id, c.Name })
                                  .ToList();
            return Json(subCategories);
        }



    }
}
