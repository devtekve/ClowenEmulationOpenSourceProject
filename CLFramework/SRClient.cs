using System;
using System.Net.Sockets;

namespace CLFramework
{
    public class SRClient
    {
        public bool State;
        public delegate void dReceive(Decode de);
        public delegate void dDisconnect(object o);

        public static event dReceive OnReceiveData;
        public static event dDisconnect OnDisconnect;
        public Socket clientSocket;

        public object Packets { get; set; }
        public int bufCount = 0; // packet buffer used
        public byte[] buffer = new byte[8192]; // packet buffer
        public byte[] tmpbuf = new byte[128]; // async read buffer
        public bList<byte[]> BuffList = new bList<byte[]>();
        public void ReceiveData(IAsyncResult ar)
        {
            Socket wSocket = (Socket)ar.AsyncState;
            try
            {
                if (wSocket.Connected)
                {
                    int recvSize = wSocket.EndReceive(ar);  // get the count of received bytes
                    bool checkData = true;
                    if (recvSize > 0)
                    {
                        if ((recvSize + bufCount) > 8192)  // that may be a try to force buffer overflow, we don't allow that ;)
                        {
                            checkData = false;
                            LocalDisconnect(wSocket);
                        }
                        else
                        {  // we have something in input buffer and it is not beyond our limits
                            Buffer.BlockCopy(tmpbuf, 0, buffer, bufCount, recvSize); // copy the new data to our buffer
                            bufCount += recvSize; // increase our buffer-counter
                        }
                    }
                    else
                    {   // 0 bytes received, this should be a disconnect
                        checkData = false;
                        LocalDisconnect(wSocket);
                    }

                    while (checkData) // repeat while we have 
                    {
                        checkData = false;
                        if (bufCount >= 6) // a minimum of 6 byte is required for us
                        {
                            Decode de = new Decode(buffer);
                            if (bufCount >= (6 + de.dataSize))  // that's a complete packet, lets call the handler
                            {
                                de = new Decode(wSocket, buffer, this, Packets);  // build up the Decode structure for next step
                                OnReceiveData(de); // call the handling routine
                                                   //Console.WriteLine("[CLIENT PACKET] {0}", BytesToString(buffer));
                                bufCount -= (6 + de.dataSize); // decrease buffer-counter
                                if (bufCount > 0) // was the buffer greater than the packet needs ? then it may be the next packet
                                {
                                    Buffer.BlockCopy(buffer, 6 + de.dataSize, buffer, 0, bufCount); // move the rest to buffer start
                                    checkData = true; // loop for next packet
                                }
                            }
                            de = null;
                        }
                    }
                    // start the next async read
                    if (wSocket != null && wSocket.Connected)
                    {
                        wSocket.BeginReceive(tmpbuf, 0, tmpbuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), wSocket);
                        State = true;
                    }
                }
                else
                {
                    LocalDisconnect(wSocket);
                }
            }
            catch (SocketException se)  // explicit handling of SocketException
            {
                if (se.ErrorCode == 10054)
                {
                    State = false;
                }
                LocalDisconnect(wSocket);
            }
            catch (Exception ex) // other exceptions
            {
                State = false;
                Log.Exception("Error in client ReceiveData: ", ex);
                LocalDisconnect(wSocket);
            }

        }

        public string BytesToString(byte[] buff)
        {
            string pack = null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buff);
            System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
            ushort datasize = br.ReadUInt16();
            ushort opcode = br.ReadUInt16();
            br.ReadUInt16();
            pack = String.Format("{0}->{1}", opcode.ToString("X2"), Decode.StringToPack(br.ReadBytes(datasize)));
            return pack;
        }

        public void Send(byte[] buff)
        {
            try
            {
                if (clientSocket.Connected && buff != null)
                {
                    while (BuffList.Count > 100) // to avoid memory leaks only store last 100 packets, think that woule be enough
                    {
                        BuffList.RemoveAt(0);
                    }
                    BuffList.Add(buff);

                    if (buff.Length > 0 && clientSocket.Connected)
                        clientSocket.Send(buff);
                    //Console.WriteLine("[SERVER PACKET] {0}", BytesToString(buff));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        void LocalDisconnect(Socket s)
        {
            if (s != null)
            {
                try
                {
                    if (OnDisconnect != null)
                    {
                        OnDisconnect(Packets);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("[Client.LocalDisconnect] ", ex);
                }
            }
        }

        public void Disconnect(Socket s)
        {
            try
            {
                if (s != null && s.Connected)
                {
                    s.Disconnect(true);
                }
                //s.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Log.Exception("[Client.Disconnect] ", ex);
            }
        }
        public void Close()
        {
            clientSocket.Close();
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
