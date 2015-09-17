using System;
using System.Collections.Generic;

namespace Clowen.Core.Networking
{
    public enum IPCCommand { IPC_REQUEST_SERVERINFO, IPC_REQUEST_LOGIN, IPC_INFO_SERVER, IPC_INFO_LOGIN }

    public class LoadNew
    {
        public static void _Do()
        {
            //Wrap our code inside a try to catch any bad exceptions
            try
            {
                
                //Create a new server (ipc server)
                Definitions.Serverdef.IPCServer = new CLFramework.Servers.IPCServer();
                
                //Set on receive data to open Onipc
                Definitions.Serverdef.IPCServer.OnReceive += new CLFramework.Servers.IPCServer.dOnReceive(OnIpc.OnIPC);
                //Start the new server
                Definitions.Serverdef.IPCServer.Start(Definitions.Serverdef.IPCIP, Definitions.Serverdef.IPCPort);
                //Check for each defined server
                foreach (KeyValuePair<int, Definitions.Serverdef.ServerDetails> GS in Definitions.Serverdef.Serverlist)
                {
                    //Set request packet for server information
                    byte[] rqPacket = Definitions.Serverdef.IPCServer.PacketRequestServerInfo(Definitions.Serverdef.IPCPort);
                    //Set ipc endcode
                    CLFramework.Servers.IPCenCode(ref rqPacket, GS.Value.code);
                    //Send ip and port and request
                    Definitions.Serverdef.IPCServer.Send(GS.Value.ip, GS.Value.ipcport, rqPacket);
                    //Set rqpacket to null again
                    rqPacket = null;
                }
            }
            //When a exception happens
            catch (Exception ex)
            {
                CLFramework.Log.Exception(ex);
            }
        }
    }
}
