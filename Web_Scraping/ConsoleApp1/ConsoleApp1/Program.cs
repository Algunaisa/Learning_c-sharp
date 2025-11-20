/*
 https://www.zenrows.com/blog/web-scraping-c-sharp#web-crawling
 */

//ADD in terminal or NuGetPackage:
//dotnet add package HtmlAgilityPack
//dotnet add package HtmlAgilityPack.CssSelectors
using HtmlAgilityPack;

//dotnet add package CsvHelper
using CsvHelper;
using System.Globalization;
using System.Reflection.Metadata;

namespace SimpleWebScraper
{
    public class Product
    {
        public string? Url { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string web_path = "https://intranet.bansi.local/Access/Index";
            extrac_product_data(web_path);
            WebCrawling(web_path);
        }

        static bool toCSV(List<Product> products, string filename)
        {
            bool ok = true;
            try
            {
                // initializing the CSV output file 
                using (var writer = new StreamWriter(filename + ".csv"))
                // initializing the CSV writer 
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // populating the CSV file 
                    csv.WriteRecords(products);
                }
            }
            catch (Exception ex) 
            { 
                ok = false;
            }
            return ok;
        }
        static List<Product> toProducts(IList<HtmlNode> productHTMLElements)
        {
            bool ok = true;
            var products = new List<Product>();
            try
            {
                //HtmlDocument document;
                //IList<HtmlNode> productHTMLElements = document.DocumentNode.QuerySelectorAll("li.product");

                // iterating over the list of product elements 
                foreach (HtmlNode productHTMLElement in productHTMLElements)
                {
                    // scraping logic 
                    string url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value);
                    string image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value);
                    string name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h2").InnerText);
                    string price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price").InnerText);
                    Product product = new Product() { Url = url, Image = image, Name = name, Price = price };
                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                ok = false;
            }
            return products;
        }
        static void extrac_product_data(string web_path)
        {
            //var web_path = "https://www.scrapingcourse.com/ecommerce/";

            var web = new HtmlWeb();
            var products = new List<Product>();

            // loading the target web page
            var document = web.Load(web_path);

            // selecting all HTML product elements from the current page 
            IList<HtmlNode> productHTMLElements = document.DocumentNode.QuerySelectorAll("td.product");
            products = toProducts(productHTMLElements);
            if (products.Count != 0)
                toCSV(products, "products");
        }
        
        static void WebCrawling(string web_path)
        {
            // initializing HAP 
            var web = new HtmlWeb();

            // creating the list that will keep the scraped data 
            var products = new List<Product>();
            // the URL of the first pagination web page 
            var firstPageToScrape = web_path;// "https://www.scrapingcourse.com/ecommerce/page/1/";
            // the list of pages discovered during the crawling task 
            var pagesDiscovered = new List<string> { firstPageToScrape };
            // the list of pages that remains to be scraped 
            var pagesToScrape = new Queue<string>();
            // initializing the list with firstPageToScrape 
            pagesToScrape.Enqueue(firstPageToScrape);
            // current crawling iteration 
            int i = 1;
            // the maximum number of pages to scrape before stopping 
            int limit = 12;
            // until there is a page to scrape or limit is hit 
            while (pagesToScrape.Count != 0 && i < limit)
            {
                // getting the current page to scrape from the queue 
                var currentPage = pagesToScrape.Dequeue();
                // loading the page 
                var currentDocument = web.Load(currentPage);
                // selecting the list of pagination HTML elements 
                IList<HtmlNode> paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("a.page-numbers");
                // to avoid visiting a page twice 
                foreach (var paginationHTMLElement in paginationHTMLElements)
                {
                    // extracting the current pagination URL 
                    var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;
                    // if the page discovered is new 
                    if (!pagesDiscovered.Contains(newPaginationLink))
                    {
                        // if the page discovered needs to be scraped 
                        if (!pagesToScrape.Contains(newPaginationLink))
                        {
                            pagesToScrape.Enqueue(newPaginationLink);
                        }
                        pagesDiscovered.Add(newPaginationLink);
                    }
                }
                // getting the list of HTML product nodes 
                IList<HtmlNode> productHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("li.product");
                products = toProducts(productHTMLElements);

                if(products.Count != 0)
                    toCSV(products, "products_" + i);

                // incrementing the crawling counter 
                i++;
            }
            

        }
    }
}