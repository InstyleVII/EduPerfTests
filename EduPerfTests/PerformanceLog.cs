using System;
using System.IO;

namespace EduPerfTests
{
    public class PerformanceLog : IDisposable
    {
        StreamWriter logFile;
        bool logInitialized = false;

        public PerformanceLog(string logFileName)
        {
            var loadPath = Utils.LogFileLocation(logFileName);
            logFile = new StreamWriter(loadPath);
        }

        public void WriteToLog(string logLine)
        {
            logFile.WriteLine(logLine);
        }

        public void Dispose()
        {
            if (logFile != null)
            {
                logFile.Dispose();
                logFile = null;
            }
        }

        public void InitializeLog(string firstLine)
        {
            if (!logInitialized)
            {
                logFile.WriteLine(firstLine);
                logInitialized = true;
            }
        }
    }
}
