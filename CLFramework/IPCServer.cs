using System;
using System.Net;
using System.Net.Sockets;

namespace CLFramework
{
    public class Servers
    {
        public static ushort IPCPort = 0;

        public static byte BCRC(byte[] aBytes)
        {
            return BCRC(aBytes, aBytes.Length);
        }

        public static byte BCRC(byte[] aBytes, int aLen)
        {
            byte crc = 0;
            for (int i = 0; i < aLen; i++)
            {
                crc ^= aBytes[i];
            }
            return crc;
        }


        public static void IPCdeCode(ref byte[] data, string code, int aLen)
        {
            if (code != "")
            {
                IPCenCode(ref data, code, aLen);
            }
        }

        public static void IPCdeCode(ref byte[] data, string code)
        {
            IPCdeCode(ref data, code, data.Length);
        }

        public static void IPCenCode(ref byte[] data, string code, int aLen)
        {
            if (code != "")
            {
                int keyindex = 0;
                byte vKey = (byte)(data[0] ^ data[1] ^ data[2]);
                string ret = string.Empty;
                byte[] key = (new System.Text.ASCIIEncoding()).GetBytes(code);
                for (int i = 3; i < aLen; i++)
                {
                    data[i] = (byte)(data[i] ^ key[keyindex++] ^ 0x96 ^ i ^ vKey);
                    if (keyindex >= key.Length)
                    {
                        keyindex = 0;
                    }
                }
            }
        }

        public static void IPCenCode(ref byte[] data, string code)
        {
            IPCenCode(ref data, code, data.Length);
        }


        public class IPCServer
        {
            public Socket theServer;
            Socket sendSocket = null;
            public byte[] buf = new byte[8192];

            public delegate void dOnReceive(Socket aServerSocket, EndPoint remoteEndPoint, byte[] data);

            public event dOnReceive OnReceive;

            public IPCServer()
            {
            }

            public void Start(string listenIP, ushort PORT)
            {
                IPCPort = PORT;
                IPAddress ip = IPAddress.Any;
                if (listenIP != "")
                {
                    ip = IPAddress.Parse(listenIP);
                }
                IPEndPoint ep = new IPEndPoint(ip, PORT);

                // create remote end point for reference
                IPEndPoint rep = new IPEndPoint(IPAddress.Any, 0);
                EndPoint xep = rep;

                // create udp socket and bind it
                try
                {
                    theServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    theServer.Bind(ep);

                    try
                    {
                        theServer.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref xep, new AsyncCallback(UdpReceiveCallback), theServer);
                    }
                    catch (Exception)
                    {
                        theServer.Close();
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10048)
                    {

                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                catch (Exception)
                {
                    sendSocket = null;
                }
            }

            public void UdpReceiveCallback(IAsyncResult ar)
            {
                Socket u = (Socket)ar.AsyncState;
                IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
                EndPoint tmpEP = e;

                try
                {

                    int nBytes = u.EndReceiveFrom(ar, ref tmpEP);
                    if (nBytes > 0)
                    {
                        try
                        {
                            if (OnReceive != null)
                            {
                                byte[] newbuf = new byte[nBytes];
                                Buffer.BlockCopy(buf, 0, newbuf, 0, nBytes);
                                try
                                {
                                    OnReceive(u, tmpEP, newbuf);
                                }
                                catch (Exception)
                                {
                                }
                                newbuf = null;
                            }
                            else
                            {
                            }
                        }
                        catch (Exception)
                        {
                        }
                        u.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref tmpEP, new AsyncCallback(UdpReceiveCallback), u);
                    }
                    else
                    {
                        theServer.Close();
                    }
                }
                catch (SocketException sex)
                {
                    if (sex.ErrorCode == 10054) // exception thrown when udp send is not possible (ICMP response: port unreachable)
                    {
                        u.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref tmpEP, new AsyncCallback(UdpReceiveCallback), u);
                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {

                    theServer.Close();
                }
            }

            public void Send(string destIP, int destPort, byte[] data)
            {
                try
                {
                    IPAddress rip = IPAddress.Parse(destIP);
                    IPEndPoint ep = new IPEndPoint(rip, destPort);

                    //Console.WriteLine("[IPC] sending data to {0}", ep.ToString());
                    sendSocket.SendTo(data, ep);
                    ep = null;
                }
                catch (Exception)
                {
                }
            }

            public byte[] PacketRequestServerInfo(ushort iPort)
            {
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte((byte)Rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_REQUEST_SERVERINFO);
                    IPP.WriteWord(0);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketResponseServerInfo(ushort iPort, byte bStatus, ushort iMaxSlots, int iUsedSlots, ushort iVersion)
            {
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {

                    IPP.WriteWord(iPort);
                    IPP.WriteByte((byte)Rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_INFO_SERVER);
                    IPP.WriteWord(5);
                    IPP.WriteByte(bStatus);
                    IPP.WriteWord(iMaxSlots);
                    IPP.WriteWord((ushort)iUsedSlots);
                    IPP.WriteWord(iVersion);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketRequestLogin(ushort iPort, string sUserID, string sPassword, UInt16 IPCid)
            {
                int dLen = sUserID.Length + sPassword.Length + 4;
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte((byte)Rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_REQUEST_LOGIN);
                    IPP.WriteWord((ushort)dLen);
                    IPP.WriteString(sUserID);
                    IPP.WriteString(sPassword);
                    IPP.WriteWord(IPCid);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketResponseLogin(ushort iPort, ushort wResult, ushort wID)
            {
                return PacketResponseLogin(iPort, wResult, wID, "");
            }

            public byte[] PacketResponseLogin(ushort iPort, ushort wResult, ushort wID, string sBanReason)
            {
                int dLen = sBanReason.Length + 5;
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte((byte)Rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_INFO_LOGIN);
                    IPP.WriteWord((byte)dLen);
                    IPP.WriteWord(wID);
                    IPP.WriteWord(wResult);
                    IPP.WriteString(sBanReason);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }
        }
    }
}
