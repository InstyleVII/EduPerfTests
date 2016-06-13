using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace EduPerfTests
{
    public static class Utils
    {
        [Flags]
        public enum Browser
        {
            None = 0,
            InternetExplorer = 1,
            Chrome = 2,
            Firefox = 4,
            MicrosoftEdge = 8,
            All = Chrome | Firefox | MicrosoftEdge
        }

        /// <summary>
        /// This method takes in a fileName then looks for it in the current directory.
        /// </summary>
        /// <param name="fileName">The file name to test</param>
        /// <returns></returns>
        public static string LogFileLocation(string fileName)
        {
            var tempFileName = fileName;
            var currentPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\";
            currentPath += @"perfResults\";
            Directory.CreateDirectory(currentPath);
            var newFilePath = currentPath + $@"{tempFileName}.csv";

            int count = 0;
            while (File.Exists(newFilePath))
            {
                tempFileName = fileName + count.ToString();
                newFilePath = currentPath + $@"{tempFileName}.csv";
                count++;
            }

            return newFilePath;
        }

        public static IEnumerable<Browser> ChosenBrowsers(this Browser flags)
        {
            ulong flag = 1;
            foreach (Browser value in Enum.GetValues(typeof(Browser)).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        public static RemoteWebDriver LaunchDriver(Browser browser)
        {
            AuditDriver();

            Console.WriteLine($"Starting browser: {browser}");
            try
            {
                RemoteWebDriver driver;

                switch (browser)
                {
                    case Browser.Firefox:
                        driver = new FirefoxDriver();
                        break;
                    case Browser.Chrome:
                        driver = new ChromeDriver();
                        break;
                    case Browser.InternetExplorer:
                        driver = new InternetExplorerDriver();
                        break;
                    case Browser.MicrosoftEdge:
                        driver = new EdgeDriver();

                        // This sleep allows Microsoft Edge Anniversary Update to workaround a bug where it doesn't properly wait for about:blank   
                        // This bug COULD occur in Fall Update 2016 but would be much rarer.
                        // It should be fixed ~May 15th in Anniversary Update. 5129594                  
                        Thread.Sleep(2000);
                        break;
                    default:
                        throw new Exception($"Unexpected browser: {browser}");
                }

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch {browser}, ERROR: {ex.Message}");
                throw;
            }
        }

        private static void AuditDriver()
        {
            try
            {
                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.IndexOf("chrome", StringComparison.OrdinalIgnoreCase) > -1
                    || process.ProcessName.IndexOf("microsoftwebdriver", StringComparison.OrdinalIgnoreCase) > -1)
                    //|| process.ProcessName.IndexOf("microsoftedge", StringComparison.OrdinalIgnoreCase) > -1
                    {
                        Console.WriteLine($"Found unexpected process, '{process.ProcessName}', when all browsers should be closed. Killing it.");
                        process.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hit an exception in AuditDriver: {e}");
            }
        }

        public static void InitializeDriver(RemoteWebDriver driver)
        { 
                driver.Manage().Window.Maximize();

                // It appears visually that some browsers may not complete all work before returning from maximize. Sleeping for paranois.
                // We should test the navigate times with and without sleep to know for certain.
                Thread.Sleep(1000);
        }
    }
}
