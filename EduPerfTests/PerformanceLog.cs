using System;
using System.IO;

namespace EduPerfTests
{
    public class PerformanceLog : IDisposable
    {
        StreamWriter logFile;
        bool logInitialized = false;
        bool wroteFirstResult = false;

        public PerformanceLog(string logFileName)
        {
            var loadPath = Utils.LogFileLocation(logFileName);
            Console.WriteLine("Storing results to {0}", loadPath);
            logFile = new StreamWriter(loadPath);
        }

        public void WriteToLog(string logLine)
        {
            if (!wroteFirstResult)
            {
                wroteFirstResult = true;
                Console.WriteLine("Writing first result to log");
            }

            logFile.WriteLine(logLine);
        }

        public void Dispose()
        {
            if (logFile != null)
            {
                Console.WriteLine("Disposing log");
                logFile.Dispose();
                logFile = null;
            }
        }

        public void InitializeLog(string firstLine)
        {
            if (!logInitialized)
            {
                Console.WriteLine("Initializing log");
                logFile.WriteLine(firstLine);
                logInitialized = true;
            }
        }
    }
}
