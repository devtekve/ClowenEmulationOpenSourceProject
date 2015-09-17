using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Movement checks before picking
        /////////////////////////////////////////////////////////////////////////////////
        void Player_PickUp()
        {
            try
            {
                if (Character.State.Sitting) return; 
                if (Character.State.Exchanging) return;
                
                if (Character.Action.Target != 0)
                {
                    WorldMgr.Items item     = Helpers.GetInformation.GetWorldItem(Character.Action.Target);

                    if (item == null) 
                        return;

                    double distance = Formule.gamedistance(Character.Position.x,Character.Position.y,item.x,item.y);

                    if (distance >= 1)
                    {
                        Character.Position.wX = item.x - Character.Position.x;
                        Character.Position.wY = item.y - Character.Position.y;

                        Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID,Formule.packetx(item.x, item.xSec),Character.Position.z,Formule.packety(item.y, item.ySec),Character.Position.xSec,Character.Position.ySec)));

                        StartMovementTimer(GetMovementTime(distance));
                        return;
                    }

                    Player_PickUpItem();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Pickup item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_PickUpItem()
        {
            try
            {
                //First check if player allready is picking up an item
                if (Character.Action.PickUping)
                {
                    //Get item information that the player has selected.
                    WorldMgr.Items item = Helpers.GetInformation.GetWorldItem(Character.Action.Target);
                    //Checks
                    if (item == null) { Character.Action.PickUping = false; return; }
                    //If the amount is lower then one
                    if (item.amount < 1) item.amount = 1;
                    //If not gold model
                    if (item.Model > 3)
                    {
                        //Get our free slots
                        byte slot = GetFreeSlot();
                        //If to low
                        if (slot <= 12)
                        {
                            Character.Action.PickUping = false;
                            client.Send(Packet.MoveItemError());
                            return;
                        }
                        //Else continue stop delete timer because its allready beeing removed.
                        item.StopDeleteTimer();
                        //Remove the world item spawn
                        Helpers.Manager.WorldItem.Remove(item);
                        //Delete the global id
                        GenerateUniqueID.Delete(item.UniqueID);
                        //Move towards the item
                        Send(Packet.StopMovement(new ObjData.vektor(Character.Information.UniqueID,
                                        (float)Formule.packetx((float)item.x, item.xSec),
                                        (float)Character.Position.z,
                                        (float)Formule.packety((float)(float)item.y, item.ySec),
                                        Character.Position.xSec,
                                        Character.Position.ySec)));
                        //Send animation packet pickup
                        Send(Packet.Pickup_Animation(Character.Information.UniqueID, 0));
                        //Check what item type we have (Etc, or armor / weapon).
                        int amount = 0;
                        //Set amount or plusvalue
                        #region Amount definition
                        if (ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.BLADE ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.CH_SHIELD ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_SHIELD ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.BOW ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.SWORD ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.EARRING ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.RING ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.NECKLACE ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.ARMOR ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.GARMENT ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.GM ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.HEAVY ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.LIGHT ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.PROTECTOR ||
                             ObjData.Manager.ItemBase[item.Model].Itemtype == ObjData.item_database.ItemType.AVATAR ||
                             ObjData.Manager.ItemBase[item.Model].Type == ObjData.item_database.ArmorType.ROBE) 
                                amount = item.PlusValue;
                        else amount = item.amount;
                        #endregion
                        //Send item creation packet
                        client.Send(Packet.CREATEITEM(0, slot, item.Model, (short)amount, (int)ObjData.Manager.ItemBase[item.Model].Defans.Durability, ObjData.Manager.ItemBase[item.Model].ID, item.UniqueID));
                        //Save to database
                        AddItem(item.Model, (short)amount, slot, Character.Information.CharacterID, item.Model);
                    }
                    //If the item is gold
                    else
                    {
                        //Remove the spawned item
                        Helpers.Manager.WorldItem.Remove(item);
                        //Remove global id
                        GenerateUniqueID.Delete(item.UniqueID);
                        //Movement packet
                        Send(Packet.StopMovement(new ObjData.vektor(Character.Information.UniqueID,
                                    (float)Formule.packetx((float)item.x, item.xSec),
                                    (float)Character.Position.z,
                                    (float)Formule.packety((float)(float)item.y, item.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));
                        //Send animation packet
                        Send(Packet.Pickup_Animation(Character.Information.UniqueID, 0));
                        //Add gold to player information
                        Character.Information.Gold += item.amount;
                        //Send visual packet for gold
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        //Send message packet gold gained
                        client.Send(Packet.GoldMessagePick(item.amount));
                        //Save player gold
                        SaveGold();
                    }
                    //Despawn item for us
                    item.DeSpawnMe();
                    //Dispose of the item
                    item.Dispose();
                    //Set picking to false
                    Character.Action.PickUping = false;
                    if (Timer.Pickup != null) Timer.Pickup.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pickup Error {0}", ex);
                Log.Exception(ex);
            }
        }
        public void Pet_PickupItem(WorldMgr.Items item)
        {
           
        }
        
    }
}
