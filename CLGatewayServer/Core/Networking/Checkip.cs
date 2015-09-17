using System.Net;
using System.Net.Sockets;
using Clowen.Definitions;

namespace Clowen.Core.Networking
{
    class Checkip
    {
        public static void _Do()
        {
            //Set bool to false
            Serverdef.multihomed = false;
            //If the local ip has not been set
            if (Serverdef.LocalIP == "")
            {
                //Get our hostname
                IPAddress[] lIpList = Dns.GetHostAddresses(Dns.GetHostName());
                //Then repeat for each ip in the list
                foreach (IPAddress aIP in lIpList)
                {
                    //If the ip addressfamily equals to the internetwork
                    if (aIP.AddressFamily.Equals(AddressFamily.InterNetwork))
                    {
                        //If it does not equal a loopback adapter
                        if (!aIP.Equals(IPAddress.Loopback))
                        {
                            //If the local ip is not blank
                            if (Serverdef.LocalIP != "")
                            {
                                //Set multhomed to true
                                Serverdef.multihomed = true;
                            }
                            //If the local ip is blank
                            else
                            {
                                //Set ip to local ip
                                Serverdef.LocalIP = aIP.ToString();
                            }
                        }
                    }
                }
            }
        }
    }
}
