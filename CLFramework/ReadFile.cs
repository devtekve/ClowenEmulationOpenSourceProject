using System;
using System.IO;
using System.Collections.Generic;

namespace CLFramework
{
    public class TxtFile
    {
        public static List<string> lines = new List<string>();
        public static string[] commands = new string[250];
        public static int amountLine;
        public static void Dispose()
        {
            commands = null;
            amountLine = 0;
            lines.Clear();
        }
        public static void ReadFromFile(string filename, char splitType)
        {
            Dispose();

            StreamReader SR;
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @filename))
                {
                    SR = File.OpenText(Environment.CurrentDirectory + @filename);
                    if (SR == null)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        string curLine = string.Empty;
                        while ((curLine = SR.ReadLine()) != null)
                        {
                            lines.Add(curLine);
                        }
                        amountLine = lines.Count;
                        SR.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("FW.File.TxtFile.ReadFromFile: ", ex);
            }
        }
    }
}
