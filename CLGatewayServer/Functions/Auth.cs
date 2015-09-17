using System;
using CLFramework;

namespace Clowen.Functions
{
    class Auth
    {
        enum LOGIN_RESULT_TYPES : ushort
        {
            SERVER_CROWDED,
            ACCOUNT_NOT_FOUND,
            ACCOUNT_ALREADY_ONLINE,
            ACCOUNT_BANNED,
            ACCOUNT_CONNECT_SUCCESSFULY,
            ACCOUNT_INFORMATION_FAILED
        };
        public static void Connect(Definitions.Clientdefinition sys)
        {
            //Wrap our code in a try / catch to catch bad exception errors
            try
            {
                
                //Create a new packet reader to read packet data
                PacketReader Reader = new PacketReader(sys.PacketInformation.buffer);
                //If the first byte = 18 we continue
                if (Reader.Byte() == 18)
                {
                    //First string packet data is for the username
                    string ID = Reader.Text();
                    //Second string packet data is for the password
                    string PW = Reader.Text();
                    
                    byte ReaderBy = Reader.Byte(); //Unknown byte not needed:0xff ?? Country?
                    //Check what server id is requested
					ushort ShardID = Reader.UInt16();
                    //Set login result
                    ushort lResult = 99;
                    //Set new details for the server in the serverlist
                    Definitions.Serverdef.ServerDetails SSI = Definitions.Serverdef.Serverlist[ShardID];
                    //If the server chosen is not null
                    if (SSI != null)
                    {
                        //TODO: Continue commenting code here
                        ushort myKey = 0;
                        string sReason = "";
                        lock (Definitions.Serverdef.IPCResultList)
                        {
                            myKey = Definitions.Serverdef.IPCNewId++;
                        }
                        byte[] rqp = Definitions.Serverdef.IPCServer.PacketRequestLogin(Definitions.Serverdef.IPCPort, ID, PW, myKey);
                        Servers.IPCenCode(ref rqp, SSI.code);
                        lock (Definitions.Serverdef.IPCResultList)
                        {
                            Definitions.Serverdef.IPCResultList.Add(myKey, new Definitions.Serverdef.IPCItem());
                            Definitions.Serverdef.IPCResultList[myKey].ResultCode = 0x8000;
                        }
                        Definitions.Serverdef.IPCServer.Send(SSI.ip, SSI.ipcport, rqp);
                        DateTime tOut = DateTime.Now.AddSeconds(30);
                        while ((tOut >= DateTime.Now) && (Definitions.Serverdef.IPCResultList[myKey].ResultCode == 0x8000) && (sys.client.clientSocket.Connected))
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        lResult = Definitions.Serverdef.IPCResultList[myKey].ResultCode;
                        sReason = Definitions.Serverdef.IPCResultList[myKey].BanReason;
                        lock (Definitions.Serverdef.IPCResultList)
                        {
                            Definitions.Serverdef.IPCResultList[myKey] = null;
                            Definitions.Serverdef.IPCResultList.Remove(myKey);
                        }
                        rqp = null;
                        switch ((LOGIN_RESULT_TYPES)lResult)
                        {
                            case LOGIN_RESULT_TYPES.SERVER_CROWDED:
                                sys.client.Send(Core.Packets.Login.ServerIsFull());
                                sys.client.Disconnect(sys.PacketInformation.Client);
                                return;
                            case LOGIN_RESULT_TYPES.ACCOUNT_NOT_FOUND:
                                sys.client.Send(Core.Packets.Login.WriteMessage("Account ID Not Found.Please register new account www.clowenonline.com"));
                            break;
                            case LOGIN_RESULT_TYPES.ACCOUNT_ALREADY_ONLINE:
                                sys.client.Send(Core.Packets.Login.AllreadyConnected());
                                sys.client.Disconnect(sys.PacketInformation.Client);
                                return;
                            case LOGIN_RESULT_TYPES.ACCOUNT_BANNED:
                                sys.client.Send(Core.Packets.Login.AccountBanned(sReason/*,BannedTime*/));
                                sys.client.Disconnect(sys.PacketInformation.Client);
                                return;
                            case LOGIN_RESULT_TYPES.ACCOUNT_CONNECT_SUCCESSFULY:
                                sys.client.Send(Core.Packets.Login.ConnectSucces(SSI.extip != "" ? SSI.extip : SSI.ip, (Int16)SSI.port, 1));
                                return;
                            case LOGIN_RESULT_TYPES.ACCOUNT_INFORMATION_FAILED:
                                sys.client.Send( Core.Packets.Login.WriteMessage("You have typed wrong ID or PW.Please re-enter your ID or PW."));
                                //sys.client.Send(Core.Packets.Login.PasswordFailed(new Random().Next(1, 16), 16));
                                return;
                            default:
                                sys.client.Send(Core.Packets.Login.WriteMessage("Connection Failed."));
                                sys.client.Disconnect(sys.PacketInformation.Client);
                                if (lResult == 0x8000)
                                {
                                    //timeout
                                    sys.client.Send(Core.Packets.Login.WriteMessage("Connection Time out.Disconnecting."));
                                    sys.client.Disconnect(sys.PacketInformation.Client);
                                }
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        bool CheckCrowed(ushort serverid)
        {
            Definitions.Serverdef.ServerDetails SI = Definitions.Serverdef.Serverlist[serverid];
            if (SI != null)
            {
                if (SI.usedSlots >= SI.maxSlots)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
