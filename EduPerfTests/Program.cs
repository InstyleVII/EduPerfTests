using System;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Threading;

namespace EduPerfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = string.Format(@"{0}\results.txt", Directory.GetCurrentDirectory());
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine("Beginning Education Performance Tests\r\n");
            }
            SiteLoadTime(path, "http://www.bing.com/mapspreview");
            SiteLoadTime(path, "https://www.brainpop.com/");
            SiteLoadTime(path, "https://www.edmodo.com/");
            SiteLoadTime(path, "https://www.google.com/maps");
            SiteLoadTime(path, "https://www.biodigital.com/");
            SiteLoadTime(path, "http://www.jpl.nasa.gov/");
            SiteLoadTime(path, "https://www.khanacademy.org/");
            SiteLoadTime(path, "https://scratch.mit.edu/");
        }

        static void SiteLoadTime(string path, string website)
        {
            var total = 0;

            // Edge Tests
            for (int i = 0; i < 10; i++)
            {
                var driver = new EdgeDriver();
                driver.Manage().Window.Maximize();
                driver.Url = website;
                Thread.Sleep(2000);
                var loadTime = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                total += loadTime;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Edge pass {0} on {1} took {2}ms", i + 1, website, loadTime));
                }
                driver.Quit();
            }

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.Write(string.Format("Edge's average for {0} was {1}ms\r\n\r\n", website, total / 10));
            }

            total = 0;

            // Chrome Tests
            for (int i = 0; i < 10; i++)
            {
                var driver = new ChromeDriver();
                driver.Manage().Window.Maximize();
                driver.Url = website;
                Thread.Sleep(2000);
                var loadTime = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                total += loadTime;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Chrome pass {0} on {1} took {2}ms", i + 1, website, loadTime));
                }
                driver.Quit();
            }

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.Write(string.Format("Chrome's average for {0} was {1}ms\r\n\r\n", website, total / 10));
            }

            total = 0;

            // Firefox Tests
            for (int i = 0; i < 10; i++)
            {
                var driver = new FirefoxDriver();
                driver.Manage().Window.Maximize();
                driver.Url = website;
                Thread.Sleep(2000);
                var loadTime = Convert.ToInt32(driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;"));
                total += loadTime;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Firefox pass {0} on {1} took {2}ms", i + 1, website, loadTime));
                }
                driver.Quit();
            }

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.Write(string.Format("Firefox's average for {0} was {1}ms\r\n\r\n", website, total / 10));
            }
        }
    }
}
