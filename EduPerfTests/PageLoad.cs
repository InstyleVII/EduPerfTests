using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    public class PageLoad
    {
        private PerformanceLog _perfLog;
        private string _scheme;
        private Stopwatch _watch;

        public PageLoad(string scheme)
        {
            _perfLog = new PerformanceLog("pageloadtestresults");
            _perfLog.InitializeLog("Site,Browser,Result (ms),Iteration,Error");
            _scheme = scheme;
            _watch = Stopwatch.StartNew();
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
                long result = -1;
                try
                {
                    Console.WriteLine($"Recording Site Load Time For '{fullUrl}'");

                    // driver.Url = site seems to fail in Chrome 50 and later. If scheme is added, navigations work.
                    driver.Navigate().GoToUrl(fullUrl);

                    if (browser == Browser.InternetExplorer)
                    {
                        Thread.Sleep(500);
                    }

                    long loadEventEnd;
                    long navStarted = _watch.ElapsedMilliseconds;
                    while (true)
                    {
                        // This loop is required because Microsoft Edge and Firefox both sometimes return from .Url earlier than they should
                        var loadEventObject = driver.ExecuteScript("return performance.timing.loadEventEnd;");
                        loadEventEnd = Convert.ToInt64(loadEventObject);
                        if (loadEventEnd != 0) break;
                        error = "driver.Navigate().GoToUrl(fullUrl); is returning early";
                        if (_watch.ElapsedMilliseconds - navStarted > 30000)
                        {
                            throw new TimeoutException("Navigate took > 30 seconds.");
                        }
                    }

                    var navStartObject = driver.ExecuteScript("return performance.timing.navigationStart;");
                    long navStart = Convert.ToInt64(navStartObject);
                    result = loadEventEnd - navStart;
                }
                catch (TimeoutException e)
                {
                    error = e.Message;
                }
                catch (Exception e)
                {
                    error = e.Message;
                    throw;
                }
                finally
                {
                    _perfLog.WriteToLog($"{fullUrl},{browser},{result},{i + 1},{error}");
                }
            }
        }
    }
}