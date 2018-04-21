using Scraper.Code.SearchResultUrlExtraction;

namespace Scraper.Code.Ranking
{
    public interface IRanker
    {
        // Return a string of space-delimited numbers representing the 1-based index of each
        // occurrence of target_site in search results, or the string "0" if there are no such
        // occurrences. The domain of the search result must either be or end with the given site.
        // This is the method which is detailed in the project brief.
        string Rank(string search_terms, string target_site);
    }
}
