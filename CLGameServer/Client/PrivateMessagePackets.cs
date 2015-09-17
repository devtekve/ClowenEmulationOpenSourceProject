using CLFramework;
using System;

namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] PrivateMessageCheck(WorldMgr.character c)
        {
            DB ms = new DB("SELECT * FROM message WHERE receiver='" + c.Information.Name + "'");
            PacketWriter Writer = new PacketWriter();
            int count = ms.Count();

            Writer.Create(OperationCode.SERVER_PM_MESSAGE);
            Writer.Byte(1);//Static
            Writer.Byte(Convert.ToByte(count));//Total count
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    string pmfrom       = reader.GetString(1);
                    string pmto         = reader.GetString(2);
                    string pmmessage    = reader.GetString(3);
                    byte pmstatus       = reader.GetByte(4);
                    DateTime pmdate     = Convert.ToDateTime(reader.GetDateTime(5));

                    Writer.Text(pmfrom);            // Message From
                    Writer.DWord(0x8A070000);       // date
                    Writer.DWord(0xC7058401);       // date
                    Writer.Byte(pmstatus);          // Status (0 = Unread) (1 = Read)
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] PrivateMessageOpen(byte type, string Messageinfo)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_PM_OPEN);
            Writer.Byte(0x01);
            Writer.Byte(type);
            Writer.Text(Messageinfo);
            return Writer.GetBytes();
        }
        public static byte[] PrivateMessageMsg(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_PM_SEND);
            switch (type)
            {
                case 1:
                    //Failed
                    Writer.Byte(0x02);
                    Writer.Byte(0x0D);
                    Writer.Byte(0x64);
                    break;
                case 2:
                    //Success
                    Writer.Byte(0x01);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] DeletePrivateMessage(byte messageid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_PM_DELETE);
            Writer.Byte(0x01);
            Writer.Byte(messageid);
            return Writer.GetBytes();
        }
    }
}
