using System;
using CLFramework;

namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] CreateFormedParty(WorldMgr.party pt)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode to packet
            Writer.Create(OperationCode.SERVER_FORMED_PARTY_CREATED);
            //Static byte
            Writer.Byte(1);
            //Party id
            Writer.DWord(pt.ptid);
            //0 Dword value
            Writer.DWord(0);
            //Party type
            Writer.Byte(pt.Type);
            //Party purpose
            Writer.Byte(pt.ptpurpose);
            //Party min level required
            Writer.Byte(pt.minlevel);
            //Party max level allowed
            Writer.Byte(pt.maxlevel);
            //Party name
            Writer.Text3(pt.partyname);
            //Return all bytes to send
            return Writer.GetBytes();
        }
    }
}
