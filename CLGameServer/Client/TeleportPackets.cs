using CLFramework;
namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] TeleportStart2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_TELEPORTSTART);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] TeleportStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_TELEPORTSTART);
            Writer.Byte(2);
            Writer.Word(1);
            return Writer.GetBytes();
        }
        
        public static byte[] ErrorArmorType(int itemid)
        {
            //Dunno what this is for yet.
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ITEM_EQUIP_CHECK);
            Writer.DWord(itemid);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] TeleportOtherStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_TELEPORTOTHERSTART);
            Writer.DWord(0);
            return Writer.GetBytes();
        }
        public static byte[] TeleportImage(byte xsec, byte ysec)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_TELEPORTIMAGE);
            Writer.Byte(xsec);
            Writer.Byte(ysec);
            return Writer.GetBytes();
        }
        public static byte[] UpdatePlace()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_SAVE_PLACE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
