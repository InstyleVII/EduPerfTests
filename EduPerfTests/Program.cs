using System;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.IO;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EduPerfTests
{
    class Program
    {
        static RemoteWebDriver driver;
        static List<string> browsers = new List<string>();
        static List<string> performanceTests = new List<string>();
        static List<string> pageLoadSites = new List<string>();
        static int performanceIterations = 0;
        static int pageLoadIterations = 0;

        static void Main(string[] args)
        {
            DetermineBrowsers();
            DeterminePerformanceTests();
            DeterminePageLoadSites();
            var perfFileName = "performancetestresults";
            var pageLoadResultsFileName = "pageloadresults";

            var perfPath = TestFileLocation(perfFileName);
            Performance.ResultsFile = perfPath;
            using (StreamWriter file = new StreamWriter(perfPath))
            {
                file.WriteLine("Benchmark,Browser,Result,Iteration");
            }

            var loadPath = TestFileLocation(pageLoadResultsFileName);
            PageLoad.Resultsfile = loadPath;
            using (StreamWriter file = new StreamWriter(loadPath))
            {
                file.WriteLine("Site,Browser,Result (ms),Iteration,Retry");
            }

            foreach (var browser in browsers)
            {
                var driver = LaunchDriver(browser);
                if (performanceTests.Count > 0)
                {
                    if (performanceTests.Contains("Octane")) Performance.Octane(browser, driver, performanceIterations);
                    if (performanceTests.Contains("SunSpider")) Performance.SunSpider(browser, driver, performanceIterations);
                    if (performanceTests.Contains("JetStream")) Performance.JetStream(browser, driver, performanceIterations);
                    if (performanceTests.Contains("WebXPRT")) Performance.WebXPRT(browser, driver, performanceIterations);
                    if (performanceTests.Contains("OORTOnline")) Performance.OORTOnline(browser, driver, performanceIterations);
                }

                if (pageLoadSites.Count > 0)
                {
                    foreach (var site in pageLoadSites)
                    {
                        PageLoad.SiteLoadTime(site, browser, driver, pageLoadIterations);
                    }
                }
                driver.Quit();
            }
        }

        /// <summary>
        /// This method takes in a fileName then looks for it int he current directory.
        /// </summary>
        /// <param name="fileName">The file name to test</param>
        /// <returns></returns>
        private static string TestFileLocation(string fileName)
        {
            var tempFileName = fileName;
            var currentPath = string.Format(@"{0}\", Directory.GetCurrentDirectory());
            var newFilePath = currentPath + string.Format(@"{0}.csv", tempFileName);

            int count = 0;
            while (File.Exists(newFilePath))
            {
                tempFileName = fileName + count.ToString();
                newFilePath = currentPath + string.Format(@"{0}.csv", tempFileName);
                count++;
            }

            return newFilePath;
        }

        static void DetermineBrowsers()
        {
            Console.WriteLine("Welcome to the Education Performance Test Suite.\n Specify the browser(s) you would like to test:\n1. Edge\n2. Chrome\n3. Firefox\n4. Internet Explorer\n5. All\n6. Instructions");
            var selectedBrowsers = Console.ReadLine();

            if (selectedBrowsers.Contains("6"))
            {
                PrintInstructions();
                DetermineBrowsers();
                return;
            }

            if (selectedBrowsers.Contains("5"))
            {
                browsers.Add("Edge");
                browsers.Add("Chrome");
                browsers.Add("Firefox");
                browsers.Add("Internet Explorer");
                return;
            }
            if (selectedBrowsers.Contains("1")) browsers.Add("Edge");
            if (selectedBrowsers.Contains("2")) browsers.Add("Chrome");
            if (selectedBrowsers.Contains("3")) browsers.Add("Firefox");
            if (selectedBrowsers.Contains("4")) browsers.Add("Internet Explorer");

            if (browsers.Count == 0)
            {
                Console.WriteLine("Unable to determine what browser you meant by '{0}'. Please try again.", selectedBrowsers);
                DetermineBrowsers();
                return;
            }
        }

        private static void PrintInstructions()
        {
            // TODO: Consider moving instructions to Exception paths to help users
            // For Web Driver, we currently put things in the PATH.ChromeDriver / EdgeDriver / InternetExplorerDriver require this.They also take a path if we wanted to automate / package.
            Console.WriteLine("Ensure all WebDriver servers are in a folder specified in PATH");
            Console.WriteLine("Chrome https://sites.google.com/a/chromium.org/chromedriver/downloads");
            Console.WriteLine("Firefox (Built in so no download is required");
            Console.WriteLine(@"Microsoft Edge Internal build can be found in following relative path from build folder x86fre\bin\spartan\MicrosoftWebDriver.exe");
            Console.WriteLine("Microsoft Edge Fall 2015 can be found at https://www.microsoft.com/en-us/download/details.aspx?id=49962");
            Console.WriteLine("Microsoft Edge Current Insider can be found at https://www.microsoft.com/en-us/download/details.aspx?id=48740");
            Console.WriteLine("IE11 can be found at http://docs.seleniumhq.org/download/");
            Console.WriteLine("Making IE11 work requires Enabling Protected mode for all Zones in Security tab of IE Settings and unblocking firewall for webdriver");
        }

        static void DeterminePerformanceTests()
        {
            Console.WriteLine("Specify the test(s) you would like to run:\n1. Octane\n2. SunSpider\n3. JetStream\n4. WebXPRT\n5. OORTOnline\n6. All\n7. Skip to Page Load");
            var selectedTests = Console.ReadLine();
            
            if (selectedTests.Contains("7"))
            {
                return;
            }

            if (selectedTests.Contains("6"))
            {
                performanceTests.Add("Octane");
                performanceTests.Add("SunSpider");
                performanceTests.Add("JetStream");
                performanceTests.Add("WebXPRT");
                performanceTests.Add("OORTOnline");
                return;
            }
            if (selectedTests.Contains("1")) performanceTests.Add("Octane");
            if (selectedTests.Contains("2")) performanceTests.Add("SunSpider");
            if (selectedTests.Contains("3")) performanceTests.Add("JetStream");
            if (selectedTests.Contains("4")) performanceTests.Add("WebXPRT");
            if (selectedTests.Contains("5")) performanceTests.Add("OORTOnline");

            if (performanceTests.Count == 0)
            {
                Console.WriteLine("Unable to determine what performance test you meant by '{0}'. Please try again.", selectedTests);
                DeterminePerformanceTests();
                return;
            }

            DeterminePerformanceIterations();
        }

        static void DeterminePerformanceIterations()
        {
            Console.WriteLine("Specify the number of iterations you would like to run:");
            int.TryParse(Console.ReadLine(), out performanceIterations);

            if (performanceIterations == 0) DeterminePerformanceIterations();
        }

        static void DeterminePageLoadSites()
        {
            Console.WriteLine("Specify the path to the csv file containing the sites you would like to test (blank to skip):");
            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path)) return;
            using (var reader = new StreamReader(path))
            {
                var input = reader.ReadToEnd();            
                pageLoadSites = input.Split(',').ToList();
            }

            // Starting count at 1 so it correlates with user chosen site start count
            int count = 1;
            foreach(var site in pageLoadSites)
            {
                Console.WriteLine(count.ToString() + "-" + site);
                count++;
            }

            DeterminePageLoadSiteStart();
            DeterminePageLoadIterations();
        }

        private static void DeterminePageLoadSiteStart()
        {
            int startingSite;
            Console.WriteLine("Specify the site number to start with. The first item is 1. (blank to run all sites):");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            startingSite = int.Parse(input);

            pageLoadSites.RemoveRange(0, startingSite - 1);
        }

        static void DeterminePageLoadIterations()
        {
            Console.WriteLine("Specify the number of iterations you would like to run:");
            int.TryParse(Console.ReadLine(), out pageLoadIterations);

            if (pageLoadIterations == 0) DeterminePageLoadIterations();
        }

        static RemoteWebDriver LaunchDriver(string browser)
        {
            try
            {
                switch (browser)
                {
                    case "Firefox":
                        driver = new FirefoxDriver();                      
                        break;
                    case "Chrome":
                        driver = new ChromeDriver();
                        break;
                    case "Internet Explorer":
                        driver = new InternetExplorerDriver();
                        break;
                    default:
                        driver = new EdgeDriver();
                        
                        // This sleep allows Edge Anniversary Update to workaround a bug where it doesn't properly wait for about:blank                     
                        Thread.Sleep(2000);
                        break;
                }

                driver.Manage().Window.Maximize();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to launch {0}, ERROR: {1}", browser, ex.Message);
            }

            return driver;
        }
    }
}