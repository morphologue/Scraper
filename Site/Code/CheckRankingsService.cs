using System;
using Scraper.Code.Ranking;
using Scraper.Code.SearchResultUrlExtraction;

namespace Scraper.Code
{
    // Logic for the CheckRankings action method of HomeController
    public static class CheckRankingsService
    {
        // The search rankings are with respect to this site.
        const string TARGET_SITE = "infotrack.com.au";

        // Call Rank() on the given IRanker then transform its result into a message which is
        // informative to the end user.
        public static (string strong, string normal) RankAndHumanify(IRanker ranker, string query)
        {
            string rankings;
            
            try {
                rankings = ranker.Rank(query, TARGET_SITE);
            } catch (ExtractionException ex) {
                return ("Error: ", ex.Message);
            }

            if (rankings == "0")
                return ("Result: ", $"The site '{TARGET_SITE}' could not be found in the search results.");
            
            string inflected_index = rankings.Split(' ').Length == 1 ? "index" : "indices";
            return ("Result: ",
                $"The site '{TARGET_SITE}' was found at the following {inflected_index} (where 1 represents the first search result): {rankings}.");
        }
    }
}
