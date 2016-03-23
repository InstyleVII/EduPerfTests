using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using OpenQA.Selenium.Remote;

namespace EduPerfTests
{
    public class Performance
    {
        public static void Octane(string browser, RemoteWebDriver driver, int iterations)
        {
            var path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://chromium.github.io/octane/";
                driver.ExecuteScript("document.getElementById('run-octane').click();");
                while (!driver.FindElementById("main-banner").Text.Contains("Octane Score:"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("main-banner").Text.Substring(14);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("Octane,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        public static void SunSpider(string browser, RemoteWebDriver driver, int iterations)
        {
            var path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);
                if (browser == "Edge")
                {
                    result = driver.FindElementById("console").Text.Substring(165, 5);
                }
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("SunSpider,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        public static void JetStream(string browser, RemoteWebDriver driver, int iterations)
        {
            var path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://browserbench.org/JetStream/";
                driver.ExecuteScript("document.getElementById('status').children[0].click();");
                while (driver.FindElementById("status").Text != "Test Again")
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("results-cell-geomean").Text.Substring(0, 6);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("JetStream,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        public static void WebXPRT(string browser, RemoteWebDriver driver, int iterations)
        {
            var path = string.Format(@"{0}\performancetestresults.csv", Directory.GetCurrentDirectory());

            for (int i = 0; i < iterations; i++)
            {
                driver.Url = "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/";
                driver.FindElementById("imgRunAll").Click();
                while (driver.Url != "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/results.php?c=0")
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementByCssSelector(".resultsOval>.scoreText").Text;
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(string.Format("WebXPRT,{0},{1},{2},", browser, result, i + 1));
                }
            }
        }

        public static void OORTOnline(string browser, RemoteWebDriver driver, int iterations)
        {
            var directory = Directory.CreateDirectory(@"C:\EduPerfTests\");

            for (int i = 0; i < iterations; i++)
            {
                driver.Manage().Window.Maximize();
                driver.Url = "http://oortonline.gl/#run";
                Thread.Sleep(600000);
                driver.GetScreenshot().SaveAsFile(Path.Combine(directory.FullName, string.Format(@"{0}{1}.png", browser, i + 1)), ImageFormat.Png);
            }
        }
    }
}