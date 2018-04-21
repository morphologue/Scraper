using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using System.Collections.Generic;
using Scraper.Code.SearchResultUrlExtraction;
using Scraper.Code.Ranking;

namespace Tests
{
    [TestClass]
    public class ExtractionRankerTests
    {
        [TestMethod]
        public void InfiniteUrls_Rank_OnlyEnumeratesFirst100()
        {
            int step_count = 0;
            IEnumerable<string> InfinitelyMe() {
                while (true) {
                    step_count++;
                    yield return "Me";
                }
            }
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(InfinitelyMe);
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            patient.Rank("doesn't matter", "doesn't matter");

            step_count.Should().Be(100, "the user is only interested in the first 100 results");
        }

        [TestMethod]
        public void NoMatchingUrls_Rank_ReturnsZero()
        {
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(new[] { "http://site1.com", "https://www.site2.org.au/coolness?query=wot", "https://SITE3.com/", "http://site2.org.au/warmness" });
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            string result = patient.Rank("doesn't matter", "totes.not.matching");

            result.Should().Be("0", "the brief indicates that zero should be returned when there are no matches");
        }

        [TestMethod]
        public void ExactMatch_Rank_ReturnsMatchingIndex()
        {
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(new[] { "http://site1.com", "https://www.site2.org.au/coolness?query=wot", "https://SITE3.com/", "http://site2.org.au/warmness" });
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            string result = patient.Rank("doesn't matter", "site1.com");

            result.Should().Be("1", $"the hostname of (only) the first extracted URL matches the second parameter of {nameof(IRanker.Rank)}() exactly");
        }

        [TestMethod]
        public void ExactAndPartialMatches_Rank_ReturnsMatchingIndices()
        {
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(new[] { "http://site1.com", "https://www.site2.org.au/coolness?query=wot", "https://SITE3.com/", "http://site2.org.au/warmness" });
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            string result = patient.Rank("doesn't matter", "site2.org.au");

            result.Should().Be("2 4", $"the hostname of the second and fourth URLs matches 'site2.org.au' either exactly or partially");
        }

        [TestMethod]
        public void PartialMatchNotPrecededByDot_Rank_ReturnsZero()
        {
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(new[] { "http://site1.com", "https://www.site2.org.au/coolness?query=wot", "https://SITE3.com/", "http://site2.org.au/warmness" });
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            string result = patient.Rank("doesn't matter", "ite2.org.au");

            result.Should().Be("0", $"partial matches must be preceded by a dot");
        }

        [TestMethod]
        public void CaseInsensitiveMatch_Rank_ReturnsIndex()
        {
            Mock<IExtractor> mock_extractor = new Mock<IExtractor>();
            mock_extractor.Setup(e => e.Extract(It.IsAny<string>()))
                .Returns(new[] { "http://site1.com", "https://www.site2.org.au/coolness?query=wot", "https://SITE3.com/", "http://site2.org.au/warmness" });
            IRanker patient = new ExtractionRanker(mock_extractor.Object);

            string result = patient.Rank("doesn't matter", "site3.com");

            result.Should().Be("3", $"the hostname of the third URL matches 'site3.com' case insensitively");
        }
    }
}
