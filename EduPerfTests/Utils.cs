﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// This method takes in a fileName then looks for it int he current directory.
        /// </summary>
        /// <param name="fileName">The file name to test</param>
        /// <returns></returns>
        public static string TestFileLocation(string fileName)
        {
            var tempFileName = fileName;
            var currentPath = string.Format(@"{0}\", Directory.GetCurrentDirectory());
            var newFilePath = currentPath + string.Format(@"{0}.csv", tempFileName);

            int count = 0;
            while (File.Exists(newFilePath))
            {
                tempFileName = fileName + count.ToString();
                newFilePath = currentPath + string.Format(@"{0}.csv", tempFileName);
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
    }
}