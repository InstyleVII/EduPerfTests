using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace EduPerfTests
{
    public class PageLoad
    {
        public static void SiteLoadTime(string site, string browser, RemoteWebDriver driver, int iterations)
        {
            var path = string.Format(@"{0}\pageloadresults.csv", Directory.GetCurrentDirectory());
            bool retry = false;
            for (int i = 0; i < iterations; i++)
            {                
                if (retry)
                {
                    Console.WriteLine("retrying");
                    i--;
                }
                else
                {
                    Console.WriteLine("siteloadtime-" + site);
                }

                
                driver.Url = site;

                if (retry)
                {
                    Thread.Sleep(15000);
                }

                Thread.Sleep(5000);

                try
                {
                    var timing = driver.ExecuteScript("return performance.timing.loadEventEnd - performance.timing.navigationStart;");
                    var result = Convert.ToInt64(timing);
                    using (StreamWriter file = new StreamWriter(path, true))
                    {
                        file.WriteLine(string.Format("{0},{1},{2},{3},{4}", site, browser, result, i + 1, retry));
                    }

                    retry = false;
                }
                catch (WebDriverException e)
                {
                    if (retry)
                    {
                        throw;
                    }

                    if (e.Message.Contains("timed out after 60 seconds"))
                    {
                        retry = true;
                    }
                }
            }
        }
    }
}