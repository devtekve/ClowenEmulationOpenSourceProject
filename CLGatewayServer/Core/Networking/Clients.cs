namespace Clowen.Core.Networking
{
    class Clients
    {
        //On client connect void
        public static void _OnClientConnect(ref object de, CLFramework.SRClient net)
        {
            de = new Definitions.Clientdefinition(net);
        }
        //When data is received
        public static void _OnReceiveData(CLFramework.Decode de)
        {
            //Goto opcode list
            Packets.Opcode_Switch.Opcode_List(de);
        }
        //When a client disconnects
        public static void _OnClientDisconnect(object o)
        {
            //Set definition info
            Definitions.Clientdefinition c = (Definitions.Clientdefinition)o;
            //Close client socket
            c.client.clientSocket.Close();
        }
    }
}
