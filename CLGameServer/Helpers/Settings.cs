using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CLFramework;

namespace CLGameServer.Helpers
{
    class Settings
    {
        public static ushort ClientVersion = 321, ServerCapacity = 2;
        public class Rate { public static int Gold = 1,Item = 1,Experience = 1,SkillPoint = 1,ItemSox = 1,Alchemy = 1,MonsterSpawn = 1,Elixir = 1,ETC = 1; }
        public static Dictionary<int, SrevoServerInfo> LSList = new Dictionary<int, SrevoServerInfo>();
        public class SrevoServerInfo
        {
            public ushort id = 0;
            public string ip = "";
            public string code = "";
            public ushort ipcport = 15779;
            public DateTime lastPing = DateTime.MinValue;
        }
        public static bool LoadServers(string serverFile, ushort defaultPort)
        {
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\Settings\" + serverFile))
                {
                    Ini ini = new Ini(Environment.CurrentDirectory + @"\Settings\" + serverFile);
                    string[] sList = null;
                    sList = ini.GetEntryNames("SERVERS");
                    if (sList != null && sList.Length > 0)
                    {

                        foreach (string sectname in sList)
                        {
                            string sName = ini.GetValue("SERVERS", sectname, "");
                            SrevoServerInfo SServerInfo = new SrevoServerInfo();
                            SServerInfo.id = Convert.ToUInt16(ini.GetValue(sName, "id", 0));
                            SServerInfo.ip = ini.GetValue(sName, "ip", "");
                            SServerInfo.ipcport = Convert.ToUInt16(ini.GetValue(sName, "ipcport", defaultPort));
                            SServerInfo.code = ini.GetValue(sName, "code", "");
                            if (SServerInfo.ip == "" || SServerInfo.id == 0 || SServerInfo.ipcport == 0 || LSList.ContainsKey(SServerInfo.id))
                            {
                                Log.Exception(string.Format("IPC: Error on Server \"{0}\" in {1}: Mandatory field missing or id already in use!", sName, serverFile));
                                SServerInfo = null;
                            }
                            else
                            {
                                LSList.Add(SServerInfo.id, SServerInfo);
                            }
                        }
                    }
                    if (LSList.Count > 0)
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("------------------------------------[SERVER]------------------------------------");
                        string defServer = "Server";
                        if (LSList.Count > 1) defServer = "Servers";
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("                          Added:");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" {0} ", LSList.Count());
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0} to the list", defServer);
                        Console.WriteLine();
                        return true;
                    }
                    else
                    {
                        Log.Exception("[IPC] Info: No LoginServers configured, using a default local LoginServer.");
                        return false;
                    }
                    sList = null;
                    ini = null;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                
            }
            return false;
        }
    }
}
