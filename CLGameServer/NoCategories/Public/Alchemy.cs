using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        Timer Alchemy;
        /////////////////////////////////////////////////////////////////////////////////
        // Create stones
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyCreateStone()
        {
            try
            {
                //Open packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                byte type1 = reader.Byte();
                byte type2 = reader.Byte();
                byte tabletslot = reader.Byte();
                byte elementslot1 = reader.Byte();
                byte elementslot2 = reader.Byte();
                byte elementslot3 = reader.Byte();
                byte elementslot4 = reader.Byte();
                reader.Close();

                //Tablet information
                ObjData.slotItem tabletslotitem = GetItem((uint)Character.Information.CharacterID, tabletslot, 0);

                //Get stone information equaly to the tablet
                int stone = GetStoneFromTablet(tabletslotitem.ID);

                //Earth element information
                ObjData.slotItem element1slotitem = GetItem((uint)Character.Information.CharacterID, elementslot1, 0);
                //Water element information
                ObjData.slotItem element2slotitem = GetItem((uint)Character.Information.CharacterID, elementslot2, 0);
                //Fire element information
                ObjData.slotItem element3slotitem = GetItem((uint)Character.Information.CharacterID, elementslot3, 0);
                //Wind element information
                ObjData.slotItem element4slotitem = GetItem((uint)Character.Information.CharacterID, elementslot4, 0);

                //Check if the requirements are ok (Extra check amount).
                if (element1slotitem.Amount < ObjData.Manager.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_AMOUNT_REQ) return;
                if (element2slotitem.Amount < ObjData.Manager.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_AMOUNT_REQ) return;
                if (element3slotitem.Amount < ObjData.Manager.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_AMOUNT_REQ) return;
                if (element2slotitem.Amount < ObjData.Manager.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_AMOUNT_REQ) return;

                //Check if the requirements are ok (Extra check element name).
                if (ObjData.Manager.ItemBase[element1slotitem.ID].Name != ObjData.Manager.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_NAME) return;
                if (ObjData.Manager.ItemBase[element2slotitem.ID].Name != ObjData.Manager.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_NAME) return;
                if (ObjData.Manager.ItemBase[element3slotitem.ID].Name != ObjData.Manager.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_NAME) return;
                if (ObjData.Manager.ItemBase[element4slotitem.ID].Name != ObjData.Manager.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_NAME) return;

                //Update amount of elements
                element1slotitem.Amount -= (short)ObjData.Manager.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_AMOUNT_REQ;
                element2slotitem.Amount -= (short)ObjData.Manager.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_AMOUNT_REQ;
                element3slotitem.Amount -= (short)ObjData.Manager.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_AMOUNT_REQ;
                element4slotitem.Amount -= (short)ObjData.Manager.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_AMOUNT_REQ;

                ItemUpdateAmount(element1slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element2slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element3slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element4slotitem, Character.Information.CharacterID);

                //Update amount of tablet
                tabletslotitem.Amount -= 1;
                ItemUpdateAmount(tabletslotitem, Character.Information.CharacterID);

                //Send alchemy packet
                client.Send(Packet.StoneCreation(tabletslot));

                //Check for new free slots in inventory
                byte freeslot = GetFreeSlot();
                //Update database and insert new item
                AddItem(stone, 1, freeslot, Character.Information.CharacterID, 0);
                //Send visual packet add stone (creation works, just need to check why it sends 2x same packet).
                client.Send(Packet.GainElements(freeslot, stone, 1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stone creation error {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Elixir alchemy
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyElixirMain()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Character.Alchemy.ItemList = new List<ObjData.slotItem>();

                byte Type = Reader.Byte();

                if (Type == 1)
                {
                    try
                    {
                        Character.Alchemy.AlchemyThread.Abort();
                        client.Send(Packet.AlchemyCancel());
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
                else if (Type == 2)
                {
                    Reader.Skip(1);
                    byte numItem = Reader.Byte();

                    if (numItem == 2)
                    {
                        Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));
                        Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));

                    }
                    else if (numItem == 3)
                    {
                        Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));
                        Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));
                        Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));
                    }
                    Alchemy = new Timer(new TimerCallback(StartAlchemyElixirResponse), 0, 3000, 0);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void StartAlchemyElixirResponse(object e)
        {
            try
            {
                Character.Alchemy.AlchemyThread = new Thread(new ThreadStart(AlchemyElixirResponse));
                Character.Alchemy.AlchemyThread.Start();
                while (!Character.Alchemy.AlchemyThread.IsAlive) ;
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void AlchemyElixirResponse()
        {
            try
            {
                int chance = 0;
                bool success = false;
                Random plus = new Random();
                int sans = 100;
                int random = plus.Next(0, sans);
                // successrate table
                switch (Character.Alchemy.ItemList[0].PlusValue)
                {
                    case 0:
                        if (random > 10)
                            success = true;
                        break; // %90
                    case 1:
                        if (random > 20)
                            success = true;
                        break; // %80
                    case 2:
                        if (random > 30)
                            success = true;
                        break; // %70
                    case 3:
                        if (random > 40)
                            success = true;
                        break; // %60
                    case 4:
                        if (random > 55)
                            success = true;
                        break; // %45
                    case 5:
                        if (random > 60)
                            success = true;
                        break; // %40
                    case 6:
                        if (random > 67)
                            success = true;
                        break; // %33.3
                    case 7:
                        if (random > 75)
                            success = true;
                        break; // %25
                    case 8:
                        if (random > 82)
                            success = true;
                        break; // %18
                    case 9:
                        if (random > 88)
                            success = true;
                        break; // %12
                    default:
                        if (random > 92)
                            success = true;
                        break; // %8
                }
                // if with lucky
                if (Character.Alchemy.ItemList.Count == 3)
                {
                    sans = sans - 5; // i think works fine
                    // dec lucky powder amount
                    Character.Alchemy.ItemList[2].Amount--;
                    ItemUpdateAmount(Character.Alchemy.ItemList[2], Character.Information.CharacterID);
                }
                // update plus value
                if (success)
                {
                    Character.Alchemy.ItemList[0].PlusValue++;
                    DB.query("UPDATE char_items SET plusvalue='" + Character.Alchemy.ItemList[0].PlusValue + "' WHERE slot='" + Character.Alchemy.ItemList[0].Slot + "' AND owner='" + Character.Information.CharacterID + "'");
                }
                else
                {
                    if (Character.Alchemy.ItemList[0].PlusValue >= 4)
                    {
                        if (ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_ASTRAL"))
                        {
                            Character.Alchemy.ItemList[0].PlusValue = 4;
                        }
                        else
                        {
                            Character.Alchemy.ItemList[0].PlusValue = 0;
                        }
                    }
                    else
                    {
                        Character.Alchemy.ItemList[0].PlusValue = 0;
                    }
                    DB.query("UPDATE char_items SET plusvalue='0' WHERE slot ='" + Character.Alchemy.ItemList[0].Slot + "' AND owner='" + Character.Information.CharacterID + "'");
                }
                client.Send(Packet.AlchemyResponse(success, Character.Alchemy.ItemList[0], 1, Convert.ToByte(ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue)));
                //delete elixir
                DB.query("DELETE FROM char_items WHERE slot='" + Character.Alchemy.ItemList[1].Slot + "' AND owner='" + Character.Information.CharacterID + "'");
                client.Send(Packet.MoveItem(0x0F, Character.Alchemy.ItemList[1].Slot, 0, 0, 0, "DELETE_ITEM"));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy Error: {0}", ex);
                Log.Exception(ex);
            }

        }
        /////////////////////////////////////////////////////////////////////////////////
        // Item reinforce with stones
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyStoneMain()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Character.Alchemy.ItemList = new List<ObjData.slotItem>();
                byte type = Reader.Byte();
                if (type == 1)
                {
                    try
                    {
                        Character.Alchemy.AlchemyThread.Abort();
                        client.Send(Packet.AlchemyCancel());
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
                else if (type == 2)
                {
                    Reader.Skip(1);
                    byte numitem = Reader.Byte();
                    Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));
                    Character.Alchemy.ItemList.Add(GetItem((uint)Character.Information.CharacterID, Reader.Byte(), 0));

                }
                Alchemy = new Timer(new TimerCallback(StartAlchemyStoneResponse), 0, 4500, 0);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void StartAlchemyStoneResponse(object e)
        {
            try
            {
                Character.Alchemy.AlchemyThread = new Thread(new ThreadStart(AlchemyStoneResponse));
                Character.Alchemy.AlchemyThread.Start();
                while (!Character.Alchemy.AlchemyThread.IsAlive) ;
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void AlchemyStoneResponse()
        {
            try
            {
                Random rnd = new Random();
                int random = rnd.Next(1, 100);
                bool success = true;
                LoadBluesid(Character.Alchemy.ItemList[0].dbID);
                if (random <= 70)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }

                if (success)
                {
                    if (ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue <= ObjData.Manager.ItemBase[Character.Alchemy.ItemList[0].ID].MaxBlueAmount)
                    {
                        Random blue = new Random();
                        int min = ObjData.Manager.MagicOptions.Find(aa => (aa.Name == ObjData.Manager.ItemBase[Character.Alchemy.ItemList[1].ID].ObjectName) && aa.Level == GetItemDegree(Character.Alchemy.ItemList[0]) + 1).MinValue;
                        int max = ObjData.Manager.MagicOptions.Find(aa => (aa.Name == ObjData.Manager.ItemBase[Character.Alchemy.ItemList[1].ID].ObjectName) && aa.Level == GetItemDegree(Character.Alchemy.ItemList[0]) + 1).MaxValue;
                        int value = blue.Next(min, max);
                        if (ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.Contains(ObjData.Manager.ItemBase[Character.Alchemy.ItemList[1].ID].ObjectName))
                        {
                            int index = ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.IndexOf(ObjData.Manager.ItemBase[Character.Alchemy.ItemList[1].ID].ObjectName);
                            index++;
                            DB.query("UPDATE char_items SET blue" + index + "amount='" + value + "' WHERE id='" + Character.Alchemy.ItemList[0].dbID + "'");
                        }
                        else
                        {
                            ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue++;
                            DB.query("UPDATE char_items SET BlueAmount='" + ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue + "',blue" + ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue + "='" + ObjData.Manager.ItemBase[Character.Alchemy.ItemList[1].ID].ObjectName + "',blue" + ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].totalblue + "amount='" + value + "' WHERE id='" + Character.Alchemy.ItemList[0].dbID + "'");
                        }
                    }
                    else
                        return;
                }

                LoadBluesid(Character.Alchemy.ItemList[0].dbID);
                if (ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_DUR"))
                {
                    Character.Alchemy.ItemList[0].Durability += Character.Alchemy.ItemList[0].Durability * (Convert.ToInt32(ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue[ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.IndexOf("MATTR_DUR")]) / 100);
                }
                if (ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_REINFORCE_ITEM"))
                {
                    Character.Alchemy.ItemList[0].PlusValue += Convert.ToByte(ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue[ObjData.Manager.ItemBlue[Character.Alchemy.ItemList[0].dbID].blue.IndexOf("MATTR_REINFORCE_ITEM")]);
                }
                DB.query("UPDATE char_items SET durability='" + Character.Alchemy.ItemList[0].Durability + "',plusvalue='" + Character.Alchemy.ItemList[0].PlusValue + "' WHERE id='" + Character.Alchemy.ItemList[0].dbID + "'");
                client.Send(Packet.AlchemyStoneResponse(success, Character.Alchemy.ItemList[0]));
                DB.query("DELETE FROM char_items WHERE slot='" + Character.Alchemy.ItemList[1].Slot + "' AND owner='" + Character.Information.CharacterID + "'");
                client.Send(Packet.MoveItem(0x0F, Character.Alchemy.ItemList[1].Slot, 0, 0, 0, "DELETE_ITEM"));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get degree information
        /////////////////////////////////////////////////////////////////////////////////
        public int GetItemDegree(ObjData.slotItem item)
        {
            try
            {
                if (1 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 8)
                    return 1;
                else if (8 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 16)
                    return 2;
                else if (16 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 24)
                    return 3;
                else if (24 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 32)
                    return 4;
                else if (32 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 42)
                    return 5;
                else if (42 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 52)
                    return 6;
                else if (52 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 64)
                    return 7;
                else if (64 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 76)
                    return 8;
                else if (76 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 90)
                    return 9;
                else if (90 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 101)
                    return 10;
                else if (101 <= ObjData.Manager.ItemBase[item.ID].Level && ObjData.Manager.ItemBase[item.ID].Level < 110)
                    return 11;
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 1;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Item destruction with alchemy
        /////////////////////////////////////////////////////////////////////////////////
        public void BreakItem()
        {
            try
            {
                //Checks before we continue
                if (Character.Stall.Stallactive || Character.Action.nAttack || Character.Action.sAttack || Character.Alchemy.working)
                    return;
                //Set bool
                Character.Alchemy.working = true;
                //TODO: Timer for alchemy start / end
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte rondorequired = reader.Byte();
                byte slot = reader.Byte();
                reader.Close();


                //Get our item information (item)
                ObjData.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);

                //Get our degree information
                byte itemdegree = ObjData.Manager.ItemBase[item.ID].Degree;

                //First we get our elements (Same degree as weapon)
                //This should return 4 items
                //Add check rondo count if high enough.
                Character.Alchemy.Elementlist = GetDegreeElements(item.ID, Character);
                //Check if the item has any blues on it.
                if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0)
                    Character.Alchemy.StonesList = GetStonesDegree(item.ID, Character);

                //Check current free slots of the player
                byte slotcheck = GetFreeSlot();
                //If slot amount is lower then 4 return
                //Slots free must be 6 i believe because of stones (TODO: Check info official).
                if (slotcheck < 4)
                {
                    //Send error message inventory full ...
                    return;
                }
                //Player has enough slots so we continue adding the new items
                else
                {
                    //Update rondo quantity
                    Character.Information.InventorylistSlot = GetPlayerItems(Character);
                    foreach (byte e in Character.Information.InventorylistSlot)
                    {
                        //Set slotitem
                        ObjData.slotItem itemrondoinfo = GetItem((uint)Character.Information.CharacterID, e, 0);
                        if (itemrondoinfo.ID != 0)
                        {
                            if (ObjData.Manager.ItemBase[itemrondoinfo.ID].Etctype == ObjData.item_database.EtcType.DESTROYER_RONDO)
                            {
                                //Update amount
                                itemrondoinfo.Amount -= rondorequired;
                                ItemUpdateAmount(itemrondoinfo, Character.Information.CharacterID);
                            }
                        }
                    }
                    //Clean our list
                    Character.Information.InventorylistSlot.Clear();
                    //Remove the item used in dissembling (Query).
                    DB.query("DELETE FROM char_items WHERE id='" + item.dbID + "' AND owner='" + Character.Information.CharacterID + "'");
                    //Remove the item used in dissembling (Visual).
                    ItemUpdateAmount(item, Character.Information.CharacterID);
                    //Send packet #2 
                    client.Send(Packet.DestroyItem());
                    //Repeat for each element in our list.
                    foreach (int e in Character.Alchemy.Elementlist)
                    {
                        if (e != 0)
                        {
                            //TODO: Make detailed randoms
                            //Make random add count for the elements
                            //NOTE: Check what item has what element on destruction. if pk2 contains or not.
                            int elementamount = 0;

                            #region Amounts
                            if (ObjData.Manager.ItemBase[item.ID].Degree == 1)
                                elementamount = Rnd.Next(1, 60);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 2)
                                elementamount = Rnd.Next(1, 90);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 3)
                                elementamount = Rnd.Next(1, 120);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 4)
                                elementamount = Rnd.Next(1, 150);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 5)
                                elementamount = Rnd.Next(1, 200);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 6)
                                elementamount = Rnd.Next(1, 250);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 7)
                                elementamount = Rnd.Next(1, 300);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 8)
                                elementamount = Rnd.Next(1, 375);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 9)
                                elementamount = Rnd.Next(1, 450);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 10)
                                elementamount = Rnd.Next(1, 600);
                            else if (ObjData.Manager.ItemBase[item.ID].Degree == 11)
                                elementamount = Rnd.Next(1, 800);
                            #endregion

                            int stoneamount = 0;

                            #region Stones
                            if (ObjData.Manager.ItemBlue[item.dbID].totalblue != 0)
                            {
                                if (ObjData.Manager.ItemBlue[item.dbID].totalblue == 1)
                                    stoneamount = Rnd.Next(0, 1);
                                else if (ObjData.Manager.ItemBlue[item.dbID].totalblue == 2)
                                    stoneamount = Rnd.Next(0, 2);
                            }
                            #endregion

                            slotcheck = GetFreeSlot();
                            //Stack items todo
                            AddItem(ObjData.Manager.ItemBase[e].ID, 10, slotcheck, Character.Information.CharacterID, 0);
                            client.Send(Packet.GainElements(slotcheck, ObjData.Manager.ItemBase[e].ID, (short)elementamount));
                        }
                    }
                    //Clear created list content.
                    Character.Alchemy.Elementlist.Clear();
                }
                //Reset bool
                Character.Alchemy.working = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy error destroyer {0}", ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get matching degree elements for item
        /////////////////////////////////////////////////////////////////////////////////
        public List<int> GetDegreeElements(int itemid, WorldMgr.character c)
        {
            try
            {
                List<int> elements = new List<int>();
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.ELEMENTS && ObjData.Manager.ItemBase[i].Degree == ObjData.Manager.ItemBase[itemid].Degree)
                        {
                            if (i != 0)
                                elements.Add(i);
                        }
                    }
                }
                return elements;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get matching degree stones for item
        /////////////////////////////////////////////////////////////////////////////////
        public List<int> GetStonesDegree(int itemid, WorldMgr.character c)
        {
            try
            {
                List<int> stones = new List<int>();
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.STONES && ObjData.Manager.ItemBase[i].Degree == ObjData.Manager.ItemBase[itemid].Degree)
                        {
                            if (i != 0)
                                stones.Add(i);
                        }
                    }
                }
                return stones;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get stone from tablet information
        /////////////////////////////////////////////////////////////////////////////////
        public int GetStoneFromTablet(int itemid)
        {
            try
            {
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Name == ObjData.Manager.ItemBase[itemid].StoneName)
                            return ObjData.Manager.ItemBase[i].ID;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }
    }
}
