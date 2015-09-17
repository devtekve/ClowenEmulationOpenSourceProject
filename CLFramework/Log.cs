using System;
using System.IO;

namespace CLFramework
{
    public class Log
    {
        public static void Exception(Exception Ex)
        {
            Console.WriteLine($"Error:{Ex.Message}");
            ExceptionWrite(Ex.ToString());
        }
        public static void Exception(string Command, Exception Ex)
        {
            Console.WriteLine($"Error:{Ex.ToString()} \n Cmnd:{Command}");
            ExceptionWrite($"Error:{Ex.ToString()} \n Cmnd:{Command}");
        }
        public static void Exception(string Ex)
        {
            Console.WriteLine(Ex);
            ExceptionWrite(Ex);
        }
        private static void ExceptionWrite(string debuginfo)
        {
            using (TextWriter writer = File.CreateText(Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("dd.MM.yyyy  -   HH.mm.ss") + ".log"))
            {
                writer.WriteLineAsync(debuginfo);
            }
        }
    }
}
