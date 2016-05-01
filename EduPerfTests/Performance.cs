using OpenQA.Selenium.Remote;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using static EduPerfTests.Utils;

namespace EduPerfTests
{
    public class Performance
    {
        public PerformanceLog _perfLog;

        public Performance(PerformanceLog perfLog)
        {
            _perfLog = perfLog;
        }
        public void Octane(Browser browser, RemoteWebDriver driver, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                driver.Url = "http://chromium.github.io/octane/";

                // Clicking in ExecuteScript to avoid border radius Web Driver Interop bug
                driver.ExecuteScript("document.getElementById('run-octane').click();");
                while (!driver.FindElementById("main-banner").Text.Contains("Octane Score:"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("main-banner").Text.Substring(14);
                _perfLog.WriteToLog($"Octane,{browser},{result},{i + 1},");
            }
        }

        public void SunSpider(Browser browser, RemoteWebDriver driver, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                driver.Url = "https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html";
                while (driver.Url.Equals("https://webkit.org/perf/sunspider-1.0.2/sunspider-1.0.2/driver.html"))
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("console").Text.Substring(161, 5);

                if (browser == Browser.MicrosoftEdge)
                {
                    result = driver.FindElementById("console").Text.Substring(165, 5);
                }

                _perfLog.WriteToLog($"SunSpider,{browser},{result},{i + 1},");
            }
        }

        public void JetStream(Browser browser, RemoteWebDriver driver, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                driver.Url = "http://browserbench.org/JetStream/";
                Thread.Sleep(2000);
                driver.ExecuteScript("document.getElementById('status').children[0].click();");
                Thread.Sleep(2000);
                while (driver.FindElementById("status").Text != "Test Again")
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementById("results-cell-geomean").Text.Substring(0, 6);
                _perfLog.WriteToLog($"JetStream,{browser},{result},{i + 1},");
            }
        }

        public void WebXPRT(Browser browser, RemoteWebDriver driver, int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                driver.Url = "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/";
                driver.FindElementById("imgRunAll").Click();
                while (driver.Url !=
                        "http://www.principledtechnologies.com/benchmarkxprt/webxprt/2015/v19982/results.php?c=0")
                {
                    Thread.Sleep(10000);
                }
                var result = driver.FindElementByCssSelector(".resultsOval>.scoreText").Text;
                _perfLog.WriteToLog($"WebXPRT,{browser},{result},{i + 1},");
            }
        }

        public void OORTOnline(Browser browser, RemoteWebDriver driver, int iterations)
        {
            var directory = Directory.CreateDirectory(@"C:\EduPerfTests\");

            for (var i = 0; i < iterations; i++)
            {
                driver.Url = "http://oortonline.gl/#run";
                Thread.Sleep(600000);
                driver.GetScreenshot().SaveAsFile(Path.Combine(directory.FullName, $@"{browser}{i + 1}.png"), ImageFormat.Png);
            }
        }
    }
}