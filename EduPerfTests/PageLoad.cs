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
                    try
                    {
                        Console.WriteLine($"Launching {browser}.");
                        using (var driver = LaunchDriver(browser))
                        {
                            this.SiteLoadTime(site, browser, driver, iterations);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unexpected exception from running browser: {e}");
                    }
                }
            }
        }

        private void SiteLoadTime(string site, Browser browser, RemoteWebDriver driver, int iterations)
        {
            string fullUrl = $"{_scheme}://{site}";

            InitializeDriver(driver);

            const int MaxRetries = 5;
            int retries = 0;
            for (int i = 0; i < iterations; i++)
            {
                string error = string.Empty;
                long result = -1;
                try
                {
                    Console.WriteLine($"Recording Site Load Time For '{fullUrl}'");

                    long navStarted = _watch.ElapsedMilliseconds;

                    // driver.Url = site seems to fail in Chrome 50 and later. If scheme is added, navigations work.
                    driver.Navigate().GoToUrl(fullUrl);

                    // Added as a potential workaround in case pages are populating loadEventEnd before page is fully loaded.
                    Thread.Sleep(2000);

                    long loadEventEnd;
                    while (true)
                    {
                        // This loop is required because Microsoft Edge and Firefox both sometimes return from .Url earlier than they should
                        var loadEventObject = driver.ExecuteScript("return performance.timing.loadEventEnd;");
                        loadEventEnd = Convert.ToInt64(loadEventObject);
                        if (loadEventEnd != 0) break;

                        // We have been seeing a lot of these on specific sites. 
                        error = "driver.Navigate().GoToUrl(fullUrl); is returning early";
                        if (_watch.ElapsedMilliseconds - navStarted > 50000)
                        {
                            throw new TimeoutException("Navigate took > 50 seconds.");
                        }

                        Thread.Sleep(1000);
                    }

                    var navStartObject = driver.ExecuteScript("return performance.timing.navigationStart;");
                    long navStart = Convert.ToInt64(navStartObject);
                    result = loadEventEnd - navStart;
                }
                catch (TimeoutException e)
                {
                    error = e.Message;
                    i = i - 1;
                    retries++;
                    if (retries > MaxRetries)
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                    i = i - 1;
                    retries++;
                    if (retries > MaxRetries)
                    {
                        throw;
                    }
                }
                finally
                {
                    _perfLog.WriteToLog($"{fullUrl},{browser},{result},{i + 1},{error}");
                }
            }
        }
    }
}