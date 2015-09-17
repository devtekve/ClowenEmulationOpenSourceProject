using System;
using System.Collections.Generic;
using CLFramework;


namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] Exchange_ItemPacket(int id, List<ObjData.slotItem> Exhange, bool mine)
        {
            
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_ITEM);
            Writer.DWord(id);
            Writer.Byte(Exhange.Count);

            for (byte i = 0; i < Exhange.Count; i++)
            {
                PlayerMgr.LoadBluesid(Exhange[i].dbID);
                if (mine) Writer.Byte(Exhange[i].Slot);

                Writer.Byte(i);
                Writer.DWord(0);
                Writer.DWord(Exhange[i].ID);

                if (ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.ARMOR ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.GARMENT ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.GM ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.HEAVY ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.LIGHT ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.PROTECTOR ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Type == ObjData.item_database.ArmorType.ROBE ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EARRING ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.RING ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.NECKLACE ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.BOW ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || 
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                {
                    Writer.Byte(Exhange[i].PlusValue);
                        Writer.LWord(0);
                        Writer.DWord(ObjData.Manager.ItemBase[Exhange[i].ID].Defans.Durability);
                        if (ObjData.Manager.ItemBlue[Exhange[i].dbID].totalblue != 0)
                        {
                            Writer.Byte(Convert.ToByte(ObjData.Manager.ItemBlue[Exhange[i].dbID].totalblue));
                            for (int a = 1; a <= ObjData.Manager.ItemBlue[Exhange[i].dbID].totalblue; a++)
                            {
                                Writer.DWord(ObjData.Manager.MagicOptions.Find(mg => (mg.Name == Convert.ToString(ObjData.Manager.ItemBlue[Exhange[i].dbID].blue[i]))).ID);
                                Writer.DWord(ObjData.Manager.ItemBlue[Exhange[i].dbID].blueamount[i]);
                            }
                        }

                        else
                        {
                            Writer.Byte(0);
                        }
                        Writer.Word(1);
                        Writer.Word(2);
                        Writer.Word(3);
                }
                else if (ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.STONES)
                {
                    Writer.Word(Exhange[i].Amount);
                    Writer.Byte(0);
                }
                else if (ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.MONSTERMASK)
                {
                    Writer.DWord(0);
                }
                else if (ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.ELIXIR)
                {
                    Writer.Word(1);
                }
                    else if (ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.ARROW || 
                    ObjData.Manager.ItemBase[Exhange[i].ID].Itemtype == ObjData.item_database.ItemType.BOLT ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.HP_POTION ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.MP_POTION ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.VIGOR_POTION ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.SPEED_POTION ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Ticket == ObjData.item_database.Tickets.BEGINNER_HELPERS ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.ELIXIR ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].Etctype == ObjData.item_database.EtcType.ALCHEMY_MATERIAL ||
                    ObjData.Manager.ItemBase[Exhange[i].ID].TypeID2 == 3)
                {
                    Writer.Word(Exhange[i].Amount);
                }
            }
            return Writer.GetBytes();
        }
        public static byte[] Exchange_ItemSlot(byte type, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(slot);
            if (type == 4) Writer.Byte(0);

            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_ACCEPT);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_ACCEPT2);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_GOLD);
            Writer.Byte(2);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Approve()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_APPROVE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Finish()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_FINISHED);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Cancel()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_CANCEL);
            Writer.Byte(0x2C);
            Writer.Byte(0x18);
            return Writer.GetBytes();
        }
        public static byte[] ItemExchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(13);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] GuildGoldUpdate(long info, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.LWord(info);
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_WINDOW);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(byte type, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_PROCESS);
            Writer.Bool(true);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] CloseExhangeWindow()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_EXCHANGE_CLOSE);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
    }
}
