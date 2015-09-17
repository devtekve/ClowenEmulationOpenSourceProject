using CLFramework;
namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] CloseNPC()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_CLOSE_NPC);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] OpenNPC(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_OPEN_NPC);
            Writer.Bool(true);
            switch (type)
            {
                case 1:
                    //Purchase items
                    Writer.Byte(type);
                    break;
                case 2:
                    Writer.Byte(2);
                    break;
                case 12:
                    Writer.Byte(0x01);
                    Writer.Byte(0x01);
                    break;
                default:
                    Writer.Byte(type);
                    break;
            }
            return Writer.GetBytes();
        }
    }
}
