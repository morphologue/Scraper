# Scraper
This web app allows the user to enter a search query, which defaults to "online title search", then
see the rankings for infotrack.com.au as scraped from google.com.au.

An instance of this app is hosted at https://gavin-tech.com/scraper.

## Tests
From the top level of the checkout:
```
cd Tests
dotnet test
```

## Running Locally
From the top level of the checkout:
```
cd Site
dotnet run
```
Then open http://localhost:5000/scraper in a browser.

## Notes
* Only the first 100 search results are scrutinised.
* The rankings reflect genuine search results: ads are not included.
* The server code downloads multiple pages from Google rather than requesting 100 results up front, as this helps to prevent Google from blocking the app.
* For the same reason there is a random delay in between Google page loads.