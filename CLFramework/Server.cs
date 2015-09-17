using System;
using System.Net;
using System.Net.Sockets;

namespace CLFramework
{
    public class Server
    {
        public bool sockdone;
        public delegate void dReceive(Decode de);
        public delegate void dConnect(ref object de, SRClient net);
        public delegate void dDisconnect(object o);

        public event dConnect OnConnect;

        Socket serverSocket;

        public void Start(string ip, int PORT)
        {
            try
            {
                IPAddress myIp = IPAddress.Any;
                if (ip != "")
                {
                    myIp = IPAddress.Parse(ip);
                }
                IPEndPoint EndPoint = new IPEndPoint(myIp, PORT);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(EndPoint);
                serverSocket.Listen(5);
                serverSocket.BeginAccept(new AsyncCallback(ClientConnect), serverSocket);
                DB.query("UPDATE users SET online='0'");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Stop()
        {
            serverSocket.Close();
        }
        private void ClientConnect(IAsyncResult ar)
        {
            Socket handlingSocket = (Socket)ar.AsyncState;
            try
            {
                Socket wSocket = handlingSocket.EndAccept(ar);

                wSocket.DontFragment = false;

                //Console.WriteLine("client connected from {0}", wSocket.RemoteEndPoint);

                object p = null;
                SRClient Player = new SRClient();

                try
                {
                    OnConnect(ref p, Player);
                }
                catch (Exception ex)
                {
                    Log.Exception("[Server.ClientConnect.OnConnect] ", ex);
                }

                Player.Packets = p;
                Player.clientSocket = wSocket;

                handlingSocket.BeginAccept(new AsyncCallback(ClientConnect), handlingSocket);

                try
                {
                    wSocket.Send(new byte[] { 0x01, 0x00, 0x00, 0x50, 0x00, 0x00, 0x01 });
                    wSocket.BeginReceive(Player.tmpbuf, 0, Player.tmpbuf.Length, SocketFlags.None, new AsyncCallback(Player.ReceiveData), wSocket);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054) // client did not wait for accept and gone
                    {
                        // ToDo: OnDisconnect
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("[Server.ClientConnect.Send+Receive]", ex);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Log.Exception(ex);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054) // Accept failed because client is no longer trying to connect, thus start new Accept
                {
                    handlingSocket.BeginAccept(new AsyncCallback(ClientConnect), handlingSocket);
                }
                else
                {
                    Log.Exception("[Server.ClientConnect.SocketException]", ex);
                }

            }
            catch (Exception ex)
            {
                Log.Exception("[Server.ClientConnect.Exception]", ex);
            }
        }
    }
}
