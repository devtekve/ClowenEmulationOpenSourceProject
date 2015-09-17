using System;
using CLFramework;

namespace Clowen.Functions
{
    class Connect
    {
        public static void ClientCheck2(Definitions.Clientdefinition sys)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create       (0x2005);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0x47);
            Writer.Byte(1);
            Writer.Byte(5);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(0);
            Writer.Byte(2);
            sys.client.Send(Writer.GetBytes());
            Writer = new PacketWriter();
            Writer.Create(0x6005);
            Writer.Byte(3);
            Writer.Byte(0);
            Writer.Byte(2);
            Writer.Byte(0);
            Writer.Byte(2);
            sys.client.Send(Writer.GetBytes());
            Writer = new PacketWriter();
            Writer.Create(0xA100);
            Writer.Byte(1);
            Writer.Byte(0);
            sys.client.Send(Writer.GetBytes());
        }
        public static void ClientCheck(Definitions.Clientdefinition sys)
        {
            PacketReader Reader = new PacketReader(sys.PacketInformation.buffer);
			byte Locale = Reader.Byte();
			string Name = Reader.Text();
			int Version = Reader.Int32();
			Reader.Close();
            try
            {
                if (Name == "SR_Client")
                {
                    sys.client.Send(Core.Packets.Connect._1());
                    sys.client.Send(Core.Packets.Connect._2());
                    sys.client.Send(Core.Packets.Connect._3());
                    sys.client.Send(Core.Packets.Connect._4());
                    sys.client.Send(Core.Packets.Connect._5());
                    if (Version == Definitions.Serverdef.SilkroadClientVersion)// 
                    {
                        sys.client.Send(Core.Packets.Connect.ActualVersion());
                    }
                    else if (Version < Definitions.Serverdef.SilkroadClientVersion - 1)// 
					{
                        sys.client.Send(Core.Packets.Connect.ClientIsToOld());
					}
                    else if (Version > Definitions.Serverdef.SilkroadClientVersion)// 
                    {
                        sys.client.Send(Core.Packets.Connect.ClientIsToNew());
                    }
                    else
                    {
                        //sys.client.Send(Patch.SendPatchFiles());
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Connect.cs Error: {0}", error);
            }
        }
        
    }
}
