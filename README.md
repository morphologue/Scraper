# Scraper
This web app allows the user to enter a search query, which defaults to "online title search", then
see the rankings for infotrack.com.au as scraped from google.com.au.

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

## Building a Docker Container
From the top level of the checkout, in bash:
```
cd Site
./publish.sh
sudo docker build -t scraper .
```

## Notes
* Only the first 100 search results are scrutinised.
* The rankings reflect genuine search results: ads are not included.
