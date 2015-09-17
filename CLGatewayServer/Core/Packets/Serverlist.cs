using CLFramework;
using System.Collections.Generic;

namespace Clowen.Core.Packets
{
    class Serverlist
    {
        public static byte[] ServerListPacket(int cliVersion)
        {
            //Create a new packet writer to create our packet
            PacketWriter W = new PacketWriter();
            //Write the opcode from server to client
            W.Create(Opcodes.SERVER.SERVER_SERVERLIST);
            //Structure of packet below
            W.Word(0x0201);
            W.Text("CLOWEN_DEV_PROJECT");
            W.Byte(0);
            //Repeat the following packet data below for each server in the list
            foreach (KeyValuePair<int, Definitions.Serverdef.ServerDetails> Gameservers in Definitions.Serverdef.Serverlist)
            {
				if (cliVersion == Definitions.Serverdef.SilkroadClientVersion)
                {
                    W.Bool(true);
                    W.Word(Gameservers.Value.id);
                    W.Text(Gameservers.Value.name);
                    W.Word(Gameservers.Value.usedSlots);
                    W.Word(Gameservers.Value.maxSlots);
                    W.Byte(Gameservers.Value.status);
                }
            }
            //Static 0 byte
            W.Byte(0);
            //Send the created packet back to the request (client).
            return W.GetBytes();
        }
    }
}
