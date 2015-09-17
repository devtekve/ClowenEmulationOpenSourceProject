using System;
using System.IO;
using CLFramework;
using Clowen.Definitions;
namespace Clowen
{
    class Settings
    {
        public static void Load()
        {
            #region Folder Check
            if (
                !File.Exists(Environment.CurrentDirectory + @"\Config\LoginServer.ini") &&
                !File.Exists(Environment.CurrentDirectory + @"\Config\IPCServer.ini") &&
                !File.Exists(Environment.CurrentDirectory + @"\Config\GameServer.ini") &&
                !File.Exists(Environment.CurrentDirectory + @"\News")
                )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not find System Files:\n{0} OR \n{1} OR \n{2} OR \nNews Folder", @"\Config\LoginServer.ini", @"\Config\IPCServer.ini", @"\Config\GameServer.ini");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(0);
            }
            else
            {
                Ini settings;
            #endregion
                #region Login Server
                settings = new Ini(Environment.CurrentDirectory + @"\Config\LoginServer.ini");
                Serverdef.Loginserver_PORT = Convert.ToInt32(settings.GetValue("LS_Information", "Port", 15779));
                Serverdef.Loginserver_IP = settings.GetValue("LS_Information", "Ipadress", "").ToString();
				Serverdef.SilkroadClientVersion = settings.GetValue("LS_Information", "Version",0);
				Serverdef.SilkroadClientLocale = (byte)settings.GetValue("LS_Information", "Locale",0);
                settings = null;
                #endregion
                #region IPC Server
                settings = new Ini(Environment.CurrentDirectory + @"\Config\IPCServer.ini");
                Serverdef.IPCPort = Convert.ToUInt16(settings.GetValue("IPC_Information", "Port", 15779));
                Serverdef.IPCIP = settings.GetValue("IPC_Information", "Ipadress", "").ToString();
                settings = null;
                #endregion
                #region Game Server
                settings = new Ini(Environment.CurrentDirectory + @"\Config\GameServer.ini");
                string[] server_list = null;
                server_list = settings.GetEntryNames("SERVERS");
                if (server_list != null && server_list.Length > 0)
                {
                    foreach (string sectname in server_list)
                    {
                        string selectedserver = settings.GetValue("SERVERS", sectname, "");
                        Serverdef.ServerDetails Serverinformation = new Serverdef.ServerDetails();
                        Serverinformation.id = Convert.ToUInt16(settings.GetValue(selectedserver, "id", 0));
                        Serverinformation.ip = settings.GetValue(selectedserver, "ip", "");
                        Serverinformation.name = settings.GetValue(selectedserver, "name", selectedserver);
                        Serverinformation.port = Convert.ToUInt16(settings.GetValue(selectedserver, "port", 15780));
                        Serverinformation.ipcport = Convert.ToUInt16(settings.GetValue(selectedserver, "ipcport", 15780));
                        Serverinformation.code = settings.GetValue(selectedserver, "code", "");
                       	if (Serverinformation.ip == "" || Serverinformation.port == 0 || Serverinformation.id == 0 || Serverinformation.ipcport == 0 || Serverdef.Serverlist.ContainsKey(Serverinformation.id))
                        {
                            Console.WriteLine("Error " + selectedserver + " in " + @"\Config\GameServer.ini" + ": field missing or id already in use!");
                            Serverinformation = null;
                        }
                        else
                        {
                            Console.WriteLine("Added {0} To the serverlist", Serverinformation.name);
                        }
                        Serverdef.Serverlist.Add(Serverinformation.id, Serverinformation);
                    }
                }
                server_list = null;
                settings = null;
                Console.WriteLine(Serverdef.TextDivider);
                #endregion
                #region NEWS
                int news_count = 0;
                string[] fileEntries = Directory.GetFiles(Environment.CurrentDirectory + @"\News", @"????-??-??.*");
                if (fileEntries.Length > 0)
                {
                    Array.Sort(fileEntries);
                    Array.Reverse(fileEntries);
                    foreach (string fName in fileEntries)
                    {
                        if (news_count < 10)
                        {
                            DateTime aDate;
                            if (DateTime.TryParse(Path.GetFileNameWithoutExtension(fName), out aDate))
                            {
                                using (StreamReader aFile = new StreamReader(fName))
                                {
                                    string line = aFile.ReadLine();
                                    if (line != null)
                                    {
                                        string line2 = aFile.ReadToEnd();
                                        if (line2 != null)
                                        {
                                            Serverdef.NewsList Item = new Serverdef.NewsList();
                                            Item.Title = line;
                                            Item.Article = line2;
                                            Item.Day = (short)aDate.Day;
                                            Item.Month = (short)aDate.Month;
                                            Item.Year = (short)aDate.Year;
                                            Serverdef.News_List.Add(Item);
                                            news_count++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please remove old news, only 10 are loaded!");
                        }
                        Console.WriteLine("Loaded {0} News Articles\n", news_count);
                        Console.WriteLine(Serverdef.TextDivider);
                    }
                }
                else
                {
                    Console.WriteLine("There is no news to be loaded\n");
                }
                #endregion
            }
        }
    }
}