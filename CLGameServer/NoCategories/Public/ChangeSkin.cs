using System;
using System.Linq;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        //##########################################
        // WorldMgr.character change skin scrolls
        //##########################################
        public void HandleSkinScroll(int skinmodel, int itemid)
        {
            try
            {
                //checks
                if (itemid == 0) return;
                //Set new skin model in database
                DB.query("UPDATE character SET chartype='" + skinmodel + "' WHERE name='" + Character.Information.Name + "'");
                //Teleport user to same location (Test location normal return)
                PlayerDataLoad();
                Character.Information.Scroll = true;
                StartScrollTimer(0);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        //##########################################
        // Item gender change
        //##########################################
        public void HandleItemChange(int itemid, byte slot, byte target_slot)
        {
            try
            {
                //Get information for the current item
                ObjData.slotItem iteminfo = GetItem((uint)Character.Information.CharacterID, target_slot, 0);
                ObjData.slotItem toolinfo = GetItem((uint)Character.Information.CharacterID, slot, 0);
                //Get item name
                string itemname = ObjData.Manager.ItemBase[iteminfo.ID].Name;

                //Checks before continuing (degree item).
                if (ObjData.Manager.ItemBase[toolinfo.ID].Name.Contains("_01"))
                {
                    if (ObjData.Manager.ItemBase[iteminfo.ID].Degree > 3) return;
                }
                else if (ObjData.Manager.ItemBase[toolinfo.ID].Name.Contains("_02"))
                {
                    if (ObjData.Manager.ItemBase[iteminfo.ID].Degree > 6 && ObjData.Manager.ItemBase[iteminfo.ID].Degree < 8) return;
                }
                else if (ObjData.Manager.ItemBase[toolinfo.ID].Name.Contains("_03"))
                {
                    if (ObjData.Manager.ItemBase[iteminfo.ID].Degree > 9 && ObjData.Manager.ItemBase[iteminfo.ID].Degree < 6) return;
                }
                else if (ObjData.Manager.ItemBase[toolinfo.ID].Name.Contains("_04"))
                {
                    if (ObjData.Manager.ItemBase[iteminfo.ID].Degree > 12 && ObjData.Manager.ItemBase[iteminfo.ID].Degree < 10) return;
                }
                //Rename the item to the opposite gender for getting the new id
                if (itemname.Contains("_M_"))
                    itemname = itemname.Replace("_M_", "_W_");
                else if (itemname.Contains("_W_"))
                    itemname = itemname.Replace("_W_", "_M_");
                //Return the new itemid value
                iteminfo.ID = GetGenderItem(itemname);
                //Send 1st packet
                client.Send(Packet.ChangeItemQ(target_slot, iteminfo.ID));
                //Remove the gender change item visually (amount).
                HandleUpdateSlotChange(target_slot, iteminfo, iteminfo.ID);
                //Need to refactor the packets for item move will do that later
                client.Send(Packet.MoveItem(target_slot, slot,0,0,0,"MOVE_GENDER_CHANGE"));
                //Need to check refresh info for the item. (Rest works).
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item gender change error {0}", ex);
                Log.Exception(ex);
            }

        }
        public int GetGenderItem(string itemname)
        {
            try
            {
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Name == itemname)
                            return ObjData.Manager.ItemBase[i].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }
    }
}
