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
                    string processName = ProcessNameFromBrowser();
                    long startSet = 0, afterSet = 0;

                    // we want to iterate 5 times and get the average for each perf value
                    for (var i = 0; i < iterations; i++)
                    {
                        long privateWorkingSetBefore = 0, privateWorkingSetAfter = 0;

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
                            privateWorkingSetBefore = new BrowserMemory(processName).CurrentPrivateWorkingSet;

                            // wait 10 seconds
                            Thread.Sleep(10000);

                            // take a snapshot
                            privateWorkingSetAfter = new BrowserMemory(processName).CurrentPrivateWorkingSet;

                            // clear cache and cookies
                            ClearCookiesAndCache(driver);
                        }

                        // do work with the memory snapshots
                        startSet += privateWorkingSetBefore / 1024;
                        afterSet += privateWorkingSetAfter / 1024;
                    }

                    long startAverage = 0, endAverage = 0;
                    startAverage = startSet / iterations;
                    endAverage = afterSet / iterations;
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
            Thread.Sleep(1000);
            try
            {
                driver.Manage().Cookies.DeleteAllCookies();
            }
            catch (Exception e)
            {
                Console.WriteLine("Clearing Cookies failed due to " + e.ToString());
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
