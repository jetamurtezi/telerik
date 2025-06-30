using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telerik.Data;
using telerik.Models;

namespace telerik.Controllers
{
    public class GridController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GridController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Books()
        {
            return View();
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
                _db.Add(book);
                _db.SaveChanges();
            }
            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public IActionResult Books_Update([DataSourceRequest] DataSourceRequest request, Book book)
        {
            if (ModelState.IsValid)
            {
                _db.Update(book);
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
    }
}
