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
                if (tests.Contains("Octane")) Octane(browser, driver);
                if (tests.Contains("SunSpider")) SunSpider(browser, driver);
                if (tests.Contains("JetStream")) JetStream(browser, driver);
                if (tests.Contains("WebXPRT")) WebXPRT(browser, driver);
                if (tests.Contains("OORTOnline")) OORTOnline(browser, driver);
                SiteLoadTime("http://www.bing.com/mapspreview", browser, driver);
                SiteLoadTime("https://www.brainpop.com/", browser, driver);
                SiteLoadTime("https://www.edmodo.com/", browser, driver);
                SiteLoadTime("https://www.google.com/maps", browser, driver);
                SiteLoadTime("https://www.biodigital.com/", browser, driver);
                SiteLoadTime("http://www.jpl.nasa.gov/", browser, driver);
                SiteLoadTime("https://www.khanacademy.org/", browser, driver);
                SiteLoadTime("https://scratch.mit.edu/", browser, driver);
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

        static void SiteLoadTime(string site, string browser, RemoteWebDriver driver)
        {
            string path = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                driver.Manage().Window.Maximize();
                driver.Url = site;
                if (browser == "Edge")
                {
                    Thread.Sleep(2000);
                }
                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},{1},{2},{3},", site, browser, result, i + 1));
                }
            }
        }

        static void Octane(string browser, RemoteWebDriver driver)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://chromium.github.io/octane/";
                driver.ExecuteScript("document.getElementById('run-octane').click();");
                while (!driver.FindElementById("main-banner").Text.Contains("Octane Score:"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("main-banner").Text.Substring(14);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Octane,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        static void SunSpider(string browser, RemoteWebDriver driver)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);
                if (browser == "Edge")
                {
                    result = driver.FindElementById("console").Text.Substring(165, 5);
                }
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("SunSpider,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        static void JetStream(string browser, RemoteWebDriver driver)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://browserbench.org/JetStream/";
                driver.ExecuteScript("document.getElementById('status').children[0].click();");
                while (driver.FindElementById("status").Text != "Test Again")
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("results-cell-geomean").Text.Substring(0, 6);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("JetStream,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        static void WebXPRT(string browser, RemoteWebDriver driver)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/";
                driver.FindElementById("imgRunAll").Click();
                while (driver.Url != "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/results.php?c=0")
                {
                    Thread.Sleep(10000);
                }
                var result =  driver.FindElementByCssSelector(".resultsOval>.scoreText").Text;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("WebXPRT,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        static void OORTOnline(string browser, RemoteWebDriver driver)
        {
            var directory = Directory.CreateDirectory(@"C:\EduPerfTests\");
            
            for (int i = 0; i < iterations; i++)
            {
                driver.Manage().Window.Maximize();
                driver.Url = "http://oortonline.gl/#run";
                Thread.Sleep(600000);
                driver.GetScreenshot().SaveAsFile(Path.Combine(directory.FullName, string.Format(@"{0}{1}.png", browser, i+1)), ImageFormat.Png);
            }
        }
    }
}
