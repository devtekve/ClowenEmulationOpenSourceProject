using CLFramework;
using System;
using System.Collections.Generic;

namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] StallOpen(string stallname, int CharacterID, int StallModel)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_OPEN);
            Writer.DWord(CharacterID);
            Writer.Text3(stallname);
            Writer.DWord(StallModel);        //Decoration

            return Writer.GetBytes();
        }
        public static byte[] StallOpened()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_OPENED);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] StallOpenGlobal(string stallname, int CharacterID, int StallModel)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_OPEN);
            Writer.DWord(CharacterID);
            Writer.Text3(stallname);
            Writer.DWord(StallModel);            //Decoration
            return Writer.GetBytes();
        }
        public static byte[] StallModifyItem(byte stallSlot, ulong price)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_ACTION);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(stallSlot);
            Writer.Word(1);
            Writer.LWord(price);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] StallItemMain(List<WorldMgr.stall.stallItem> ItemList)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_ACTION);
            Writer.Byte(1);
            Writer.Byte(2);
            Writer.Word(0);
            StallItemPacket(ItemList, Writer);
            return Writer.GetBytes();
        }
        public static void StallItemPacket(List<WorldMgr.stall.stallItem> ItemList, PacketWriter Writer)
        {

            for (byte i = 0; i < ItemList.Count; i++)
            {
                //Define item id
                int itemid = ItemList[i].Item.ID;
                //Temp disable mall type, todo : Enable PRE-Mall to sell in stalls.
                if (ObjData.Manager.ItemBase[itemid].Name.Contains("MALL")) return;

                PlayerMgr.LoadBluesid(ItemList[i].Item.dbID);

                Writer.Byte(ItemList[i].stallSlot);
                Writer.DWord(0);
                Writer.DWord(itemid);

                //Define what types of item we are adding to the stall
                if (ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.ARMOR ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.GARMENT ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.GM ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.HEAVY ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.LIGHT ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.PROTECTOR ||
                    ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.ROBE ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EARRING ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.RING ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.NECKLACE ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.BLADE ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.BOW ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.SWORD ||
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || 
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                {
                    Writer.Byte(ItemList[i].Item.PlusValue);
                        Writer.LWord(0);
                        Writer.DWord(ObjData.Manager.ItemBase[ItemList[i].Item.ID].Defans.Durability);
                        if (ObjData.Manager.ItemBlue[ItemList[i].Item.dbID].totalblue != 0)
                        {
                            Writer.Byte(Convert.ToByte(ObjData.Manager.ItemBlue[ItemList[i].Item.dbID].totalblue));
                            for (int a = 1; a <= ObjData.Manager.ItemBlue[ItemList[i].Item.dbID].totalblue; a++)
                            {
                                Writer.DWord(ObjData.Manager.MagicOptions.Find(mg => (mg.Name == Convert.ToString(ObjData.Manager.ItemBlue[ItemList[i].Item.dbID].blue[i]))).ID);
                                Writer.DWord(ObjData.Manager.ItemBlue[ItemList[i].Item.dbID].blueamount[i]);
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
                else if (ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.STONES)
                {
                    Writer.Word(ItemList[i].Item.Amount);
                    Writer.Byte(0);
                }
                else if (ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.MONSTERMASK)
                {
                    Writer.DWord(0);//Todo : Load monster mask monster model id here
                    return;
                }
                else if (ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.ELIXIR)
                {
                    Writer.Word(1);
                }
                    else if (ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.ARROW || 
                    ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.BOLT ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.HP_POTION ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.MP_POTION ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.VIGOR_POTION ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.SPEED_POTION ||
                    ObjData.Manager.ItemBase[itemid].Ticket == ObjData.item_database.Tickets.BEGINNER_HELPERS ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.ELIXIR ||
                    ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.ALCHEMY_MATERIAL ||
                    ObjData.Manager.ItemBase[itemid].TypeID2 == 3)
                {
                    Writer.Word(ItemList[i].Item.Amount);
                }                
                Writer.Byte(ItemList[i].Item.Slot);
                Writer.Word(ItemList[i].Item.Amount);
                Writer.LWord(ItemList[i].price);
            }
            Writer.Byte(0xFF);
        }
        public static byte[] StallBuyItem(byte stallslot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_BUY);
            Writer.Byte(amount);
            Writer.Byte(stallslot);
            return Writer.GetBytes();
        }
        public static byte[] StallBuyItem2(string charname, byte stallslot, List<WorldMgr.stall.stallItem> ItemList)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_PLAYERUPDATE);
            Writer.Byte(3);
            Writer.Byte(stallslot);
            Writer.Word(charname.Length);
            Writer.String(charname);
            StallItemPacket(ItemList, Writer);

            return Writer.GetBytes();
        }
        public static byte[] EnterStall(int CharacterID, WorldMgr.stall stall)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_OTHER_OPEN);
            Writer.Byte(1);
            Writer.DWord(stall.ownerID);
            Writer.Text3(stall.WelcomeMsg);
            Writer.Bool(stall.isOpened);
            Writer.Byte(0);
            StallItemPacket(stall.ItemList, Writer);
            Writer.Byte(stall.Members.Count - 2);
            for (byte i = 0; i < stall.Members.Count; i++)
            {
                if (stall.Members[i] != stall.ownerID && stall.Members[i] != CharacterID)
                    Writer.DWord(stall.Members[i]);
            }

            return Writer.GetBytes();
        }
        public static byte[] StallPlayerUpdate(int Characterid, int type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_PLAYERUPDATE);
            Writer.Byte(type);
            Writer.DWord(Characterid);
            return Writer.GetBytes();
        }
        public static byte[] LeaveStall()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_OTHER_CLOSE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] StallSetState(byte state)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_ACTION);
            Writer.Byte(1);
            Writer.Byte(5);
            Writer.Byte(state);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] StallWelcome(string welcome)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_ACTION);
            Writer.Byte(1);
            Writer.Byte(6);
            Writer.Text3(welcome);
            return Writer.GetBytes();
        }
        public static byte[] StallNameGlobal(int characterid, string stallname)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_RENAME);
            Writer.DWord(characterid);
            Writer.Text3(stallname);
            return Writer.GetBytes();
        }
        public static byte[] StallName(string stallname)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_ACTION);
            Writer.Byte(1);
            Writer.Byte(7);
            return Writer.GetBytes();
        }
        public static byte[] StallClose()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_CLOSED);
            Writer.Byte(1);

            return Writer.GetBytes();
        }
        public static byte[] StallCloseGlobal(int charid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_STALL_CLOSE);
            Writer.DWord(charid);
            Writer.Word(0);
            return Writer.GetBytes();
        }
    }
}
