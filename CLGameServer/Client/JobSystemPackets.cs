using CLFramework;
using System;

namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] MakeAlias(string name, byte switchinfo)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_MAKE_ALIAS);
            Writer.Byte(1);
            switch (switchinfo)
            {
                case 0:
                    Writer.Byte(0);
                    Writer.Text(name);
                    break;
                case 1:
                    Writer.Byte(1);
                    Writer.Text(name);
                    break;

                default:
                    Console.WriteLine("Alias Case: " + switchinfo);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] MakeAliasError(string name, byte switchinfo)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_MAKE_ALIAS);
            Writer.Byte(2);
            Writer.Word(0);
            Writer.Byte(0);
            Writer.Text(name);
            return Writer.GetBytes();
        }
        public static byte[] PrevJobInfo(int character, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_PREV_JOB);
            switch (type)
            {
                case 244:
                    Writer.Byte(0x02);
                    Writer.Byte(0x29);
                    Writer.Byte(0x48);
                    break;
                default:
                    //Console.WriteLine("Job Case: " + type);
                    break;
            }

            return Writer.GetBytes();
        }
        public static byte[] JoinMerchant(int id, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_JOIN_MERC);
            switch (type)
            {
                case 3:
                    Writer.Byte(1);
                    Writer.Byte(type);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    break;
                default:
                    //Console.WriteLine("Join hunter Case: " + type);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] LeaveJob()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_LEAVE_JOB);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
