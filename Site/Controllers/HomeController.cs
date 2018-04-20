using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckRankings(string query)
        {
            System.Threading.Thread.Sleep(5000);
            return Json(new Dictionary<string, string>()
            {
                ["strong"] = "Result: ",
                ["normal"] = $"This seems alright, aye? You entered '{query}'."
            });
        }
    }
}
