using System;
using System.Collections.Generic;

namespace Clowen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Clowen Login Server";
            Settings.Load();
            Core.Networking.Checkip._Do();
            CLFramework.Server net = new CLFramework.Server();
            net.OnConnect += new CLFramework.Server.dConnect(Core.Networking.Clients._OnClientConnect);

            CLFramework.SRClient.OnReceiveData += new CLFramework.SRClient.dReceive(Core.Networking.Clients._OnReceiveData);
            CLFramework.SRClient.OnDisconnect += new CLFramework.SRClient.dDisconnect(Core.Networking.Clients._OnClientDisconnect);
            Clowen.Core.Networking.LoadNew._Do();
            net.Start(Definitions.Serverdef.Loginserver_IP, Definitions.Serverdef.Loginserver_PORT);
            while (true)
            {
                System.Threading.Thread.Sleep(100);
                foreach (KeyValuePair<int, Definitions.Serverdef.ServerDetails> SSI in Definitions.Serverdef.Serverlist)
                {
                    if (SSI.Value.status != 0 && SSI.Value.lastPing.AddMinutes(5) < DateTime.Now)
                    {
                        SSI.Value.status = 0;
                        Console.WriteLine("Error Server " + SSI.Value.name + " has timed out");
                    }
                }
            }
        }
    }
}
