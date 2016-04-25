using OpenQA.Selenium.Remote;
using System;
using System.IO;
using System.Threading;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    public class PageLoad : IDisposable
    {
        StreamWriter logFile;
        bool logInitialized = false;

        public PageLoad()
        {
            const string pageLoadResultsFileName = "pageloadresults";
            var loadPath = Utils.TestFileLocation(pageLoadResultsFileName);
            logFile = new StreamWriter(loadPath);            
        }

        public void Dispose()
        {
            if (logFile != null)
            {
                logFile.Dispose();
                logFile = null;
            }
        }

        void initializeLog()
        {
            if (!logInitialized)
            {
                logFile.WriteLine("Site,Browser,Result (ms),Iteration");
                logInitialized = true;
            }
        }

        public void SiteLoadTime(string site, Browser browser, RemoteWebDriver driver, int iterations)
        {
            initializeLog();

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
                logFile.WriteLine(string.Format("{0},{1},{2},{3}", site, browser, result, i + 1));
            }
        }


    }
}