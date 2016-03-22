using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace EduPerfTests
{
    public class PageLoad
    {
        public static void SiteLoadTime(string site, string browser, RemoteWebDriver driver, int iterations)
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
                var timing = driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;");
                var result = Convert.ToInt64(timing);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("{0},{1},{2},{3},", site, browser, result, i + 1));
                }
            }
        }
    }
}
