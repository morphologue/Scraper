using System.Collections.Generic;

namespace Scraper.Code.SearchResultUrlExtraction
{
    // Implementors of this interface can extract a sequence of strings, each which represents the
    // URL of a search result, given terms to search.
    public interface IExtractor
    {
        IEnumerable<string> Extract(string search_terms);
    }
}
