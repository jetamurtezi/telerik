using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using telerik.Models;
using telerik.Services;
using Microsoft.AspNetCore.Authorization;

namespace telerik.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _environment;
        public BookController(IBookService bookService, IWebHostEnvironment environment)
        {
            _bookService = bookService;
            _environment = environment;
        }
        public IActionResult Details(int id)
        {
            var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
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
        public async Task<IActionResult> CreateBook([DataSourceRequest] DataSourceRequest request, Book book)
        {
            Console.WriteLine($"Received CreateBook request: ModelState.IsValid = {ModelState.IsValid}");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }
            Console.WriteLine($"Book from model: {Newtonsoft.Json.JsonConvert.SerializeObject(book)}");
            Console.WriteLine($"Form data: {string.Join(", ", Request.Form.Keys.Select(k => $"{k}: {Request.Form[k]}"))}");

            if (book != null)
            {
                // Fallback to form data if model binding fails
                if (string.IsNullOrEmpty(book.Title) && !string.IsNullOrEmpty(Request.Form["Title"]))
                {
                    book.Title = Request.Form["Title"];
                    book.Author = Request.Form["Author"];
                    book.Genre = Request.Form["Genre"];
                    book.Price = decimal.TryParse(Request.Form["Price"], out var price) ? price : (decimal?)null;
                    book.Stock = int.TryParse(Request.Form["Stock"], out var stock) ? stock : (int?)null;
                    book.CategoryId = int.TryParse(Request.Form["CategoryId"], out var categoryId) ? categoryId : (int?)null;
                }

                // Handle CoverImage upload
                if (string.IsNullOrEmpty(book.CoverImage) && !string.IsNullOrEmpty(Request.Form["CoverImage"]))
                {
                    book.CoverImage = Request.Form["CoverImage"];
                    ModelState.Remove("CoverImage"); // Nëse është i detyrueshëm
                }
                else if (!string.IsNullOrEmpty(Request.Form["CoverImage"]))
                {
                    book.CoverImage = Request.Form["CoverImage"];
                    Console.WriteLine($"Used existing CoverImage: {book.CoverImage}");
                }
                else
                {
                    // Allow null CoverImage if not required
                    ModelState.Remove("CoverImage"); // Remove validation if file is optional
                }

                if (ModelState.IsValid || true) // Temporary bypass for debugging
                {
                    _bookService.CreateBook(book);
                    Console.WriteLine($"Book saved with Id: {book.Id}");
                }
                else
                {
                    Console.WriteLine("Model state invalid, not saving book.");
                }
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file uploaded." });

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = "/uploads/" + fileName;

            return Json(new { success = true, imageUrl });
        }
        //[Authorize(Roles = "Admin")]
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


        [HttpPost]
        public IActionResult SubmitOrder(Order order)
        {
            var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == order.BookId);
            if (book == null || book.Stock <= 0)
                return NotFound();

            book.Stock -= 1;
            _bookService.UpdateBook(book);

            _bookService.SubmitOrder(order);

            TempData["SuccessMessage"] = "Blerja u krye me sukses!";
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Orders()
        {
            var orders = _bookService.GetAllOrders(); 
            return View(orders);
        }

        public IActionResult Dashboard()
        {
            var books = _bookService.GetAllBooks();
            var orders = _bookService.GetAllOrders();

            int totalStock = books.Sum(b => b.Stock ?? 0); 
            int totalOrders = orders.Count;

            int totalInitialStock = books.Sum(b =>
            {
                int ordersForBook = orders.Count(o => b.Id.HasValue && o.BookId == b.Id.Value);
                return (b.Stock ?? 0) + ordersForBook;
            });

            ViewBag.TotalStock = totalStock;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.InitialStock = totalInitialStock;

            return View();
        }

        public IActionResult GetDashboardChartData()
        {
            var books = _bookService.GetAllBooks();
            var orders = _bookService.GetAllOrders();

            int totalStock = books.Sum(b => b.Stock ?? 0);
            int totalOrders = orders.Count;
            int totalInitialStock = books.Sum(b =>
            {
                int ordersForBook = orders.Count(o => o.BookId == b.Id);
                return (b.Stock ?? 0) + ordersForBook;
            });

            var chartData = new List<DashboardData>
    {
        new DashboardData { Label = "Të Shitur", Value = totalOrders },
        new DashboardData { Label = "Në Stok", Value = totalStock },
        new DashboardData { Label = "Stoku Fillestar", Value = totalInitialStock }
    };

            return Json(chartData);
        }


    }
}
