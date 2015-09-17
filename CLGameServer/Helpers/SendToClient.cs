using System;
using CLFramework;
namespace CLGameServer.Helpers
{
    class SendToClient
    {
        public static void SendAll(byte[] buff)
        {
            lock (Manager.clients)
            {
                for (int i = 0; i < Manager.clients.Count; i++)
                {
                    try
                    {
                        if (Manager.clients[i] != null && Manager.clients[i].Character.InGame)
                            Manager.clients[i].client.Send(buff);
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
        }
        
    }
}
