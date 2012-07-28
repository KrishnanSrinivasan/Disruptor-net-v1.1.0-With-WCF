using System;
using System.IO;

namespace Common
{
    /// <summary>
    /// Utility class to Log info onto a log file.
    /// </summary>
    public struct Log
    {
        private const String LogFile = "Log.txt";
        
        public static void Write(string info)
        {
            Console.WriteLine(info);

            using (FileStream f = File.Open(LogFile, FileMode.Append))
            {
                StreamWriter w = new StreamWriter(f);
                w.WriteLine(info);
                w.Close();
            }
        }
    }
}
