using System;
using System.Collections.Generic;
using CLFramework;

namespace CLGameServer.Brain
{
    class ServerMgr
    {
        public static void UpdateServerInfo(byte bStatus)
        {
            foreach (KeyValuePair<int, Helpers.Settings.SrevoServerInfo> LS in Helpers.Settings.LSList)
            {
                try
                {
                    byte[] tBuf = Helpers.Manager.IPC.PacketResponseServerInfo(Servers.IPCPort, bStatus, Helpers.Settings.ServerCapacity, Helpers.Manager.GetOnlineClientCount, Helpers.Settings.ClientVersion);
                    Servers.IPCenCode(ref tBuf, LS.Value.code);
                    Helpers.Manager.IPC.Send(LS.Value.ip, LS.Value.ipcport, tBuf);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[IPC] Error informing LoginServer {0}:{1}> {2}", LS.Value.ip, LS.Value.ipcport, ex);
                }
            }
        }
        public static void UpdateServerInfo(object List = null, EventArgs a = null)
        {
            UpdateServerInfo(1);
        }
    }
}
