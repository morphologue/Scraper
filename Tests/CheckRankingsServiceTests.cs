using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using System;
using Scraper.Code;
using Scraper.Code.Ranking;
using Scraper.Code.SearchResultUrlExtraction;

namespace Tests
{
    [TestClass]
    public class CheckRankingsServiceTests
    {
        [TestMethod]
        public void RankerThrowsException_RankAndHumanify_ThrowsException()
        {
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();

            Action action = () => CheckRankingsService.RankAndHumanify(mock_ranker.Object, "doesn't matter");

            action.Should().Throw<Exception>("the service is only expected to deal with ExtractionExceptions");
        }

        [TestMethod]
        public void RankerThrowsExtractionException_RankAndHumanify_ReturnsExceptionMessage()
        {
            const string MESSAGE = "this is a test!";
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ExtractionException(MESSAGE));

            (string strong, string normal) result = CheckRankingsService.RankAndHumanify(mock_ranker.Object, "doesn't matter");

            result.Should().Be(("Error: ", MESSAGE), "the message from an ExtractionException should be passed back to the user");
        }

        [TestMethod]
        public void RankerReturnsZero_RankAndHumanify_ReturnsPoliteRejectionMessage()
        {
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("0");

            (string strong, string normal) result = CheckRankingsService.RankAndHumanify(mock_ranker.Object, "doesn't matter");

            result.Should().Be(("Result: ", "The site 'infotrack.com.au' could not be found in the search results."),
                "such a message lets the user down gently");
        }

        [TestMethod]
        public void RankerReturnsSingleIndex_RankAndHumanify_ReturnsSingularMessage()
        {
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("4");

            (string strong, string normal) result = CheckRankingsService.RankAndHumanify(mock_ranker.Object, "doesn't matter");

            result.Should().Be(("Result: ", "The site 'infotrack.com.au' was found at the following index (where 1 represents the first search result): 4."),
                "a count noun referring to a single thing should take singular form");
        }

        [TestMethod]
        public void RankerReturnsMultipleIndices_RankAndHumanify_ReturnsPluralMessage()
        {
            Mock<IRanker> mock_ranker = new Mock<IRanker>();
            mock_ranker.Setup(r => r.Rank(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("4 14");

            (string strong, string normal) result = CheckRankingsService.RankAndHumanify(mock_ranker.Object, "doesn't matter");

            result.Should().Be(("Result: ", "The site 'infotrack.com.au' was found at the following indices (where 1 represents the first search result): 4 14."),
                "a count noun referring to a zero or many things should take plural form");
        }
    }
}
