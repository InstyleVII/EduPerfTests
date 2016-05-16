using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Threading;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    public class PageLoad
    {
        private PerformanceLog _perfLog;
        private string _scheme;

        public PageLoad(string scheme)
        {
            _perfLog = new PerformanceLog("pageloadtestresults");
            _perfLog.InitializeLog("Site,Browser,Result (ms),Iteration,Error");
            _scheme = scheme;
        }

        public void LoadPages(List<string> pageLoadSites, Browser chosenBrowsers, int iterations)
        {
            foreach (var site in pageLoadSites)
            {
                foreach (Browser browser in chosenBrowsers.ChosenBrowsers())
                {
                    using (var driver = LaunchDriver(browser))
                    {
                        this.SiteLoadTime(site, browser, driver, iterations);
                    }
                }
            }
        }

        private void SiteLoadTime(string site, Browser browser, RemoteWebDriver driver, int iterations)
        {
            string fullUrl = $"{_scheme}://{site}";
            for (int i = 0; i < iterations; i++)
            {
                string error = string.Empty;
                Console.WriteLine($"Recording Site Load Time For '{fullUrl}'");

                // driver.Url = site seems to fail in Chrome 50 and later. If scheme is added, navigations work.
                driver.Navigate().GoToUrl(fullUrl);

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
                    error = "driver.Navigate().GoToUrl(fullUrl); is returning early";
                }

                var navStartObject = driver.ExecuteScript("return performance.timing.navigationStart;");
                long navStart = Convert.ToInt64(navStartObject);
                var result = loadEventEnd - navStart;
                _perfLog.WriteToLog($"{site},{browser},{result},{i + 1},");
            }
        }
    }
}