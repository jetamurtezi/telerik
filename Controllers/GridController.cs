using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using telerik.Models;

namespace telerik.Controllers
{
    public class GridController : Controller
    {

        public IActionResult Customers()
        {
            return View();
        }
        public ActionResult Customers_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = Enumerable.Range(0, 50).Select(i => new Customer
            {
                CompanyName = "Company Name " + i,
                ContactName = "Contact Name " + i,
                ContactTitle = "Contact Title " + i,
                Country = "Coutry " + i
            });

            var dsResult = result.ToDataSourceResult(request);
            return Json(dsResult);
        }

        
    }
}
