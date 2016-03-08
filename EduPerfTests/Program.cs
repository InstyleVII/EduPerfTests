using System;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using OpenQA.Selenium.Remote;

namespace EduPerfTests
{
    class Program
    {
        static RemoteWebDriver driver;
        static string[] browsers = { "chrome", "firefox", "ie", "edge" };
        static int iterations = 1;

        static void Main(string[] args)
        {
            string perfPath = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(perfPath))
            {
                file.WriteLine("Benchmark,Browser,Result,Iteration");
            }

            foreach (var browser in browsers)
            {
                Octane(browser);
                WebXPRT(browser);
                SunSpider(browser);
                JetStream(browser);
                OORTOnline(browser);
            }

            string loadPath = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(loadPath))
            {
                file.WriteLine("Site,Browser,Result (ms),Iteration,");
            }

            foreach (var browser in browsers)
            {
                SiteLoadTime("http://www.bing.com/mapspreview", browser);
                SiteLoadTime("https://www.brainpop.com/", browser);
                SiteLoadTime("https://www.edmodo.com/", browser);
                SiteLoadTime("https://www.google.com/maps", browser);
                SiteLoadTime("https://www.biodigital.com/", browser);
                SiteLoadTime("http://www.jpl.nasa.gov/", browser);
                SiteLoadTime("https://www.khanacademy.org/", browser);
                SiteLoadTime("https://scratch.mit.edu/", browser);
            }
        }

        static RemoteWebDriver LaunchDriver(string browser)
        {
            RemoteWebDriver driver = null;

            try
            {
                switch (browser)
                {
                    case "firefox":
                        driver = new FirefoxDriver();
                        break;
                    case "chrome":
                        driver = new ChromeDriver();
                        break;
                    case "ie":
                        driver = new InternetExplorerDriver();
                        break;
                    default:
                        driver = new EdgeDriver();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to launch {0}, ERROR: {1}", browser, ex.Message);
            }

            return driver;
        }

        static void SiteLoadTime(string site, string browser)
        {
            string path = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                var driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().SetPageLoadTimeout(new TimeSpan(0, 0, 0, 0, 5000));
                driver.Url = site;
                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},{1},{2},{3},", site, browser, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void Octane(string browser)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Url = "http://chromium.github.io/octane/";
                driver.ExecuteScript("document.getElementById('run-octane').click();");
                while (!driver.FindElementById("main-banner").Text.Contains("Octane Score:"))
                {
                    Thread.Sleep(1000);
                }
                var result = driver.FindElementById("main-banner").Text.Substring(14);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Octane,{0},{1},{2},", browser, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void SunSpider(string browser)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                var driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(1000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("SunSpider,{0},{1},{2},", browser, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void JetStream(string browser)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            
            for (int i = 0; i < iterations; i++)
            {
                var driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Url = "http://browserbench.org/JetStream/";
                driver.ExecuteScript("document.getElementById('status').children[0].click();");
                while (driver.FindElementById("status").Text != "Test Again")
                {
                    Thread.Sleep(1000);
                }
                var result = driver.FindElementById("results-cell-geomean").Text.Substring(0, 6);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("JetStream,{0},{1},{2},", browser, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void WebXPRT(string browser)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Url = "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/";
                driver.FindElementById("imgRunAll").Click();
                while (driver.Url != "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/results.php?c=0")
                {
                    Thread.Sleep(1000);
                }
                var result =  driver.FindElementByCssSelector(".resultsOval>.scoreText").Text;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("WebXPRT,{0},{1},{2},", browser, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void OORTOnline(string browser)
        {
            var directory = Directory.CreateDirectory(@"C:\EduPerfTests\");
            
            for (int i = 0; i < iterations; i++)
            {
                var driver = LaunchDriver(browser);
                driver.Manage().Window.Maximize();
                driver.Url = "http://oortonline.gl/#run";
                Thread.Sleep(600000);
                driver.GetScreenshot().SaveAsFile(Path.Combine(directory.FullName, string.Format(@"{0}{1}.png", browser, i+1)), ImageFormat.Png);
                driver.Quit();
            }
        }
    }
}
