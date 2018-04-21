using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using System;
using System.Linq;
using System.Collections.Generic;
using Scraper.Code.Download;
using Scraper.Code.SearchResultUrlExtraction;

namespace Tests
{
    [TestClass]
    public class GoogleExtractorTests
    {
        [TestMethod]
        public void DownloadingRecaptcha_ExtractToList_ThrowsExtractionException()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            mock_downloader.Setup(d => d.Download("https://www.google.com.au/search?q=Hello+World&start=0"))
                .Returns("<html><body><div id=\"recaptcha\" class=\"g-recaptcha\"></div></body></html>");
            IExtractor patient = new GoogleExtractor(mock_downloader.Object);

            Action action = () => patient.Extract("Hello World").ToList();

            action.Should().Throw<ExtractionException>()
                .Which.Message.Should().Contain("reCAPTCHA", "the HTML includes the reCAPTCHA indicator (and no next page link)");
        }

        [TestMethod]
        public void EmptyLink_ExtractToList_ThrowsExtractionException()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            mock_downloader.Setup(d => d.Download("https://www.google.com.au/search?q=Hello+World&start=0"))
                .Returns("<h3 class=\"r\"><a href=\"\"></a></h3>");
            IExtractor patient = new GoogleExtractor(mock_downloader.Object);

            Action action = () => patient.Extract("Hello World").ToList();

            action.Should().Throw<ExtractionException>()
                .Which.Message.Should().EndWith("There might be a problem with Google.", "an empty HREF is not expected");
        }

        [TestMethod]
        public void UnterminatedLink_ExtractToList_ThrowsExtractionException()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            mock_downloader.Setup(d => d.Download("https://www.google.com.au/search?q=Hello+World&start=0"))
                .Returns("<h3 class=\"r\"><a href=\"></a></h3>");
            IExtractor patient = new GoogleExtractor(mock_downloader.Object);

            Action action = () => patient.Extract("Hello World").ToList();

            action.Should().Throw<ExtractionException>()
                .Which.Message.Should().EndWith("There might be a problem with Google.", "an unterminated HREF is not expected");
        }

        [TestMethod]
        public void TwoPagesWithTwoLinksEach_ExtractToList_Returns4Links()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            mock_downloader.Setup(d => d.Download(It.IsAny<string>()))
                .Returns<string>(u => {
                    bool first_page = u.EndsWith("start=0");
                    string search_results = "<h3 class=\"r\"><a href=\"http://link1.com/\"></a></h3><h3 class=\"r\"><a href=\"http://link2.com/\"></a></h3>";
                    return search_results + (first_page ? "<a class=\"pn\" href=\"/page2\"></a>" : "");
                });
            IExtractor patient = new GoogleExtractor(mock_downloader.Object);

            List<string> result = patient.Extract("Hello World").ToList();

            result.Should().BeEquivalentTo(new[] { "http://link1.com/", "http://link2.com/", "http://link1.com/", "http://link2.com/" },
                "the same two links are on both pages");
        }

        [TestMethod]
        public void TwoPages_ExtractToList_UpdatesStartQueryParamForPagination()
        {
            Mock<IDownloader> mock_downloader = new Mock<IDownloader>();
            string downloaded_url = null;
            mock_downloader.Setup(d => d.Download(It.IsAny<string>()))
                .Returns<string>(u => {
                    bool first_page = u.EndsWith("start=0");
                    downloaded_url = u;
                    string search_results = "<h3 class=\"r\"><a href=\"http://link1.com/\"></a></h3><h3 class=\"r\"><a href=\"http://link2.com/\"></a></h3>";
                    return search_results + (first_page ? "<a class=\"pn\" href=\"/page2\"></a>" : "");
                });
            IExtractor patient = new GoogleExtractor(mock_downloader.Object);

            patient.Extract("Hello World").ToList();

            downloaded_url.Should().Be("https://www.google.com.au/search?q=Hello+World&start=2", "there were two links on the first page");
        }
    }
}
