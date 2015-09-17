using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Add items to database
        /////////////////////////////////////////////////////////////////////////////////
        public void AddItem(int itemid, short prob, byte slot, int id, int modelid)
        {
            #region Add item to db
            try
            {
                if (ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.BLADE ||
                     ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.CH_SHIELD ||
                     ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EU_SHIELD ||
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
                     ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.EARRING ||
                     ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.RING ||
                     ObjData.Manager.ItemBase[itemid].Itemtype == ObjData.item_database.ItemType.NECKLACE ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.ARMOR ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.GARMENT ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.GM ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.HEAVY ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.LIGHT ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.PROTECTOR ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.AVATAR ||
                     ObjData.Manager.ItemBase[itemid].Type == ObjData.item_database.ArmorType.ROBE)
                {
                    DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,inavatar,storagetype,charbound) VALUES ('" + itemid + "','" + prob + "','" + ObjData.Manager.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','0','0','" + ObjData.Manager.ItemBase[itemid].SoulBound + "' )");

                }
                else if (ObjData.Manager.ItemBase[itemid].Pettype == ObjData.item_database.PetType.GRABPET)
                {
                    DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + 0 + "','" + ObjData.Manager.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + ObjData.Manager.ItemBase[itemid].SoulBound + "')");

                    DB ms = new DB("SELECT TOP 1 * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' ORDER BY id DESC");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            int idinfo = reader.GetInt32(0);
                            DB.query("Insert Into pets (playerid, pet_type, pet_name, pet_state, pet_itemid, pet_active, pet_check, pet_unique) VALUES ('" + Character.Information.CharacterID + "','4','No name','1','" + itemid + "','0','item" + slot + "','" + idinfo + "')");
                        }
                    }
                    ms.Close();
                }
                else if (ObjData.Manager.ItemBase[itemid].Pettype == ObjData.item_database.PetType.ATTACKPET)
                {
                    DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + 0 + "','" + ObjData.Manager.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + ObjData.Manager.ItemBase[itemid].SoulBound + "')");

                    DB ms = new DB("SELECT TOP 1 * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' ORDER BY id DESC");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            int idinfo = reader.GetInt32(0);
                            DB.query("Insert Into pets (playerid, pet_type, pet_name, pet_state, pet_itemid, pet_active, pet_check, pet_unique) VALUES ('" + Character.Information.CharacterID + "','2','No name','1','" + itemid + "','0','item" + slot + "','" + idinfo + "')");
                        }
                    }
                    ms.Close();
                }
                else if (ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.STONES)
                {
                    DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,inavatar,storagetype,charbound) VALUES ('" + itemid + "','" + 0 + "','" + 0 + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','0','0','" + ObjData.Manager.ItemBase[itemid].SoulBound + "' )");
                }
                else if (ObjData.Manager.ItemBase[itemid].Etctype == ObjData.item_database.EtcType.MONSTERMASK)
                {
                    DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,modelid,charbound) VALUES ('" + itemid + "','" + 0 + "','" + 0 + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + modelid + "','" + ObjData.Manager.ItemBase[itemid].SoulBound + "' )");
                }
                else
                {
                    if (prob < 2) prob = 1;
                    DB.query("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + prob + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + ObjData.Manager.ItemBase[itemid].SoulBound + "' )");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Item add error: ", ex);
            }
            #endregion
        }            
    }
}