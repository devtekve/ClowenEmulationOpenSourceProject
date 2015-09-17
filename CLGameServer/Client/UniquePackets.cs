using CLFramework;
namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] Unique_Data(byte type, int mobid, string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_UNIQUE_ANNOUNCE);
            Writer.Byte(type);
            switch (type)
            {
                case 5:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    break;
                case 6:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    Writer.Text(name);
                    break;
            }


            return Writer.GetBytes();
        }
    }
}
