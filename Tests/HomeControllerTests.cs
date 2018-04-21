using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using System;
using System.Linq;
using System.Collections.Generic;
using Site.Controllers;
using Scraper.Code;
using Scraper.Code.Download;
using Scraper.Code.Ranking;
using Scraper.Code.SearchResultUrlExtraction;

namespace Tests
{
    // Note that unlike the other test classes, these tests are actually integration tests.
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void TwoPagesWithTwoLinks_CheckRankings_ReturnsJsonResultWithMatchingIndices()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            mock_downloader.Setup(d => d.Download(It.IsAny<string>()))
                .Returns<string>(u => {
                    bool first_page = u.EndsWith("World");
                    string search_results = "<h3 class=\"r\"><a href=\"http://link1.com/\"></a></h3><h3 class=\"r\"><a href=\"http://www.infotrack.com.au/\"></a></h3>";
                    return search_results + (first_page ? "<a class=\"pn\" href=\"/page2\"></a>" : "");
                });
            Mock<ILogger<HomeController>> mock_logger = new Mock<ILogger<HomeController>>();
            HomeController patient = new HomeController(mock_logger.Object, new ExtractionRanker(new GoogleExtractor(mock_downloader.Object)));

            IActionResult result = patient.CheckRankings("Hello World");

            result.Should().BeOfType<JsonResult>()
                .Which.Value.Should().BeEquivalentTo(new Dictionary<string, string>()
                {
                    ["strong"] = "Result: ",
                    ["normal"] = "The site 'infotrack.com.au' was found at the following indices (where 1 represents the first search result): 2 4."
                }, "the second and final link on each page was to InfoTrack");
        }

        [TestMethod]
        public void RankerThrowsException_CheckRankings_ReturnsJsonResultWithExceptionMessage()
        {
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Hello Test"));
            Mock<ILogger<HomeController>> mock_logger = new Mock<ILogger<HomeController>>();
            HomeController patient = new HomeController(mock_logger.Object, mock_ranker.Object);

            IActionResult result = patient.CheckRankings("doesn't matter");

            result.Should().BeOfType<JsonResult>()
                .Which.Value.Should().BeEquivalentTo(new Dictionary<string, string>()
                {
                    ["strong"] = "Error: ",
                    ["normal"] = "An unexpected error occurred: Hello Test"
                }, "an unexpected exception (i.e. one other than ExtractionException) was thrown");
        }
    }
}
