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

            var perfPath = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(perfPath))
            {
                file.WriteLine("Benchmark,Browser,Result,Iteration");
            }

            var loadPath = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(loadPath))
            {
                file.WriteLine("Site,Browser,Result (ms),Iteration,");
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

        static void DetermineBrowsers()
        {
            Console.WriteLine("Welcome to the Education Performance Test Suite.\n Specify the browser(s) you would like to test:\n1. Edge\n2. Chrome\n3. Firefox\n4. Internet Explorer\n5. All");
            var selectedBrowsers = Console.ReadLine();

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

            if (browsers.Count == 0) DetermineBrowsers();
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

            if (performanceTests.Count == 0) DeterminePerformanceTests();

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

            DeterminePageLoadIterations();
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
                        driver.Manage().Window.Maximize();
                        break;
                    case "Chrome":
                        driver = new ChromeDriver();
                        driver.Manage().Window.Maximize();
                        break;
                    case "Internet Explorer":
                        driver = new InternetExplorerDriver();
                        driver.Manage().Window.Maximize();
                        break;
                    default:
                        driver = new EdgeDriver();
                        driver.Manage().Window.Maximize();
                        Thread.Sleep(2000);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to launch {0}, ERROR: {1}", browser, ex.Message);
            }

            return driver;
        }
    }
}