using System;
using System.Net;
using Clowen.Definitions;
using System.Text;

namespace Clowen.Core.Networking
{
    class OnIpc
    {
        public static void OnIPC(System.Net.Sockets.Socket aSocket, EndPoint ep, byte[] data)
        {
            //Wrap our code inside a try to catch bad exception errors
            try
            {
                //Make sure the data lenght is equal to six or higher
                if (data.Length >= 6)
                {
                    //If so we continue and Set pServer information
                    ushort pServer = (ushort)(data[0] + (data[1] << 8));
                    Serverdef.ServerDetails remoteGameServer = Serverdef.GetServerByEndPoint(((IPEndPoint)ep).Address.ToString(), pServer);
                    if (remoteGameServer != null)
                    {
                        //Decode checksum data
                        CLFramework.Servers.IPCdeCode(ref data, remoteGameServer.code);
                        //Command data
                        byte pCmd = data[3];
                        //Data lenght
                        int dLen = (data[4] + (data[5] << 8));
                        //Checksum byte
                        byte crc = CLFramework.Servers.BCRC(data, data.Length - 1);
                        //If the checksum codes dont match up
                        if (data[data.Length - 1] != crc) // wrong CRC
                        {
                            //Write the error information to console
                            Console.WriteLine("Error Code for: " + remoteGameServer.name + " are not the same");
                            //Return back
                            return;
                        }
                        //If checksum is ok
                        if (data.Length >= (dLen + 6))
                        {
                            if (pCmd == (byte)IPCCommand.IPC_INFO_SERVER)
                            {
                                if (data.Length >= 11)
                                {
                                    remoteGameServer.maxSlots = (ushort)(data[7] + (data[8] << 8));
                                    remoteGameServer.usedSlots = (ushort)(data[9] + (data[10] << 8));
                                    remoteGameServer.lastPing = DateTime.Now;
                                    //Console.WriteLine("[SERVER] Server: " + remoteGameServer.name + ": players online " + remoteGameServer.usedSlots + "/" + remoteGameServer.maxSlots + "");
                                    if (remoteGameServer.status == 0 && data[6] != 0)
                                    {
                                        Console.WriteLine("[SERVER] Server: " + remoteGameServer.name + " is now online");
                                    }
                                    if (remoteGameServer.status != 0 && data[6] == 0)
                                    {
                                        Console.WriteLine("[SERVER] Server: " + remoteGameServer.name + " is now in check state");
                                    }
                                    remoteGameServer.status = data[6];
                                }
                                else
                                {
                                }
                            }
                            else if (pCmd == (byte)IPCCommand.IPC_INFO_LOGIN)
                            {
                                if (dLen >= 4)
                                {
                                    ushort IPCid = (ushort)(data[6] + (data[7] << 8));
                                    ushort IPCResult = (ushort)(data[8] + (data[9] << 8));
                                    byte sLen = data[10];
                                    lock (Serverdef.IPCResultList)
                                    {
                                        if (Serverdef.IPCResultList.ContainsKey(IPCid))
                                        {
                                            Serverdef.IPCResultList[IPCid].ResultCode = IPCResult;
                                            if (sLen > 0)
                                            {
                                                Serverdef.IPCResultList[IPCid].BanReason = ASCIIEncoding.ASCII.GetString(data, 11, sLen);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error ResultList mismatch");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error unknown command recevied");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error data to short");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error can't find the GameServer " + ((IPEndPoint)ep).Address.ToString() + "");
                    }
                }
                else
                {
                    Console.WriteLine("Error packet to short from " + ep.ToString() + "");
                }
            }
            catch (Exception ex)
            {
                CLFramework.Log.Exception(ex);
            }
        }
    }
}
