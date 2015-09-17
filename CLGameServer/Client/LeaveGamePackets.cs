using CLFramework;
namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] StartingLeaveGame(byte time, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_LEAVE_ACCEPT);
            Writer.Byte(1);
            Writer.DWord(time);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] EndLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_LEAVE_SUCCESS);
            return Writer.GetBytes();
        }
        public static byte[] CancelLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_LEAVE_CALCEL);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
    }
}
