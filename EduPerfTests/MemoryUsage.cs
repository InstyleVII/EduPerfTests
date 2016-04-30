using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    class MemoryUsage : Program
    {
        private PerformanceLog _perfLog;
        private Browser _browser; 

        public MemoryUsage()
        {
            _perfLog = new PerformanceLog("memoryusagetestresults");
            _perfLog.InitializeLog("Site,StartMemoryAverage,EndMemoryAverage,Delta");
        }

        public void RunMemoryUsageTests(List<string> pageLoadSites, Browser browser, int iterations)
        {
            _browser = browser;

            using (_perfLog)
            {
                foreach (var site in pageLoadSites)
                {
                    var store = new List<MemoryData>();
                    string processName = ProcessNameFromBrowser();

                    // we want to iterate 5 times and get the average for each perf value
                    for (var i = 0; i < iterations; i++)
                    {
                        BrowserMemory b, b2;
                        var memData = new MemoryData();

                        // launch the browser
                        using (var driver = LaunchDriver(_browser))
                        {
                            // browse to the page
                            driver.Url = site;

                            while (true) // wait for the page to load
                            {
                                if (LoadEvent(driver) != 0) break;
                            }

                            // take a snapshot
                            b = new BrowserMemory(processName);

                            // wait 10 seconds
                            Thread.Sleep(10000);

                            // take a snapshot
                            b2 = new BrowserMemory(processName);

                            // clear cache and cookies
                            ClearCookiesAndCache(driver);
                        }

                        // do work with the memory snapshots
                        var startSet = b.CurrentPrivateWorkingSet / 1024;
                        var afterSet = b2.CurrentPrivateWorkingSet / 1024;
                        var deltaSet = afterSet - startSet;
                        memData.Iteration = i;
                        memData.StartMemory = startSet; // default is KB
                        memData.EndMemory = afterSet;
                        store.Add(memData);
                    }

                    // have 5 iterations in memData to get average
                    long startTimes = 0, endTimes = 0;
                    foreach (var item in store)
                    {
                        startTimes += item.StartMemory;
                        endTimes += item.EndMemory;
                    }

                    long startAverage = 0, endAverage = 0;
                    startAverage = startTimes / iterations;
                    endAverage = endTimes / iterations;
                    long delta = endAverage - startAverage;

                    _perfLog.WriteToLog(
                        site + "," + 
                        startAverage.ToString() + "," + 
                        endAverage.ToString() + "," + 
                        delta.ToString());
                }
            }

        }

        private string ProcessNameFromBrowser()
        {
            string processName = string.Empty;

            switch (_browser)
            {
                case Browser.MicrosoftEdge:
                    processName = "MicrosoftEdge";
                    break;
                case Browser.Chrome:
                    processName = "chrome.exe";
                    break;
                case Browser.Firefox:
                    processName = "firefox.exe";
                    break;
                default:
                    processName = "MicrosoftEdge";
                    break;
            }

            return processName;
        }
        private static long LoadEvent(RemoteWebDriver driver)
        {
            // This loop is required because Microsoft Edge and Firefox both sometimes return from .Url earlier than they should
            var loadEventObject = driver.ExecuteScript("return performance.timing.loadEventEnd;");
            return Convert.ToInt64(loadEventObject);
        }
        private static void ClearCookiesAndCache(RemoteWebDriver driver)
        {
            try
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            catch (Exception)
            {
                // TODO
            }

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "RunDll32.exe";
            p.StartInfo.Arguments = "InetCpl.cpl,ClearMyTracksByProcess 2";
            p.Start();
            p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
