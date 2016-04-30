using OpenQA.Selenium.Remote;
using System;
using System.Threading;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    public class PageLoad
    {
        private PerformanceLog _perfLog;

        public PageLoad(PerformanceLog perfLog)
        {
            _perfLog = perfLog;
        }

        public void SiteLoadTime(string site, Browser browser, RemoteWebDriver driver, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine("Recording Site Load Time For-" + site);

                driver.Url = site;

                if (browser == Browser.InternetExplorer)
                {
                    Thread.Sleep(500);
                }

                long loadEventEnd;
                while (true)
                {
                    // This loop is required because Microsoft Edge and Firefox both sometimes return from .Url earlier than they should
                    var loadEventObject = driver.ExecuteScript("return performance.timing.loadEventEnd;");
                    loadEventEnd = Convert.ToInt64(loadEventObject);
                    if (loadEventEnd != 0) break;
                    Console.WriteLine("Url is returning early. Please investigate!");
                }

                var navStartObject = driver.ExecuteScript("return performance.timing.navigationStart;");
                long navStart = Convert.ToInt64(navStartObject);
                var result = loadEventEnd - navStart;
                _perfLog.WriteToLog($"{site},{browser},{result},{i + 1}");
            }
        }
    }
}