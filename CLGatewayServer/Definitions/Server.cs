using System;
using System.Collections.Generic;

namespace Clowen.Definitions
{
    class Serverdef
    {
        public static int SilkroadClientVersion = 0;		
		public static byte SilkroadClientLocale = 0;
        public static string TextDivider = "###############################################################################";
        public static string Loginserver_IP = "";
        public static int Loginserver_PORT = 15779;
        public static ushort IPCNewId = 0;
        public static string IPCIP = "0.0.0.0";
        public static CLFramework.Servers.IPCServer IPCServer;
        public static ushort IPCPort = 15779;
        public class IPCItem
        {
            public ushort ResultCode;
            public string BanReason;
        }
        public static string LocalIP = "";
        public static bool multihomed = false;
        public static Dictionary<ushort, IPCItem> IPCResultList = new Dictionary<ushort, IPCItem>();
        public static Dictionary<int, Serverdef.ServerDetails> Serverlist = new Dictionary<int, Serverdef.ServerDetails>();
        public class ServerDetails
        {
            public ushort id = 0;
            public ushort port = 15779;
            public ushort ipcport = 15780;
            public ushort maxSlots = 0;
            public ushort usedSlots = 0;
            public string name = "";
            public string ip = "";
            public string extip = "";
            public string code = "";
            public byte status = 0;
            public DateTime lastPing = DateTime.MinValue;
        }
        public static List<NewsList> News_List = new List<NewsList>();
        public class NewsList
        {
            public string Title, Article;
            public short Day, Month, Year;
        }
        public static ServerDetails GetServerByEndPoint(string ip, int port)
        {
            ServerDetails GS = null;
            foreach (KeyValuePair<int, ServerDetails> GSI in Serverlist)
            {
                if (GSI.Value.ip == ip && GSI.Value.ipcport == port)
                {
                    GS = GSI.Value;
                }
            }
            return GS;
        }
    }
}
