using System;
using CLFramework;


namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] AlchemyResponse(bool isSuccess, ObjData.slotItem sItem, byte type, byte totalblue)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ALCHEMY);
            Writer.Byte(1);
            Writer.Byte(2);
            Writer.Bool(isSuccess);
            Writer.Byte(sItem.Slot);
            if (!isSuccess) { Writer.Byte(0); }
            Writer.DWord(0);
            Writer.DWord(sItem.ID);
            Writer.Byte(sItem.PlusValue);
            Writer.LWord(0);
            Writer.DWord(sItem.Durability);
            Writer.Byte(ObjData.Manager.ItemBlue[sItem.dbID].totalblue);
            for (int i = 0; i <= ObjData.Manager.ItemBlue[sItem.dbID].totalblue - 1; i++)
            {
                Writer.DWord(ObjData.Manager.MagicOptions.Find(mg => (mg.Name == Convert.ToString(ObjData.Manager.ItemBlue[sItem.dbID].blue[i]))).ID);
                Writer.DWord(ObjData.Manager.ItemBlue[sItem.dbID].blueamount[i]);
            }
            Writer.Word(1);
            Writer.Word(2);
            Writer.Word(3);

            return Writer.GetBytes();
        }
        public static byte[] AlchemyStoneResponse(bool isSuccess, ObjData.slotItem sItem)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ALCHEMY_STONE);
            if (isSuccess)
            {
                Writer.Byte(1);
                Writer.Byte(2);
                Writer.Byte(1);
                Writer.Byte(sItem.Slot);
                Writer.DWord(0);
                Writer.DWord(sItem.ID);
                Writer.Byte(sItem.PlusValue);
                Writer.LWord(0);
                Writer.DWord(sItem.Durability);
                Writer.Byte(ObjData.Manager.ItemBlue[sItem.dbID].totalblue);
                for (int i = 0; i <= ObjData.Manager.ItemBlue[sItem.dbID].totalblue - 1; i++)
                {
                    Writer.DWord(ObjData.Manager.MagicOptions.Find(mg => (mg.Name == Convert.ToString(ObjData.Manager.ItemBlue[sItem.dbID].blue[i]))).ID);
                    Writer.DWord(ObjData.Manager.ItemBlue[sItem.dbID].blueamount[i]);
                }
                Writer.Byte(1);
                Writer.Byte(0);
                Writer.Byte(2);
                Writer.Byte(0);
                Writer.Byte(3);
                Writer.Byte(0);
            }
            else
            {
                Writer.Byte(2);
                Writer.Byte(0x23);
                Writer.Byte(0x54);
            }
            return Writer.GetBytes();
        }
        public static byte[] AlchemyCancel()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ALCHEMY);
            Writer.Byte(1);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] StoneCreation(byte slot)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(OperationCode.SERVER_ALCHEMY_CREATE_STONE);
            writer.Byte(1);
            writer.Byte(2);
            writer.Byte(slot);
            writer.DWord(1);
            return writer.GetBytes();
        }
    }
}
