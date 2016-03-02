using System;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.IO;
using System.Threading;

namespace EduPerfTests
{
    class Program
    {
        static void Main(string[] args)
        {

            Octane(1);
            SunSpider(1);
            SiteLoadTime("http://www.bing.com/mapspreview", 1);
            SiteLoadTime("https://www.brainpop.com/", 1);
            SiteLoadTime("https://www.edmodo.com/", 1);
            SiteLoadTime("https://www.google.com/maps", 1);
            SiteLoadTime("https://www.biodigital.com/", 1);
            SiteLoadTime("http://www.jpl.nasa.gov/", 1);
            SiteLoadTime("https://www.khanacademy.org/", 1);
            SiteLoadTime("https://scratch.mit.edu/", 1);
        }

        static void SiteLoadTime(string site, int pass)
        {
            string path = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Site,Browser,Result (ms),Iteration,");
            }

            // Edge Tests
            for (int i = 0; i < pass; i++)
            {
                var driver = new EdgeDriver();
                driver.Manage().Window.Maximize();
                driver.Url = site;
                Thread.Sleep(2000);

                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},Edge,{1},{2},", site, result, i + 1));
                }
                driver.Quit();
            }

            // Chrome Tests
            for (int i = 0; i < pass; i++)
            {
                var driver = new ChromeDriver();
                driver.Manage().Window.Maximize();
                driver.Url = site;
                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},Chrome,{1},{2},", site, result, i + 1));
                }
                driver.Quit();
            }

            // Firefox Tests
            for (int i = 0; i < pass; i++)
            {
                var driver = new FirefoxDriver();
                driver.Manage().Window.Maximize();
                driver.Url = site;
                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},Firefox,{1},{2},", site, result, i + 1));
                }
                driver.Quit();
            }

            // Internet Explorer Tests
            for (int i = 0; i < pass; i++)
            {
                var driver = new InternetExplorerDriver();
                driver.Manage().Window.Maximize();
                driver.Url = site;
                var result = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},Internet Explorer,{1},{2},", site, result, i + 1));
                }
                driver.Quit();
            }
        }

        static void Octane(int pass)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Benchmark,Browser,Result,Iteration");
            }

            // Firefox Runs
            for (int i = 0; i < pass; i++)
            {
                var driver = new FirefoxDriver();
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
                    file.WriteLine(string.Format("Octane,Firefox,{0},{1},", result, i + 1));
                }
                driver.Quit();
            }

            // Internet Explorer Runs
            for (int i = 0; i < pass; i++)
            {
                var driver = new InternetExplorerDriver();
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
                    file.WriteLine(string.Format("Octane,Internet Explorer,{0},{1},", result, i + 1));
                }
                driver.Quit();
            }
        }

        static void SunSpider(int pass)
        {
            string path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            // Firefox Runs
            for (int i = 0; i < pass; i++)
            {
                var driver = new FirefoxDriver();
                driver.Manage().Window.Maximize();
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(1000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("SunSpider,Firefox,{0},{1},", result, i + 1));
                }
                driver.Quit();
            }

            // Internet Explorer Runs
            for (int i = 0; i < pass; i++)
            {
                var driver = new InternetExplorerDriver();
                driver.Manage().Window.Maximize();
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(1000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("SunSpider,Firefox,{0},{1},", result, i + 1));
                }
                driver.Quit();
            }
        }
    }
}
