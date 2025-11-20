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
using System.Diagnostics;

//dotnet add package Selenium.WebDriver
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
        static string webPath = "https://intranet.bansi.local/Access/Index";
        /*
            https://intranet.bansi.local/Access/Index
            https://amazon.com.mx
            https://www.scrapingcourse.com/ecommerce/
         */
        static void Main(string[] args)
        {
            //extrac_product_data(webPath);
            //WebCrawling(webPath);
            scrapSelenium_openChromeInHeadless();
        }

        #region Auxiliares

        #region File
        static bool toCSV(List<Product> products, string filename)
        {
            Console.WriteLine("\n\ttoCSV:");
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
            catch (UnauthorizedAccessException e) // Access problems
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch (FileNotFoundException e)       // File does not exist
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch (IOException e)                // Some other IO problem.
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                ok = false;
            }
            finally
            {
                Console.WriteLine("\t\tOK:" + ok);
                //this append even after catch!!!
            }
            return ok;
        }
        static bool toTxt(string text, string filename)
        {
            Console.WriteLine("\n\ttoTxt:");
            bool ok = true;
            try
            {
                using (var writer = new StreamWriter(filename + ".txt"))
                {
                    writer.Write(text);
                }
                Process.Start(new ProcessStartInfo("C:\\Learning_c-sharp\\Web_Scraping\\ConsoleApp1\\ConsoleApp1\\bin\\Debug\\net8.0\\"+ filename + ".txt") { UseShellExecute = true });

            }
            catch (UnauthorizedAccessException e) // Access problems
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch (FileNotFoundException e)       // File does not exist
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch (IOException e)                // Some other IO problem.
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ok = false;
            }
            finally
            {
                Console.WriteLine("\t\tOK:" + ok);
                //this append even after catch!!!
            }
            return ok;
        }
        #endregion File

        #region Productos
        static List<Product> toProducts(IList<HtmlNode> productHTMLElements)
        {
            Console.WriteLine("\n\ttoProducts:");
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
            Console.WriteLine("\t\tOK:" + ok);
            return products;
        }

        #endregion Productos

        #endregion Auxiliares

        #region Spiders
        static void extrac_product_data(string web_path)
        {
            Console.WriteLine("\nextrac_product_data");

            var web = new HtmlWeb();
            // setting a global User-Agent header 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
            
            // loading the target web page
            var document = web.Load(web_path);
            Console.WriteLine("\n\tDocumentNode Length: " + document.DocumentNode.OuterLength);
            toTxt(document.DocumentNode.OuterHtml, "_ContenidoHTML"); //document.DocumentNode.InnerHtml

            // selecting all HTML product elements from the current page 
            IList<HtmlNode> productHTMLElements = document.DocumentNode.QuerySelectorAll("td.product");
            Console.WriteLine("\n\tproductHTMLElements: " + productHTMLElements.Count);

            var products = new List<Product>();
            products = toProducts(productHTMLElements);
            if (products.Count != 0)
                toCSV(products, "_products");
        }
        
        static void WebCrawling(string web_path)
        {
            Console.WriteLine("\nWebCrawling");
            // initializing HAP 
            var web = new HtmlWeb();

            // setting a global User-Agent header 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

            // creating the list that will keep the scraped data 
            var products = new List<Product>();
            // the URL of the first pagination web page 
            var firstPageToScrape = web_path;
            // the list of pages discovered during the crawling task 
            var pagesDiscovered = new List<string> { firstPageToScrape };
            // the list of pages that remains to be scraped 
            var pagesToScrape = new Queue<string>();
            // initializing the list with firstPageToScrape 
            pagesToScrape.Enqueue(firstPageToScrape);

            Console.WriteLine("\n\tpagesToScrape: " + pagesToScrape.Count);

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

                Console.WriteLine("\n\tpaginationHTMLElements: " + paginationHTMLElements.Count);

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
                    toCSV(products, "_products_" + i);

                // incrementing the crawling counter 
                i++;
            }
        }

        /*
        Dynamic content websites use JavaScript for rendering or retrieving data. That's because they rely on JavaScript to dynamically retrieve all or part of the content. Scraping such websites requires a tool that can run JavaScript, like a headless browser. If you're not familiar with this term, a headless browser is a programmable browser with no GUI.
        With more than 65 million downloads, the most used headless browser library for C# is Selenium. Install Selenium.WebDriver's NuGet package.
        dotnet add package Selenium.WebDriver

        The Selenium FindElements() function allows instructing the browser to look for HTML nodes. Thanks to it, you can select the product HTML elements via a CSS selector query. Then, iterate over them in a foreach loop. Apply GetAttribute() and use Text to extract the data of interest.
        Scraping a website in C# with HAP or Selenium is about the same, code-wise. The difference is in the way they run the scraping logic. HAP parses HTML pages to extract data from them and Selenium runs the scraping statements in a headless browser.

        Other tools to consider when it comes to web scraping with C# are:

        ZenRows: A fully-featured easy-to-use API to make extracting data from web pages easy. ZenRows offers an automatic bypass for any anti-bot or anti-scraping system. Plus, it comes with rotating proxies, headless browser functionality, and a 99% uptime guarantee.
        Puppeteer Sharp: The .NET port of the popular Puppeteer Node.js library. With it, you can instruct a headless Chromium browser to perform testing and scraping.
        AngleSharp: An open-source .NET library for parsing and manipulating XML and HTML. It allows you to extract data from a website and select HTML elements via CSS selectors.
         */
        static void scrapSelenium_openChromeInHeadless()
        {
            try
            {
                Console.WriteLine("\nscrapSelenium ");
                var products = new List<Product>();

                // to open Chrome in headless mode 
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");

                // starting a Selenium instance 
                using (var driver = new ChromeDriver(chromeOptions))
                {
                    // navigating to the target page in the browser 
                    driver.Navigate().GoToUrl(webPath);//webPath

                    Console.WriteLine("\n\tWeb Title: " + driver.Title);
                    toTxt(driver.PageSource, "_ContenidoHTML"); //document.DocumentNode.InnerHtml

                    // getting the HTML product elements 
                    //IList<HtmlNode>
                    var productHTMLElements = driver.FindElements(By.CssSelector("li.product"));

                    // iterating over them to scrape the data of interest 
                    foreach (var productHTMLElement in productHTMLElements)
                    {
                        // scraping logic 
                        var url = productHTMLElement.FindElement(By.CssSelector("a")).GetAttribute("href");
                        var image = productHTMLElement.FindElement(By.CssSelector("img")).GetAttribute("src");
                        var name = productHTMLElement.FindElement(By.CssSelector("h2")).Text;
                        var price = productHTMLElement.FindElement(By.CssSelector(".price")).Text;

                        var product = new Product() { Url = url, Image = image, Name = name, Price = price };

                        products.Add(product);
                    }
                }

                if (products.Count != 0)
                    toCSV(products, "_products");
            }
            catch (Exception ex) {
                Console.WriteLine("\nERROR:\a");
                Console.WriteLine("\n*** Message *********************************************************************************");
                Console.WriteLine(ex.Message);

                Console.WriteLine("\n*** Data *********************************************************************************");

                // Data is empty
                Console.WriteLine(ex.Data.Values);

                // Add custom data to the exception
                ex.Data.Add("UserContext", "User 'JohnDoe' attempted to perform action 'UpdateProfile'.");
                ex.Data.Add("ItemId", 123);
                ex.Data.Add("Timestamp", DateTime.UtcNow);

                // Log the exception, including the custom data
                foreach(System.Collections.DictionaryEntry entry in ex.Data)
                {
                    Console.WriteLine($"  Custom Data - Key: {entry.Key}, Value: {entry.Value}");
                }

                Console.WriteLine("\n*** StackTrace *********************************************************************************");
                Console.WriteLine(ex.StackTrace);

                Console.WriteLine("\n*** ex.ToString() *********************************************************************************");
                Console.WriteLine(ex.ToString());

                Console.WriteLine("\n*** END *********************************************************************************");
            }
        }
        #endregion Spiders

        //
        //Web data scraping using C# is a challenge.
        //That's due to the many anti-scraping technologies websites now use.
        //Bypassing them all isn't easy, and you always need to find a workaround.
        //Avoid all this with a complete C# web scraping API, like ZenRows.
        //Thanks to it, you perform data scraping via API calls and forget about anti-bot protections.
        //

    }
}