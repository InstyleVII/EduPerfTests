using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    class Program
    {
        private static Browser chosenBrowsers;
        private static readonly List<string> performanceTests = new List<string>();
        private static List<string> pageLoadSites = new List<string>();
        private static int performanceIterations = 0;
        private static int pageLoadIterations = 1; // always do at least one - using this number for Mem Use as well...
        private static string pathToSites = string.Empty;
        private static string pageLoadScheme = string.Empty;

        static void Main(string[] args)
        {
            bool runPerfTests = false, runPageLoadTests = false, runPageMemoryTests = false;

            DetermineBrowsers();

            Console.WriteLine("Run Performance Tests? y|n (default is n)");
            string response = Console.ReadLine();
            if (string.IsNullOrEmpty(response))
                response = "n";
            if (response.ToLower() == "y")
                runPerfTests = true;

            Console.WriteLine("Run Page Load Tests? y|n (default is n)");
            response = Console.ReadLine();
            if (string.IsNullOrEmpty(response))
                response = "n";
            if (response.ToLower() == "y")
                runPageLoadTests = true;

            Console.WriteLine("Run Page Memory Tests? y|n (default is n)");
            response = Console.ReadLine();
            if (string.IsNullOrEmpty(response))
                response = "n";
            if (response.ToLower() == "y")
                runPageMemoryTests = true;

            if (runPerfTests)
            {
                DeterminePerformanceTests();
            }
            if (runPageLoadTests || runPageMemoryTests)
            {
                DetermineSitesToLoad();
                DetermineScheme();
                DeterminePageLoadIterations();
            }
            if (runPerfTests)
            {
                RunPerformance();
            }
            if (runPageLoadTests)
            {
                RunPageLoad();
            }
            if (runPageMemoryTests)
            {
                RunPageMemory();
            }

            Console.WriteLine("Done... Press Enter to exit.");
            Console.ReadLine();
        }
        private static void RunPageMemory()
        {
            if (pageLoadSites.Count > 0)
            {
                MemoryUsage memUse = new MemoryUsage();

                foreach (Browser browser in chosenBrowsers.ChosenBrowsers())
                {
                    memUse.RunMemoryUsageTests(pageLoadSites, browser, pageLoadIterations);
                }
            }
        }
        private static void RunPageLoad()
        {
            if (pageLoadSites.Count > 0)
            {
                PageLoad pageLoader = new PageLoad(pageLoadScheme);
                pageLoader.LoadPages(pageLoadSites, chosenBrowsers, pageLoadIterations);
            }
        }        

        private static void RunPerformance()
        {
            if (performanceTests.Count > 0)
            {
                using (var perfLog = new PerformanceLog("performancetestresults"))
                {
                    perfLog.InitializeLog("Benchmark,Browser,Result,Iteration");

                    Performance performanceTester = new Performance(perfLog);

                    foreach (Browser browser in chosenBrowsers.ChosenBrowsers())
                    {
                        try
                        { 
                            using (var driver = LaunchDriver(browser))
                            {
                                InitializeDriver(driver);

                                if (performanceTests.Contains("Octane"))
                                    performanceTester.Octane(browser, driver, performanceIterations);
                                if (performanceTests.Contains("SunSpider"))
                                    performanceTester.SunSpider(browser, driver, performanceIterations);
                                if (performanceTests.Contains("JetStream"))
                                    performanceTester.JetStream(browser, driver, performanceIterations);
                                if (performanceTests.Contains("WebXPRT"))
                                    performanceTester.WebXPRT(browser, driver, performanceIterations);
                                if (performanceTests.Contains("OORTOnline"))
                                    performanceTester.OORTOnline(browser, driver, performanceIterations);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Unexpected exception from running browser: {e}");
                        }
                    }
                }
            }
        }

        static void DetermineBrowsers()
        {
            Console.WriteLine("Welcome to the Education Performance Test Suite.\n Specify the browser(s) you would like to test:\n1. Edge\n2. Chrome\n3. Firefox\n4. Internet Explorer\n5. All (except IE)\n6. Instructions");
            var selectedBrowsers = Console.ReadLine();

            if (selectedBrowsers.Contains("6"))
            {
                PrintInstructions();
                DetermineBrowsers();
                return;
            }

            if (selectedBrowsers.Contains("5"))
            {
                chosenBrowsers = Browser.All;
                return;
            }
            
            if (selectedBrowsers.Contains("1")) chosenBrowsers = chosenBrowsers | Browser.MicrosoftEdge;
            if (selectedBrowsers.Contains("2")) chosenBrowsers = chosenBrowsers | Browser.Chrome;
            if (selectedBrowsers.Contains("3")) chosenBrowsers = chosenBrowsers | Browser.Firefox;
            if (selectedBrowsers.Contains("4")) chosenBrowsers = chosenBrowsers | Browser.InternetExplorer;

            if (chosenBrowsers == 0)
            {
                Console.WriteLine(
                    $"Unable to determine what browser you meant by '{selectedBrowsers}'. Please try again.");
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
            }

            if (selectedTests.Contains("1")) performanceTests.Add("Octane");
            if (selectedTests.Contains("2")) performanceTests.Add("SunSpider");
            if (selectedTests.Contains("3")) performanceTests.Add("JetStream");
            if (selectedTests.Contains("4")) performanceTests.Add("WebXPRT");
            if (selectedTests.Contains("5")) performanceTests.Add("OORTOnline");

            if (performanceTests.Count == 0)
            {
                Console.WriteLine(
                    $"Unable to determine what performance test you meant by '{selectedTests}'. Please try again.");
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

        static void DetermineSitesToLoad()
        {
            Console.WriteLine("Specify the path to the csv file containing the sites you would like to test (blank to skip, d for default):");
            pathToSites = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(pathToSites)) return;

            if (pathToSites.ToLower() == "d")
                pathToSites = Directory.GetCurrentDirectory().ToString() + @"\sitelist.csv";

            using (var reader = new StreamReader(pathToSites))
            {
                var input = reader.ReadToEnd();            
                pageLoadSites = input.Split(',').ToList();
            }

            // Starting count at 1 so it correlates with user chosen site start count
            var count = 1;
            foreach(var site in pageLoadSites)
            {
                Console.WriteLine(count.ToString() + "-" + site);
                count++;
            }

            DeterminePageLoadSiteStart();
        }
        
        private static void DeterminePageLoadSiteStart()
        {
            Console.WriteLine("Specify the site number to start with. The first item is 1. (blank to run all sites):");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            var startingSite = int.Parse(input);

            pageLoadSites.RemoveRange(0, startingSite - 1);
        }

        static void DetermineScheme()
        {
            Console.WriteLine("Specify the scheme to use to load the sites (blank for https, invalid scheme will use https):");
            pageLoadScheme = Console.ReadLine();

            if (!Uri.CheckSchemeName(pageLoadScheme))
            {
                pageLoadScheme = Uri.UriSchemeHttps;
            }
        }

        static void DeterminePageLoadIterations()
        {
            Console.WriteLine("Specify the number of iterations you would like to run:");
            int.TryParse(Console.ReadLine(), out pageLoadIterations);

            if (pageLoadIterations < 1) DeterminePageLoadIterations();
        }
    }
}