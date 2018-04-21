using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.Code;
using Scraper.Code.Ranking;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        ILogger<HomeController> log;
        IRanker ranker;

        public HomeController(ILogger<HomeController> log, IRanker ranker)
        {
            this.log = log;
            this.ranker = ranker;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult CheckRankings(string query)
        {
            string strong, normal;
            try {
                (strong, normal) = CheckRankingsService.RankAndHumanify(ranker, query);
            } catch (Exception ex) {
                strong = "Error: ";
                normal = $"An unexpected error occurred: {ex.Message}";
                log.LogError(ex, $"Unexpected error during {nameof(CheckRankings)}");
            }
            return Json(new Dictionary<string, string>()
            {
                ["strong"] = strong,
                ["normal"] = normal
            });
        }
    }
}
