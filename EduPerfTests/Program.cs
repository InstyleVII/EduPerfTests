using System;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;

namespace EduPerfTests
{
    class Program
    {
        static RemoteWebDriver driver;
        static List<String> browsers = new List<String>();
        static List<String> tests = new List<String>();
        static int iterations = 1;

        static void Main(string[] args)
        {
            DetermineBrowsers();
            DetermineTests();

            Console.WriteLine("Specify the number of iterations you would like to run:");
            iterations = Int32.Parse(Console.ReadLine());

            string perfPath = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(perfPath))
            {
                file.WriteLine("Benchmark,Browser,Result,Iteration");
            }

            string loadPath = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(loadPath))
            {
                file.WriteLine("Site,Browser,Result (ms),Iteration,");
            }

            foreach (var browser in browsers)
            {
                var driver = LaunchDriver(browser);
                if (tests.Contains("Octane")) Performance.Octane(browser, driver, iterations);
                if (tests.Contains("SunSpider")) Performance.SunSpider(browser, driver, iterations);
                if (tests.Contains("JetStream")) Performance.JetStream(browser, driver, iterations);
                if (tests.Contains("WebXPRT")) Performance.WebXPRT(browser, driver, iterations);
                if (tests.Contains("OORTOnline")) Performance.OORTOnline(browser, driver, iterations);
                PageLoad.SiteLoadTime("http://www.bing.com/mapspreview", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://www.brainpop.com/", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://www.edmodo.com/", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://www.google.com/maps", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://www.biodigital.com/", browser, driver, iterations);
                PageLoad.SiteLoadTime("http://www.jpl.nasa.gov/", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://www.khanacademy.org/", browser, driver, iterations);
                PageLoad.SiteLoadTime("https://scratch.mit.edu/", browser, driver, iterations);
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
            return;
        }

        static void DetermineTests()
        {
            Console.WriteLine("Specify the test(s) you would like to run:\n1. Octane\n2. SunSpider\n3. JetStream\n4. WebXPRT\n5. OORTOnline\n6. All");
            var selectedTests = Console.ReadLine();

            if (selectedTests.Contains("6"))
            {
                tests.Add("Octane");
                tests.Add("SunSpider");
                tests.Add("JetStream");
                tests.Add("WebXPRT");
                tests.Add("OORTOnline");
                return;
            }
            if (selectedTests.Contains("1")) tests.Add("Octane");
            if (selectedTests.Contains("2")) tests.Add("SunSpider");
            if (selectedTests.Contains("3")) tests.Add("JetStream");
            if (selectedTests.Contains("4")) tests.Add("WebXPRT");
            if (selectedTests.Contains("5")) tests.Add("OORTOnline");
            if (browsers.Count == 0) DetermineTests();
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
