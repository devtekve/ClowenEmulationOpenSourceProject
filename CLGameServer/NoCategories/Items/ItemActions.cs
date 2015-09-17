using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Move Items
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMove(byte fromSlot, byte toSlot, short quantity)
        {
            #region Move items
            try
            {
                //Get from both slots the item information details
                #region Get slot information
                ObjData.slotItem fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 0);
                ObjData.slotItem toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 0);
                #endregion
                //Define (rename for easy usage).
                #region Redefine slotnames
                fromItem.Slot = fromSlot;
                toItem.Slot = toSlot;
                #endregion
                //Checks
                #region Check slots available
                //If we unequip a item, and place it on a taken slot
                //With this part if unequipping a item, we just use toslot , because it will be changed here.
                if (fromSlot < 13 && toItem.ID != 0)
                {
                    //We get our next free available slot.
                    toSlot = GetFreeSlot();
                    //Though if our free slots is to low, we return.
                    if (toSlot <= 12) return;
                }
                #endregion
                //Job slot handling
                #region Job slot
                //When equipping a job suit, we cannot equip another (Switch them out).
                if (toSlot == 8 && toItem.ID == 0)//So our to slot must be empty
                {
                    //If item is not a  hunter suit but character job is hunter , stop
                    if (Character.Job.type == 1 && ObjData.Manager.ItemBase[fromItem.ID].Type != ObjData.item_database.ArmorType.HUNTER) return;
                    //If item is not a thief suit but character job is thief , stop
                    if (Character.Job.type == 2 && ObjData.Manager.ItemBase[fromItem.ID].Type != ObjData.item_database.ArmorType.THIEF) return;
                    //TODO: Find out more about trader job specifications
                    //If no job
                    if (Character.Job.type == 0) return;
                }
                //If we unequip from our job slot
                if (fromSlot == 8 && fromItem.ID != 0)
                {
                    ItemUnEquiped(fromItem);
                }
                #endregion
                //If we equip to slot 7 (Shield / Arrow slot) Completed.
                #region Shield / Arrow slot
                if (toSlot == 7)
                {
                    //We get more information about the items
                    ObjData.slotItem shieldItem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    ObjData.slotItem weaponItem = GetItem((uint)Character.Information.CharacterID, 6, 0);

                    //First we check if our level is high enough.
                    if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE,Messages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                        return;
                    }
                    //Then we check the race the item belongs to EU / CH.
                    else if (!CheckRace(Character.Information.Model, fromItem.ID))
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                        return;
                    }
                    //If we allready have a weapon equipped
                    if (weaponItem.ID != 0)
                    {
                        //If we allready have a shield equipped
                        if (shieldItem.ID != 0)
                        {
                            //We compare what item we are going to equip first. (If this is a shield).
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                            {
                                //Now we check if the weapon we are holding is not two handed, so we dont need to unequip the weapon item first.
                                if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                    ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                    ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                    ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF)
                                {
                                    #region Done
                                    //Equip new shield
                                    ItemEquiped(fromItem, 7);
                                    //Visually update
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Now we update the database information of the item to the new slot.                                   
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If the user is holding a two handed weapon.
                                else
                                {
                                    #region Done
                                    byte new_slot = GetFreeSlot();
                                    //We start to unequip the two handed weapon before we equip our weapon.   
                                    ItemUnEquiped(weaponItem);
                                    //Unequip our arrows
                                    ItemUnEquiped(shieldItem);
                                    //Equip new shield
                                    ItemEquiped(fromItem, 7);
                                    //Global info
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = 0;
                                    Character.Information.Item.sID = 0;
                                    //Visually update
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Visually update
                                    GetUpdateSlot(weaponItem, new_slot, 0, 1);
                                    //Database update
                                    DB.query("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Database update
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //Database update
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    #endregion
                                }
                            }
                            //If the user wants to equip arrows
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOLT || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.ARROW)
                            {
                                //We firstly check if the user has a bow equiped
                                if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //If the user has a bow equip we begin equipping our arrow / bolt item
                                    GetUpdateSlot(shieldItem, fromItem.Slot, 0, shieldItem.Amount);
                                    //We update the database with the old equipped slot item to new freeslot information
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    //We set the item amount to global information
                                    Character.Information.Item.sAmount = fromItem.Amount;
                                    Character.Information.Item.sID = fromItem.ID;
                                    //And finally set new database information for the new equipped item
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If the user wants to equip arrows, and is not holding a bow
                                else
                                {
                                    //Return with no action taken
                                    return;
                                }
                            }
                        }
                        //If the player does not have a shield equipped yet. but has a weapon equiped
                        else
                        {
                            //We check if the player is equipping a new shield.
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                            {
                                //Then we check if the player weapon type can have a shield or not.
                                if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD)
                                {
                                    #region Done
                                    //We begin equipping the item
                                    ItemEquiped(fromItem, 7);
                                    //Now we update the remaining information
                                    client.Send(Packet.MoveItem(0, fromSlot, toSlot, quantity, 0, "MOVE_INSIDE_INVENTORY"));
                                    //Then we update the item information in the database with the new slot.
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                else
                                {
                                    #region Done
                                    //Get free slot for unequiping the weapon
                                    byte new_slot = GetFreeSlot();
                                    //If not enough slots
                                    if (new_slot <= 12) return;
                                    //We start to unequip the two handed weapon before we equip our shield.
                                    ItemUnEquiped(weaponItem);
                                    //Now we update the old slot (or freeslot), to new info.
                                    ItemEquiped(fromItem, 7);
                                    //Update visually the shielditem info
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Update weapon
                                    GetUpdateSlot(weaponItem, new_slot, 0, 1);
                                    //Now we update the database information of the item to the new slot.                                   
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //And then we update the old item to the new freeslot information
                                    DB.query("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + weaponItem.dbID + "'");
                                    #endregion
                                }
                            }
                            //Again if the player wants to equip an arrow type, while holding a weapon
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOLT || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.ARROW)
                            {
                                //We check if the player has a bow or not.
                                if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done

                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = fromItem.Amount;
                                    Character.Information.Item.sID = fromItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(fromItem, 7, 0, fromItem.Amount);
                                    //Update the slot information in database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                    //If the user is not holding a weapon
                    {
                        //We check if the user is holding a shield.
                        if (shieldItem.ID != 0)
                        {
                            //If the user has a shield equipped.
                            if (ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                            {
                                //We check if the user wants to equip another shield type.
                                if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                                {
                                    #region Done
                                    //If the weapon is one handed and not a bow related item we start unequipping our shield.
                                    ItemUnEquiped(shieldItem);
                                    //Now we update the old slot (or freeslot), to new info.
                                    ItemEquiped(fromItem, 7);
                                    //Now we update the remaining information
                                    client.Send(Packet.MoveItem(0, fromSlot, 7, quantity, 0, "MOVE_INSIDE_INVENTORY"));
                                    //Now we update the database information of the item to the new slot.                                   
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //And then we update the old item to the new freeslot information
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    #endregion
                                }
                                else return;
                            }
                            else return;
                        }
                        //If there's no shield or weapon equiped.
                        else
                        {
                            //We check if the user wants to equip a shield
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                            {
                                #region Done
                                //Now we update the old slot (or freeslot), to new info.
                                ItemEquiped(fromItem, 7);
                                //Now we update the remaining information
                                client.Send(Packet.MoveItem(0, fromSlot, 7, 1, 0, "MOVE_INSIDE_INVENTORY"));
                                //Now we update the database information of the item to the new slot.                                   
                                DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            else return;
                        }
                    }
                }
                #endregion
                //Weapon slot Completed.
                #region Weapon slot
                if (toSlot == 6)
                {
                    //Get global item information
                    ObjData.slotItem shieldItem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    ObjData.slotItem weaponItem = GetItem((uint)Character.Information.CharacterID, 6, 0);

                    //First we check if our level is high enough.
                    if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                        return;
                    }
                    //Then we check the race the item belongs to EU / CH.
                    else if (!CheckRace(Character.Information.Model, fromItem.ID))
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                        return;
                    }

                    //If the player has a weapon equipped
                    if (weaponItem.ID != 0)
                    {
                        //If the player has a 1 handed weapon equipped
                        if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD || 
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE || 
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF || 
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF || 
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD)
                        {
                            //If the player is holding a shield item
                            if (shieldItem.ID != 0)
                            {
                                //If the shield item is a shield
                                if (ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                                {
                                    //if the player wants to equip a bow
                                    if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                    {
                                        #region Done
                                        byte new_slot = GetFreeSlot();
                                        //If freeslots to low
                                        if (new_slot <= 12) return;
                                        //Unequip the current weapon
                                        ItemUnEquiped(weaponItem);
                                        //Equip new bow
                                        ItemEquiped(fromItem, 6);
                                        //Unequip shield
                                        ItemUnEquiped(shieldItem);
                                        //Visually update
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Check if player has arrows equiped
                                        byte ammoslot = GetAmmoSlot(Character);
                                        //We check if the slot is not empty allready has arrows equipped.
                                        if (ammoslot != 0)
                                        {
                                            //Get the arrow information
                                            ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                            //Set the amount of arrows globally
                                            Character.Information.Item.sAmount = AmmoItem.Amount;
                                            Character.Information.Item.sID = AmmoItem.ID;
                                            //Now we update the remaining information
                                            GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                            //Update the slot information in database
                                            DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                            //Update database
                                            DB.query("UPDATE char_items SET itemnumber='item" + ammoslot + "',slot='" + ammoslot + "' WHERE id='" + shieldItem.dbID + "'");
                                        }
                                        else
                                        {
                                            //Visual update
                                            GetUpdateSlot(shieldItem, new_slot, 0, 1);
                                            //Update database
                                            DB.query("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + shieldItem.dbID + "'");
                                        }

                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        #endregion
                                    }
                                    //If the player wants to equip an other type two handed
                                    #region Done
                                    else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                                    {
                                        byte freeslot = GetFreeSlot();
                                        //Unequip the current shield
                                        ItemUnEquiped(shieldItem);
                                        //Unequip the current weapon
                                        ItemUnEquiped(weaponItem);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Visual update weapon slot
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Visual update shield slot
                                        GetUpdateSlot(shieldItem, freeslot, 0, 1);
                                        DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + freeslot + "',slot='" + freeslot + "' WHERE id='" + shieldItem.dbID + "'");
                                    }
                                    #endregion
                                    //If player wants to equip 1 handed item, we can keep the shield equiped
                                    else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF)
                                    {
                                        #region Done
                                        //Unequip Weapon
                                        ItemUnEquiped(weaponItem);
                                        //Update visually
                                        GetUpdateSlot(weaponItem, fromItem.Slot, 0, 1);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Update Database
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        #endregion
                                    }
                                }
                                else
                                    return;
                            }
                            //If no shield is equiped
                            else
                            {
                                //if player wants to equip bow
                                if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //Unequip the current weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new bow
                                    ItemEquiped(fromItem, 6);
                                    //Visually update
                                    GetUpdateSlot(weaponItem, fromSlot, 0, 1);
                                    //Check if player has arrows equiped
                                    byte ammoslot = GetAmmoSlot(Character);
                                    //We check if the slot is not empty allready has arrows equipped.
                                    if (ammoslot != 0)
                                    {
                                        //Get the arrow information
                                        ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                        //Set the amount of arrows globally
                                        Character.Information.Item.sAmount = AmmoItem.Amount;
                                        Character.Information.Item.sID = AmmoItem.ID;
                                        //Now we update the remaining information
                                        GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                        //Update the slot information in database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    }
                                    //Update database
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If player wants to equip other weapon type
                                #region Done
                                if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                                {
                                    //Unequip our weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new weapon
                                    ItemEquiped(fromItem, 6);
                                    //Visually update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Update database
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                }
                                #endregion
                            }
                        }
                        //If player has a 2 handed weapon equiped
                        else if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                            ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE)
                        {

                            //If player wants to equip a bow
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                            {
                                #region Done
                                //Unequip the current weapon
                                ItemUnEquiped(weaponItem);
                                //Equip new bow
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(weaponItem, fromSlot, 0, 1);
                                //Check if player has arrows equiped
                                byte ammoslot = GetAmmoSlot(Character);
                                //We check if the slot is not empty allready has arrows equipped.
                                if (ammoslot != 0)
                                {
                                    //Get the arrow information
                                    ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update the slot information in database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                }
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            //If player wants to equip other weapon types
                            #region Done
                            else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                            {
                                //Unequip our weapon
                                ItemUnEquiped(weaponItem);
                                //Equip new weapon
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                        //If player has a bow equiped
                        else if (ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[weaponItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                        {
                            //If arrows are equiped
                            if (shieldItem.ID != 0)
                            {
                                //Check to make sure its a arrow
                                if (ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.ARROW || ObjData.Manager.ItemBase[shieldItem.ID].Itemtype == ObjData.item_database.ItemType.BOLT)
                                {
                                    //Check what we are equipping
                                    #region Done
                                    if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                        ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                                    {
                                        //Get free slot to move to
                                        byte freeslot = GetFreeSlot();
                                        //If slots free is to low
                                        if (freeslot <= 12) return;
                                        //Unequip our bow
                                        ItemUnEquiped(weaponItem);
                                        //Unequip arrow
                                        ItemUnEquiped(shieldItem);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Visual update
                                        GetUpdateSlot(weaponItem, fromItem.Slot, 0, 1);
                                        //Visual update
                                        GetUpdateSlot(shieldItem, freeslot, 0, 1);
                                        //Set global info
                                        Character.Information.Item.sAmount = 0;
                                        Character.Information.Item.sID = 0;
                                        //Update Database
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update Database
                                        DB.query("UPDATE char_items SET itemnumber='item" + freeslot + "',slot='" + freeslot + "' WHERE id='" + shieldItem.dbID + "'");
                                        //Update Database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    }
                                    #endregion
                                    //If player wants to equip another bow
                                    #region Done
                                    else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                    {
                                        //Unequip current bow
                                        ItemUnEquiped(weaponItem);
                                        //Equip new bow
                                        ItemEquiped(fromItem, 6);
                                        //Visually update
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    }
                                    #endregion
                                }
                            }
                            //If no arrow item is equiped
                            else
                            {
                                //If player wants to equip another bow
                                if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //Unequip weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new bow
                                    ItemEquiped(fromItem, 6);
                                    //Visualy update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Check if user has arrows
                                    byte ammoslot = GetAmmoSlot(Character);
                                    //If the user has arrows
                                    if (ammoslot != 0)
                                    {
                                        //Get the arrow information
                                        ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                        //Set the amount of arrows globally
                                        Character.Information.Item.sAmount = AmmoItem.Amount;
                                        Character.Information.Item.sID = AmmoItem.ID;
                                        //Now we update the remaining information
                                        GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                        //Update the slot information in database
                                        DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    }
                                    //Update database info
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database info
                                    DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If player wants to equip another weapon type
                                #region Done
                                if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                                {
                                    //Unequip our bow
                                    ItemUnEquiped(weaponItem);
                                    //Equip new weapon
                                    ItemEquiped(fromItem, 6);
                                    //Visual update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Update Database
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update Database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                }
                                #endregion
                            }
                        }
                    }
                    //If no weapon is equiped
                    else
                    {
                        //If shield has been equipped.
                        if (shieldItem.ID != 0)
                        {
                            //If player wants to equip a bow
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || 
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                            {
                                #region Done
                                //Equip the bow
                                ItemEquiped(fromItem, 6);
                                //Unequip shield item
                                ItemUnEquiped(shieldItem);
                                //Send visual equip bow
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Get arrows in inventory
                                byte ammoslot = GetAmmoSlot(Character);
                                //If player has arrows
                                if (ammoslot != 0)
                                {
                                    //Get the arrow information
                                    ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update the slot information in database
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    //Update in database
                                    DB.query("UPDATE char_items SET itemnumber='item" + ammoslot + "',slot='" + ammoslot + "' WHERE id='" + shieldItem.dbID + "'");
                                }
                                else
                                {
                                    GetUpdateSlot(shieldItem, fromSlot, 0, 1);
                                    //Update in database
                                    DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                }
                                //Update in database
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            //If 2 handed weapon
                            #region Done
                            else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                    ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                            {
                                //Unequip old item
                                ItemUnEquiped(shieldItem);
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Visual update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Visual update
                                GetUpdateSlot(shieldItem, fromSlot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                //Update database information
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                            //If one handed weapon
                            #region Done
                            else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD)
                            {
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Visually update the inventory
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database information
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                        //Nothing equipped
                        else
                        {
                            //If player wants to equip bow
                            #region Done
                            if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                            {

                                //Equip the bow
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Updat database information
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                //Check if player has arrows in equipment
                                byte ammoslot = GetAmmoSlot(Character);
                                //If the player has arrows
                                if (ammoslot != 0)
                                {
                                    //Get arrow information
                                    ObjData.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Equip the arrows
                                    ItemEquiped(fromItem, 7);
                                    //Set global information                                 
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Visually show changes
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update database information
                                    DB.query("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                }
                            }
                            #endregion
                            //If player wants to equip other weapons
                            #region Equip other items (done)
                            else if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.EU_TSWORD)
                            {
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Show visual changes
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database information
                                DB.query("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                //Clothing items (Armor / Jewelry).
                #region Clothing items
                //Check if our item is a none weapon type and clothing type
                if (ObjData.Manager.ItemBase[fromItem.ID].TypeID2 == 1 && toSlot != 6 && toSlot != 7 && toSlot < 13)
                {

                    if (toSlot < 13)
                    {
                        //First we check if our level is high enough.
                        if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                        {
                            client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                            return;
                        }
                        //Then we check if the gender is the same as ours.
                        else if (!CheckGender(Character.Information.Model, fromItem.ID))
                        {
                            client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_GENDER_MISMATCH));
                            return;
                        }
                        //Then we check the race the item belongs to EU / CH.
                        else if (!CheckRace(Character.Information.Model, fromItem.ID))
                        {
                            client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                            return;
                        }
                        //Then we check if armor type equals the current equipped ones.
                        else if (!CheckArmorType(fromItem.ID, Character.Information.CharacterID))
                        {
                            client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UITT_MSG_CUSTOM_ARMOR_TYPE_WRONG));
                            return;
                        }
                        //All checks ok we equip the item
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Unequip item
                                ItemUnEquiped(toItem);
                                //Equip the new item
                                ItemEquiped(fromItem, toSlot);
                                //Update visual
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                            }
                            else
                            {
                                //Equip the new item
                                ItemEquiped(fromItem, toSlot);
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    else if (fromSlot < 13)
                    {
                        if (toItem.ID != 0)
                        {
                            byte new_slot = GetFreeSlot();
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, new_slot, 0, 1);
                            DB.query("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    else
                    {
                        if (toItem.ID != 0)
                        {
                            byte new_slot = GetFreeSlot();
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, new_slot, 0, 1);
                            DB.query("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                }
                #endregion
                //Normal item movement non equpping inside inventory
                #region Normal inventory movement
                short newquantity = 0;
                short fromquantity = 0;
                if (fromSlot >= 13 && toSlot >= 13)
                {
                    if (toItem.ID != 0)
                    {
                        if (toItem.ID == fromItem.ID)
                        {
                            if (ObjData.Manager.ItemBase[fromItem.ID].TypeID2 == 3 && ObjData.Manager.ItemBase[toItem.ID].TypeID2 == 3)
                            {
                                if (ObjData.Manager.ItemBase[fromItem.ID].Max_Stack > 1)
                                {
                                    newquantity = (short)(fromItem.Amount + toItem.Amount);
                                    if (newquantity > ObjData.Manager.ItemBase[fromItem.ID].Max_Stack)
                                    {
                                        GetUpdateSlot(fromItem, toSlot, 0, fromItem.Amount);
                                        DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                        DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                                    }
                                    else if (newquantity <= ObjData.Manager.ItemBase[fromItem.ID].Max_Stack)
                                    {
                                        DB.query("delete from char_items where id='" + fromItem.dbID + "'");
                                        DB.query("UPDATE char_items SET quantity='" + newquantity + "' WHERE id='" + toItem.dbID + "'");
                                        GetUpdateSlot(fromItem, toSlot, 0, newquantity);
                                    }
                                    else
                                    {
                                        fromquantity = (short)(newquantity % ObjData.Manager.ItemBase[fromItem.ID].Max_Stack);
                                        newquantity -= fromquantity;
                                        DB.query("UPDATE char_items SET quantity='" + fromquantity + "' WHERE id='" + fromItem.dbID + "'");
                                        DB.query("UPDATE char_items SET quantity='" + newquantity + "' WHERE id='" + toItem.dbID + "'");
                                    }
                                }
                            }
                            else
                            {
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                            }
                        }
                        else
                        {
                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                        }
                    }
                    else
                    {
                        if (fromItem.Amount != quantity && ObjData.Manager.ItemBase[fromItem.ID].TypeID2 == 3)
                        {
                            AddItem(fromItem.ID, quantity, toSlot, Character.Information.CharacterID, 0);
                            int calc = (fromItem.Amount - quantity);
                            if (calc < 1) calc = 1;
                            GetUpdateSlot(fromItem, toSlot,0, quantity);

                            DB.query("UPDATE char_items SET quantity='" + calc + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else if (toItem.ID != 0)
                        {

                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            DB.query("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                        }
                        else
                        {
                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                }
                #endregion
                //Unequip from slot
                #region Unequip items
                if (fromSlot < 13 && toSlot > 13)
                {
                    #region From slot 6
                    ObjData.slotItem weaponitem = GetItem((uint)Character.Information.CharacterID, 6, 0);
                    ObjData.slotItem shieldslotitem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    //If we unequip a bow or other item
                    if (fromSlot == 6 && shieldslotitem.ID != 0)
                    {
                        //if we unequip a bow
                        if (ObjData.Manager.ItemBase[weaponitem.ID].Itemtype == ObjData.item_database.ItemType.BOW || ObjData.Manager.ItemBase[weaponitem.ID].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW)
                        {
                            //If we drag it to a none free slot
                            if (toItem.ID != 0)
                            {
                                //Free arrow slot
                                byte freeweaponslot = GetFreeSlot();
                                //Unequip the arrows
                                ItemUnEquiped(shieldslotitem);
                                //Update visually arrow
                                GetUpdateSlot(shieldslotitem, freeweaponslot, 0, quantity);
                                //Update database bow
                                DB.query("UPDATE char_items SET itemnumber='item" + freeweaponslot + "',slot='" + freeweaponslot + "' WHERE id='" + shieldslotitem.dbID + "'");
                                //Free weapon slot
                                byte freearrowslot = GetFreeSlot();
                                //Unequip the weapon
                                ItemUnEquiped(fromItem);
                                //Update visually bow
                                GetUpdateSlot(fromItem, freearrowslot, 0, 1);
                                //Update global information
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database arrow
                                DB.query("UPDATE char_items SET itemnumber='item" + freearrowslot + "',slot='" + freearrowslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            //If we drag to free slot
                            else
                            {
                                //Unequip the arrow
                                ItemUnEquiped(fromItem);
                                //Unequip the weapon
                                ItemUnEquiped(shieldslotitem);
                                //Update visually bow
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update bow database
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                //Free weapon slot
                                byte newslot = GetFreeSlot();
                                //Update visually arrows
                                GetUpdateSlot(shieldslotitem, newslot, 0, quantity);
                                //Update global information
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update arrow in database
                                DB.query("UPDATE char_items SET itemnumber='item" + newslot + "',slot='" + newslot + "' WHERE id='" + shieldslotitem.dbID + "'");
                            }
                        }
                        //If we unequip another weapon type and shield is equiped we keep the shield.
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Free weapon slot
                                byte weaponslot = GetFreeSlot();
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, weaponslot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + weaponslot + "',slot='" + weaponslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    //If no shield item has been equiped
                    else if (fromSlot == 6 && shieldslotitem.ID == 0)
                    {
                        if (toItem.ID != 0)
                        {
                            //Free weapon slot
                            byte weaponslot = GetFreeSlot();
                            //Unequip our weapon
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, weaponslot, 0, 1);
                            //Update database
                            DB.query("UPDATE char_items SET itemnumber='item" + weaponslot + "',slot='" + weaponslot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            //Update database
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    #endregion
                    //It we unequip a shield or arrows
                    #region From slot 7
                    else if (fromSlot == 7)
                    {
                        //if we unequip arrows
                        if (ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.ARROW || ObjData.Manager.ItemBase[fromItem.ID].Itemtype == ObjData.item_database.ItemType.BOLT)
                        {
                            if (toItem.ID != 0)
                            {
                                //Free arrow slot
                                byte arrowslot = GetFreeSlot();
                                //Unequip our arrows
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, arrowslot, 0, quantity);
                                //Set global data
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + arrowslot + "',slot='" + arrowslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, quantity);
                                //Set global data
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                        //If we unequip shields
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Free weapon slot
                                byte newshieldslot = GetFreeSlot();
                                //Unequip our weapon
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, newshieldslot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + newshieldslot + "',slot='" + newshieldslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    #endregion
                    //Other slots
                    #region Other from slots
                    else
                    {
                        if (toItem.ID != 0)
                        {
                            //Free weapon slot
                            byte newunequipslot = GetFreeSlot();
                            //Unequip our weapon
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, newunequipslot, 0, 1);
                            //Update database
                            DB.query("UPDATE char_items SET itemnumber='item" + newunequipslot + "',slot='" + newunequipslot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            //Update database
                            DB.query("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    #endregion
                }
                #endregion
                //Save player information
                SavePlayerInfo();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Unequip Items
        /////////////////////////////////////////////////////////////////////////////////
        public void ItemUnEquiped(ObjData.slotItem item)
        {
            #region Unequip items
            LoadBluesid(item.dbID);
            try
            {
                if (item.Slot <= 5)
                {
                    Character.Stat.MagDef -= ObjData.Manager.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.MagDefINC);
                    Character.Stat.PhyDef -= ObjData.Manager.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.PhyDefINC);
                    Character.Stat.Parry -= ObjData.Manager.ItemBase[item.ID].Defans.Parry;
                    if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    client.Send(Packet.PlayerStat(Character));
                }
                if (item.Slot == 6)
                {
                    Character.Stat.MinPhyAttack -= ObjData.Manager.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.PhyAttackInc);
                    Character.Stat.MaxPhyAttack -= ObjData.Manager.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.PhyAttackInc);
                    Character.Stat.MinMagAttack -= ObjData.Manager.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.MagAttackINC);
                    Character.Stat.MaxMagAttack -= ObjData.Manager.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.MagAttackINC);
                    Character.Stat.Hit -= ObjData.Manager.ItemBase[item.ID].Attack.MinAttackRating;
                    if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    client.Send(Packet.PlayerStat(Character));
                    Character.Information.Item.wID = 0;
                }
                if (item.Slot == 7)
                {
                    if (ObjData.Manager.ItemBase[item.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[item.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                    {
                        Character.Stat.MagDef -= ObjData.Manager.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.MagDefINC);
                        Character.Stat.PhyDef -= ObjData.Manager.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.PhyDefINC);
                        if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    }
                    client.Send(Packet.PlayerStat(Character));
                    Character.Information.Item.sAmount = 0;
                    Character.Information.Item.sID = 0;
                }
                if (item.Slot == 8)
                {
                    if (Character.Job.Jobname != "0" && Character.Job.state == 1)
                    {
                        //Teleport user back to binded location
                       Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                        Character.Information.Scroll = true;
                        StartScrollTimer(0);
                        DB.query("UPDATE character_jobs SET job_state='0' WHERE character_name='" + Character.Information.Name + "'");
                        Character.Job.state = 0;
                        SavePlayerReturn();
                    }
                }
                if (item.Slot >= 9 && item.Slot <= 12)
                {
                    Character.Stat.MagDef -= (short)(ObjData.Manager.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                    Character.Stat.PhyDef -= (short)(ObjData.Manager.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                    client.Send(Packet.PlayerStat(Character));
                    if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                }
                Send(Packet.MoveItemUnequipEffect(Character.Information.UniqueID, item.Slot, item.ID));
                SavePlayerInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Equiped items
        /////////////////////////////////////////////////////////////////////////////////
        public void ItemEquiped(ObjData.slotItem item, byte slot)
        {
            #region Equiped items
            try
            {       
                if (Character.Information.Level >= ObjData.Manager.ItemBase[item.ID].Level)
                {
                    if (slot <= 5)
                    {
                        Character.Stat.MagDef += ObjData.Manager.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.MagDefINC);
                        Character.Stat.PhyDef += ObjData.Manager.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.PhyDefINC);
                        Character.Stat.Parry += ObjData.Manager.ItemBase[item.ID].Defans.Parry;
                        if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 6)
                    {
                        Character.Stat.MinPhyAttack += ObjData.Manager.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.PhyAttackInc);
                        Character.Stat.MaxPhyAttack += ObjData.Manager.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.PhyAttackInc);
                        Character.Stat.MinMagAttack += ObjData.Manager.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.MagAttackINC);
                        Character.Stat.MaxMagAttack += ObjData.Manager.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Attack.MagAttackINC);
                        Character.Stat.Hit += ObjData.Manager.ItemBase[item.ID].Attack.MinAttackRating;
                        Character.Information.Item.wID = item.ID;
                        if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 7)
                    {
                        if (ObjData.Manager.ItemBase[item.ID].Itemtype == ObjData.item_database.ItemType.CH_SHIELD || ObjData.Manager.ItemBase[item.ID].Itemtype == ObjData.item_database.ItemType.EU_SHIELD)
                        {
                            Character.Stat.MagDef += ObjData.Manager.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.MagDefINC);
                            Character.Stat.PhyDef += ObjData.Manager.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.PhyDefINC);
                            if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        }
                        Character.Information.Item.sAmount = item.Amount;
                        Character.Information.Item.sID = item.ID;
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 8)
                    {
                        //Teleport user back to binded location
                        Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                        Character.Information.Scroll = true;
                        StartScrollTimer(0);//Set to job time info
                        DB.query("UPDATE character_jobs SET job_state='1' WHERE character_name='" + Character.Information.Name + "'");
                        Character.Job.state = 1;
                        SavePlayerReturn();
                    }
                    if (slot >= 9 && slot <= 12)
                    {
                        Character.Stat.MagDef += (short)(ObjData.Manager.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                        Character.Stat.PhyDef += (short)(ObjData.Manager.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * (double)ObjData.Manager.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                        client.Send(Packet.PlayerStat(Character));
                        if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                    }
                    Send(Packet.MoveItemEnquipEffect(Character.Information.UniqueID, item.Slot, item.ID, item.PlusValue));
                    SavePlayerInfo();
                }
                else
                    return;
            }

            catch (Exception ex)
            {
                Log.Exception("Item equip error: ", ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Avatar Unequiped
        /////////////////////////////////////////////////////////////////////////////////
        void ItemAvatarUnEquip(byte fromSlot, byte toSlot)
        {
            #region Avatar unequiped
            try
            {
                GetFreeSlot();
                ObjData.slotItem toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 0);
                int avatarid = 0;
                int dbID = 0;

                if (toItem.ID != 0) toSlot = GetFreeSlot();
                if (toSlot <= 12) return;

                DB ms = new DB("SELECT * FROM char_items WHERE itemnumber='avatar" + fromSlot + "' AND owner='" + Character.Information.CharacterID + "' AND inAvatar='1'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        avatarid = reader.GetInt32(2);
                        dbID = reader.GetInt32(0);
                    }
                }
                ms.Close();

                client.Send(Packet.MoveItem(35, fromSlot, toSlot, 1,0,"MOVE_INSIDE_INVENTORY"));
                Send(Packet.MoveItemUnequipEffect(Character.Information.UniqueID, fromSlot, avatarid));

                string nonquery = "UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "',inAvatar='0' WHERE owner='" + Character.Information.CharacterID + "' AND itemnumber='avatar" + fromSlot + "' AND id='" + dbID + "'";
                DB.query(nonquery);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Avatar Equiped
        /////////////////////////////////////////////////////////////////////////////////
        void ItemAvatarEquip(byte fromSlot, byte toSlot)
        {
            #region Avatar equiped
            try
            {
                GetFreeSlot();
                ObjData.slotItem toItem = new ObjData.slotItem();
                ObjData.slotItem fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 0);

                DB ms = new DB("SELECT * FROM char_items WHERE itemnumber='avatar" + toSlot + "' AND owner='" + Character.Information.CharacterID + "'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        toItem.ID = reader.GetInt32(2);
                        toItem.dbID = reader.GetInt32(0);
                    }
                }
                ms.Close();

                if (toItem.ID != 0) return;

                if (fromItem.ID == 0) return;
                if (!CheckGender(Character.Information.Model, fromItem.ID))
                {
                    return;
                }

                else
                {
                    string nonquery = "UPDATE char_items SET itemnumber='avatar" + toSlot + "',inAvatar='1',slot='" + toSlot + "' WHERE owner='" + Character.Information.CharacterID + "' AND itemnumber='item" + fromSlot + "' AND id='" + fromItem.dbID + "'";
                    DB.query(nonquery);
                }
                Send(Packet.MoveItemEnquipEffect(Character.Information.UniqueID, toSlot, fromItem.ID, 0));
                client.Send(Packet.MoveItem(36, fromSlot, toSlot, 1, 0, "MOVE_INSIDE_INVENTORY"));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Drop Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_DropItem(byte slot)
        {
            #region Drop Item
            try
            {
                if (Character.Action.nAttack) return; 
                if (Character.Action.sAttack) return; 
                if (Character.Stall.Stallactive) return;
                if (Character.State.Exchanging) return;
                if (Character.Alchemy.working) return;
                if (Character.State.Busy) return;
                if (Character.Network.Guild.UsingStorage) return;
                //Else we continue
                else
                {
                    //Get item information from slot.
                    ObjData.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);
                    //Check if the item is a item mall item before dropping.
                    if (ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.AVATAR28D ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.CHANGESKIN ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.GLOBALCHAT ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.INVENTORYEXPANSION ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.REVERSESCROLL ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.STALLDECORATION ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.WAREHOUSE ||
                        ObjData.Manager.ItemBase[item.ID].Etctype == ObjData.item_database.EtcType.AVATAR28D ||
                        ObjData.Manager.ItemBase[item.ID].Type == ObjData.item_database.ArmorType.AVATAR ||
                        ObjData.Manager.ItemBase[item.ID].Type == ObjData.item_database.ArmorType.AVATARATTACH ||
                        ObjData.Manager.ItemBase[item.ID].Type == ObjData.item_database.ArmorType.AVATARHAT ||
                        ObjData.Manager.ItemBase[item.ID].Pettype == ObjData.item_database.PetType.ATTACKPET ||
                        ObjData.Manager.ItemBase[item.ID].Pettype == ObjData.item_database.PetType.GRABPET)
                        return;
                    //If the id model is lower then 4 dont allow to drop.
                    //Gold drop requires another drop part.
                    if (item.ID < 4) return;
                    //Anti hack check
                    int owner = Convert.ToInt32(DB.GetData("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner"));
                    //If the player really is the owner of this item we continue to drop it.
                    if (owner == Character.Information.CharacterID)
                    {
                        //Check for item amount.
                        if (item.Amount <= ObjData.Manager.ItemBase[item.ID].Max_Stack)
                        {
                            //Spawn new item globally
                            WorldMgr.Items sitem = new WorldMgr.Items();
                            sitem.Model = item.ID;
                            sitem.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.World);
                            sitem.UniqueID = sitem.Ids.GetUniqueID;
                            sitem.amount = item.Amount;
                            sitem.PlusValue = item.PlusValue;
                            sitem.x = Character.Position.x;
                            sitem.z = Character.Position.z;
                            sitem.y = Character.Position.y;
                            sitem.CalculateNewPosition();
                            sitem.xSec = Character.Position.xSec;
                            sitem.ySec = Character.Position.ySec;

                            #region Amount definition
							if (ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.BLADE ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.CH_SHIELD ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_SHIELD ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.BOW ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.SPEAR ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.SWORD ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.EARRING ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.RING ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.NECKLACE ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.ARMOR ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.GARMENT ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.GM ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.HEAVY ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.LIGHT ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.PROTECTOR ||
								 ObjData.Manager.ItemBase[sitem.Model].Itemtype == ObjData.item_database.ItemType.AVATAR ||
								 ObjData.Manager.ItemBase[sitem.Model].Type == ObjData.item_database.ArmorType.ROBE)
								sitem.Type = 2;
							else
								sitem.Type = 3;
                            #endregion

                            sitem.fromType = 6;
                            sitem.fromOwner = Character.Information.UniqueID;
                            sitem.downType = false;
                            sitem.Owner = 0;
                            sitem.Send(Packet.ObjectSpawn(sitem), true);
                            Helpers.Manager.WorldItem.Add(sitem);
                            //Send visual packet for removing the item from inventory.
                            client.Send(Packet.DespawnFromInventory(sitem.UniqueID));
                            client.Send(Packet.MoveItem(7, slot, 0, 0, 0, "DELETE_ITEM"));
                            //Update database and remove the item
                            DB.query("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Character.Information.CharacterID + "'");
                            //Save player information
                            SavePlayerInfo();
                        }
                        else
                            return;
                    }
                    //If the player is not the owner of the item beeing dropped we ban the player.
                    else
                    {
                        Disconnect("ban");
                        Log.Exception("Autobanned user: " + Player.AccountName + " Due to hacking");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Drop Gold
        /////////////////////////////////////////////////////////////////////////////////
        void Player_DropGold(ulong Gold)
        {
            #region Drop Gold
            try
            {
                if (Character.Action.nAttack) return;
                if (Character.Action.sAttack) return;
                if (Character.Stall.Stallactive) return;
                if (Character.State.Exchanging) return;
                if (Character.Alchemy.working) return;
                if ((ulong)Character.Information.Gold >= Gold)
                {
                    GetFreeSlot();
                    WorldMgr.Items item = new WorldMgr.Items();
                    item.amount = (int)Gold;
                    item.Model = 1;
                    if (item.amount < 1000) item.Model = 1;
                    else if (item.amount > 1000 && item.amount < 10000) item.Model = 2;
                    else if (item.amount > 10000) item.Model = 3;
                    item.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.World);
                    item.UniqueID = item.Ids.GetUniqueID;
                    item.x = Character.Position.x;
                    item.z = Character.Position.z;
                    item.y = Character.Position.y;
                    item.CalculateNewPosition();
                    item.xSec = Character.Position.xSec;
                    item.ySec = Character.Position.ySec;
                    item.Type = 1;
                    item.fromType = 6;
                    item.Owner = 0;
                    Helpers.Manager.WorldItem.Add(item);
                    item.Send(Packet.ObjectSpawn(item), true);
                    Character.Information.Gold -= (long)Gold;
                    client.Send(Packet.InfoUpdate(1, (int)Character.Information.Gold, 0));
                    client.Send(Packet.MoveItem(0x0A, 0, 0, 0, (long)Gold, "DELETE_GOLD"));
                    SaveGold();
                }
                else
                {
                    client.Send(Packet.Message(OperationCode.SERVER_UPDATEGOLD, Messages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Buy Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_BuyItem(byte selected_tab, byte selected_line, short buy_amount, int selected_npc_id)
        {
            #region Buy item
            try
            {
                //Create new object
                WorldMgr.Monsters npc_details = Helpers.GetInformation.GetObject(selected_npc_id);
                //Get shop line information
                string s = ObjData.Manager.ObjectBase[npc_details.ID].Tab[selected_tab];
                //Get shop index content
                ObjData.shop_data shopitem = ObjData.shop_data.GetShopIndex(s);
                //Get items
                short items = (short)ObjData.Manager.ItemBase[ObjData.item_database.GetItem(shopitem.Item[selected_line])].ID;
                //Set item price
                int gold = ObjData.Manager.ItemBase[items].Shop_price;
                //Check slot information
                byte slot = GetFreeSlot();
                if (slot <= 12) return;
                //Check the max amount of the item.
                if (buy_amount <= ObjData.Manager.ItemBase[ObjData.item_database.GetItem(shopitem.Item[selected_line])].Max_Stack)
                {
                    //Get item id
                    int Itemidinfo = ObjData.Manager.ItemBase[ObjData.item_database.GetItem(shopitem.Item[selected_line])].ID;
                    //Check if what the player is buying is a wearable type and not ETC so max stack is 1
                    if (ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.SPEAR ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.GLAVIE ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.SWORD ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.BLADE ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_AXE ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_DAGGER ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_DARKSTAFF ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_HARP ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_STAFF ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_SWORD ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_TSTAFF ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_TSWORD ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.CH_SHIELD ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_SHIELD ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.BOW ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EU_CROSSBOW ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.HUNTERSUIT ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.THIEFSUIT ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.TRADERSUIT ||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.ARMOR||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.GARMENT ||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.HEAVY ||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.LIGHT ||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.PROTECTOR ||
                                ObjData.Manager.ItemBase[Itemidinfo].Type == ObjData.item_database.ArmorType.ROBE ||
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.EARRING || 
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.RING || 
                                ObjData.Manager.ItemBase[Itemidinfo].Itemtype == ObjData.item_database.ItemType.NECKLACE 
                        )
                        buy_amount = 1;
                    //Multiply the amount / price.
                    gold *= buy_amount;
                    //Reduct the gold from the player

                    //If the character gold equals or is higher then the price , continue
                    if (Character.Information.Gold >= gold)
                    {
                        string str = string.Format("OLD GOLD:{0} EACH PRICE:{1}",Character.Information.Gold,gold);
                        
                        Character.Information.Gold -= gold;
                        str += " NEW GOLD:" + Character.Information.Gold + " MYCALC:" + (Character.Information.Gold - gold).ToString();
                        //Send packet to update gold visual
                        client.Send(Packet.ChatPacket(7, 0, str, ""));
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        //Save player gold
                        SaveGold();
                        //Add new item to user
                        client.Send(Packet.MoveItemBuy(8, selected_tab, selected_line, (byte)buy_amount, slot, buy_amount));
                        //Amount information
                        if (buy_amount > 1)
                            AddItem(items, buy_amount, slot, Character.Information.CharacterID, 0);
                        else if (buy_amount == 1)
                            AddItem(items, 0, slot, Character.Information.CharacterID, 0);
                    }
                    else
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                    }
                }
                else
                {
                    client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(string.Format("Player_BuyItem({0},{1},{2},{3})::Error..", selected_tab, selected_line, buy_amount, selected_npc_id), ex);
            }
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Sell Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_SellItem(byte slot, short amount, int o_id)
        {
            #region Sell item
            try
            {
                
                ObjData.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);
                int SellPrice = ObjData.Manager.ItemBase[item.ID].Sell_Price * amount;
                Character.Information.Gold += SellPrice;
                // Tax
                /*string str = "GOLD:" + Character.Information.Gold + " SELL PRICE:" + SellPrice;
                if (NPC.Info(ObjData.Manager.ObjectBase[o_id].Name))
                {
                    
                        int testprice  = (int)(SellPrice * 0.2);
                        
                    Character.Information.Gold -= SellPrice - testprice;
                    str += " TAX PRICE:" + testprice + " NEW GOLD:" + Character.Information.Gold;
                }
                client.Send(Packet.ChatPacket(7, 0, str, ""));*/
                client.Send(Packet.UpdateGold(Character.Information.Gold));
                SaveGold();
                int owner = Convert.ToInt32(DB.GetData("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner"));
                if (owner == Character.Information.CharacterID)
                {
                    if (amount <= ObjData.Manager.ItemBase[item.ID].Max_Stack)
                    {
                        client.Send(Packet.MoveItemSell(9, slot, amount, o_id));
                        if (item.Amount != amount)
                        {
                            int calc = (item.Amount - amount);
                            if (calc < 1) calc = 1;
                            DB.query("UPDATE char_items SET quantity='" + calc + "' WHERE itemnumber='" + "item" + slot + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + item.ID + "'");
                        }
                        else
                        {
                            DB.query("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Character.Information.CharacterID + "'");
                        }
                        Character.Buy_Pack.Add(item);
                    }
                    else
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                    }
                }
                else
                {
                    Disconnect("ban");
                    Log.Exception("Autobanned user: " + Player.AccountName + " Due to hacking");
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Buy back item (Needs work).
        /////////////////////////////////////////////////////////////////////////////////
        public void Player_BuyPack()
        {
            #region Player Buy Back
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                int id = Reader.Int32(); byte b_slot = Reader.Byte();
                Reader.Close();
                byte i_slot = GetFreeSlot();
                if (i_slot <= 12) return;

                Print.Format(b_slot.ToString());

                ObjData.slotItem item = Character.Buy_Pack.Get(b_slot);
                if (item.Amount < 1) item.Amount = 1;
                if (item.Amount <= ObjData.Manager.ItemBase[item.ID].Max_Stack)
                {
                    if (Character.Information.Gold >= ObjData.Manager.ItemBase[item.ID].Sell_Price)
                    {
                        Character.Information.Gold -= ObjData.Manager.ItemBase[item.ID].Sell_Price;
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                    }
                    else
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_UPDATEGOLD, Messages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                    }
                    SaveGold();
                    if (ObjData.Manager.ItemBase[item.ID].TypeID2 == 1)
                    {
                        DB.query("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + ObjData.Manager.ItemBase[item.ID].Defans.Durability + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "')");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, 1));
                    }
                    else if (ObjData.Manager.ItemBase[item.ID].TypeID2 == 2)
                    {
                        DB.query("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.Amount + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (ObjData.Manager.ItemBase[item.ID].TypeID2 == 3)
                    {
                        DB.query("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.Amount + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (ObjData.Manager.ItemBase[item.ID].TypeID2 == 4)
                    {
                        DB.query("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + 1 + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (ObjData.Manager.ItemBase[item.ID].TypeID2 == 6)
                    {
                        DB.query("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + 1 + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                }
                else
                {
                    client.Send(Packet.Message(OperationCode.SERVER_ITEM_MOVE, Messages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            SavePlayerInfo();
            #endregion
        }
    }
}