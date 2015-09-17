using CLFramework;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CLGameServer
{
    class Program
    {
        public static Server net;
        static bool cancelServer = false;
        static DateTime lastPromote = DateTime.MinValue;
        #region App Close Handling
        delegate bool ConsoleEventHandlerDelegate(ConsoleHandlerEventCode eventCode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleEventHandlerDelegate handlerProc, bool add);

        enum ConsoleHandlerEventCode : uint { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT = 1, CTRL_CLOSE_EVENT = 2, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT = 6 }

        static ConsoleEventHandlerDelegate consoleHandler;

        static Program()
        {
            consoleHandler = new ConsoleEventHandlerDelegate(ConsoleEventHandler);
            SetConsoleCtrlHandler(consoleHandler, true);
        }

        static bool ConsoleEventHandler(ConsoleHandlerEventCode eventCode)
        {
            if (eventCode == ConsoleHandlerEventCode.CTRL_CLOSE_EVENT)
            {
                Console.WriteLine("Exiting because user closed Console", eventCode);
                // CloseSystem();
                Environment.Exit(0);
            }
            return false;
        }
        #endregion

        #region Events
        public static void _OnReceiveData(Decode de)
        {
            Client.Parse.OperationCodes(de);
        }
        public static void _OnClientConnect(ref object de, SRClient net)
        {
            de = new PlayerMgr(net);
        }
        public static void _OnClientDisconnect(object o)
        {
            try
            {
                if (o != null)
                {
                    PlayerMgr s = (PlayerMgr)o;
                    s.PrintLastPack();
                    s.Disconnect("normal");
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #region IPCServer
        public static void _OnIPC(Socket aSocket, EndPoint ep, byte[] data)
        {
            try
            {
                if (data.Length >= 6)
                {
                    ushort pServer = (ushort)(data[0] + (data[1] << 8));
                    Helpers.Settings.SrevoServerInfo remoteLoginServer = Helpers.GetInformation.GetServerByEndPoint(((IPEndPoint)ep).Address.ToString(), pServer);
                    if (remoteLoginServer != null)
                    {
                        // decode data
                        Servers.IPCdeCode(ref data, remoteLoginServer.code);

                        byte pCmd = data[3];
                        int dLen = (data[4] + (data[5] << 8));
                        byte crc = Servers.BCRC(data, data.Length - 1);
                        if (data[data.Length - 1] != crc) // wrong CRC
                        {
                            Log.Exception("[IPC] Wrong Checksum from Server " + remoteLoginServer.id + ". Please Check !");
                            return;
                        }
                        if (data.Length >= (dLen + 6))
                        {
                            if (pCmd == (byte)IPCCommand.IPC_REQUEST_SERVERINFO)
                            {

                                remoteLoginServer.lastPing = DateTime.Now;
                                byte[] rspBuf = Helpers.Manager.IPC.PacketResponseServerInfo(Servers.IPCPort, 1, 100, Helpers.Manager.GetOnlineClientCount, Helpers.Settings.ClientVersion);
                                Servers.IPCenCode(ref rspBuf, remoteLoginServer.code);
                                Helpers.Manager.IPC.Send(remoteLoginServer.ip, remoteLoginServer.ipcport, rspBuf);

                            }
                            else if (pCmd == (byte)IPCCommand.IPC_REQUEST_LOGIN)
                            {
                                remoteLoginServer.lastPing = DateTime.Now;
                                if (dLen > 4)
                                {
                                    int bp = 6;
                                    byte cLen = data[bp++];
                                    byte[] tmpbuf = new byte[cLen];
                                    Buffer.BlockCopy(data, bp, tmpbuf, 0, cLen);
                                    bp += cLen;
                                    string tmpID = ASCIIEncoding.ASCII.GetString(tmpbuf);
                                    cLen = data[bp++];
                                    tmpbuf = new byte[cLen];
                                    Buffer.BlockCopy(data, bp, tmpbuf, 0, cLen);
                                    bp += cLen;
                                    ushort rCode = (ushort)(data[bp] + (data[bp + 1] << 8));
                                    string tmpPW = ASCIIEncoding.ASCII.GetString(tmpbuf);
                                    WorldMgr.player tmpPlayer = null;
                                    ushort lResult = PlayerMgr.LoginUser(tmpID, ref tmpPW, ref tmpPlayer, false);
                                    tmpPlayer = null;
                                    tmpbuf = Helpers.Manager.IPC.PacketResponseLogin(Servers.IPCPort, lResult, rCode, lResult == 4 ? tmpPW : "");
                                    Servers.IPCenCode(ref tmpbuf, remoteLoginServer.code);
                                    Helpers.Manager.IPC.Send(remoteLoginServer.ip, remoteLoginServer.ipcport, tmpbuf);
                                }
                                else
                                {
                                    Console.WriteLine("[IPC] content to short");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[IPC] unknown command recevied");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[IPC] data to short");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[IPC] can't find the LoginServer {0}:{1}", ((IPEndPoint)ep).Address.ToString(), pServer);
                    }

                }
                else
                {
                    Log.Exception("[IPC] packet to short from " + ep.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[IPC.OnIPC] ", ex);
            }
        }
        #endregion
        
        static void Main(string[] args)
        {
            Ini ini;
            string sqlConnect = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silk;Integrated Security=True;MultipleActiveResultSets=True;";
            string sIpIPC = "";
            string sIpServer = "";
            ushort iPortIPC = 15780;
            ushort iPortServer = 15780;
            ushort iPortCmd = 10101;
            if (File.Exists("./Settings/Settings.ini"))
            {
                //Load our ini file
                ini = new Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                //Read line below given value.
                sqlConnect = ini.GetValue("Database", "connectionstring", @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silk;Integrated Security=True;MultipleActiveResultSets=True;").ToString();
                //Load our rates.
                Helpers.Settings.Rate.Gold = Convert.ToByte(ini.GetValue("Rates", "Goldrate", 1));
                Helpers.Settings.Rate.Item = Convert.ToByte(ini.GetValue("Rates", "Droprate", 1));
                Helpers.Settings.Rate.Experience = Convert.ToByte(ini.GetValue("Rates", "XPrate", 1));
                Helpers.Settings.Rate.SkillPoint = Convert.ToByte(ini.GetValue("Rates", "SPrate", 1));
                Helpers.Settings.Rate.ItemSox = Convert.ToByte(ini.GetValue("Rates", "Sealrate", 1));
                Helpers.Settings.Rate.Elixir = Convert.ToByte(ini.GetValue("Rates", "Elixirsrate", 1));
                Helpers.Settings.Rate.Alchemy = Convert.ToByte(ini.GetValue("Rates", "Alchemyrate", 1));
                Helpers.Settings.Rate.ETC = Convert.ToByte(ini.GetValue("Rates", "ETCrate", 1));
                Helpers.Settings.Rate.MonsterSpawn = Convert.ToByte(ini.GetValue("Rates", "Spawnrate", 1));
                iPortIPC = Convert.ToUInt16(ini.GetValue("IPC", "port", 15780));
                sIpIPC = ini.GetValue("IPC", "ip", "");
                iPortServer = Convert.ToUInt16(ini.GetValue("Server", "port", 15780));
                sIpServer = ini.GetValue("Server", "ip", "");
                iPortCmd = Convert.ToUInt16(ini.GetValue("CMD", "port", 10101));
                Helpers.Manager.maxSlots = Convert.ToInt32(ini.GetValue("Server", "MaxSlots", 100));
            }
            else
            {
                Log.Exception("Settings Error");
            }
            DB.Connection(sqlConnect);
            // create servers
            try
            {
                net = new Server();

                net.OnConnect += new Server.dConnect(_OnClientConnect);

                Helpers.Manager.ServerStartedTime = DateTime.Now;

                SRClient.OnReceiveData += new SRClient.dReceive(_OnReceiveData);
                SRClient.OnDisconnect += new SRClient.dDisconnect(_OnClientDisconnect);
                
                #region IPC Server StartUp
                Helpers.Manager.IPC = new Servers.IPCServer();
                Helpers.Manager.IPC.OnReceive += new Servers.IPCServer.dOnReceive(_OnIPC);
                Helpers.Settings.LoadServers("LoginServers.ini", 15779);
                #endregion

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }


            #region Check Directories
            string cur_path = Environment.CurrentDirectory + @"\PlayerData\";
            Directory.CreateDirectory(cur_path + "HotKey");
            Directory.CreateDirectory(cur_path + "AutoPotion");
            Directory.CreateDirectory(cur_path + "Log");
            cur_path = null;
            #endregion
            FileDB.Load();
            //Update serverlist info
            Helpers.Manager.clients.update += new EventHandler(Brain.ServerMgr.UpdateServerInfo);
            Helpers.Manager.IPC.Start(sIpIPC, iPortIPC);
            net.Start(sIpServer, iPortServer);
            Brain.ServerMgr.UpdateServerInfo();
            // main loop
            lastPromote = DateTime.Now;
            Console.ReadLine();
            Brain.ServerMgr.UpdateServerInfo(0);
        }
    }
}
