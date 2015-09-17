using CLFramework;

namespace Clowen.Core.Packets
{
    class Connect
    {
        public static byte[] GateWayPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_INFO);
            Writer.Text("GatewayServer");
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] _1()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x20);
            return Writer.GetBytes();
        }
        public static byte[] _2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x0100);
            Writer.Word(0x0100);
            Writer.Byte(0x69);
            Writer.Byte(0x0C);
            Writer.DWord(0x00000005);
            Writer.Byte(0x02);
            return Writer.GetBytes();
        }
        public static byte[] _3()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x60);
            return Writer.GetBytes();
        }
        public static byte[] _4()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x0300);
            Writer.Word(0x0200);
            Writer.Word(0x0200);
            return Writer.GetBytes();
        }
        public static byte[] _5()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0);
            Writer.Byte(0xA1);
            return Writer.GetBytes();
        }
        public static byte[] _6()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(4);
            Writer.Byte(0xA1);

            return Writer.GetBytes();
        }
        public static byte[] ActualVersion()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Word(0x100);
            return Writer.GetBytes();
        }
        public static byte[] ClientIsToOld()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA100);
            Writer.Byte(2);
            Writer.Byte(5);
            return Writer.GetBytes();
        }
        public static byte[] ClientIsToNew()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Byte(2);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
