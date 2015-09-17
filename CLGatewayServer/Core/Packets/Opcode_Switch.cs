using System;
using CLFramework;

namespace Clowen.Core.Packets
{
    class Opcode_Switch
    {
        //Public void select opcode
        public static void Opcode_List(Decode information)
        
        {
            //Set definition for detailed information
            Definitions.Clientdefinition Control = (Definitions.Clientdefinition)information.Packet;
            //Set Packetinformation as information (short).
            Control.PacketInformation = information;
            //Create a new packet reader to view incoming packet data
            PacketReader R = new PacketReader(Control.PacketInformation.buffer);
            //Create switch code based on the opcode the client sends.
            switch (information.opcode)
            {
                case Opcodes.CLIENT.CLIENT_PING_CHECK:
                    break;
                case Opcodes.CLIENT.CLIENT_INFO:
                    //If the client connecting is silkroad
                    if (R.Text() == "SR_Client")
                        Control.client.Send(Connect.GateWayPacket());
                    break;
				case Opcodes.CLIENT.CLIENT_GATEWAY_PATCH_REQUEST:
                    Functions.Connect.ClientCheck(Control);
                    break;
				case Opcodes.CLIENT.CLIENT_GATEWAY_SERVERLIST_REQUEST:
                    Control.client.Send(Serverlist.ServerListPacket(0));
                    break;
				case Opcodes.CLIENT.CLIENT_GATEWAY_LOGIN_REQUEST:
                    Functions.Auth.Connect(Control);
                    break;
				case Opcodes.CLIENT.CLIENT_GATEWAY_NOTICE_REQUEST:
                    Control.client.Send(Connect._6());
                    Control.client.Send(News.NewsPacket());
                    break;
                case 1905:
                    byte[] buffer = information.buffer;
                    Definitions.Clientdefinition.Connected_Users = BitConverter.ToInt16(buffer, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
