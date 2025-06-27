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
        public IActionResult Customers()
        {
            return View();
        }
        public IActionResult Customers_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_db.Customers.ToDataSourceResult(request));
        }
        [HttpPost]
        public IActionResult Customers_Create([DataSourceRequest] DataSourceRequest request, Customer customer)
        {
            if (ModelState.IsValid)
            {
                _db.Add(customer);
                _db.SaveChanges();
            }
            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public IActionResult Customers_Update([DataSourceRequest] DataSourceRequest request, Customer customer)
        {
            if (ModelState.IsValid)
            {
                _db.Update(customer);
                _db.SaveChanges();
            }
            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult Customer_Delete([DataSourceRequest] DataSourceRequest request, Customer customer)
        {
            if (ModelState.IsValid)
            {
                _db.Remove(customer);
                _db.SaveChanges();
            }
            return Json(new[] { customer }.ToDataSourceResult(request, ModelState));
        }
    }
}
