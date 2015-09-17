using System;
using System.Linq;
using System.Data.SqlClient;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        void StopTimers()
        {
            try
            {
                if (Timer.Attack != null) StopAttackTimer();
                if (Timer.Casting != null) return;
                if (Timer.Berserker != null) StopBerserkTimer();
                if (Timer.Pvp != null) StopPvpTimer();
                if (Timer.Scroll != null) return;
                if (Timer.Sitting != null || HPRegen != null || MPRegen != null)
                {
                    StopMPRegen();
                    StopHPRegen();
                }
                if (Timer.SkillCasting != null) StopSkillTimer();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        //###########################################################################################
        // Start teleporting
        //###########################################################################################
        public void Teleport_Start()
        {
            try
            {
                lock (this)
                {
                    //Checks before we continue
                    if (Character.Action.PickUping) return;
                    //Timer checks
                    StopTimers();
                    //Open the packet reader
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    //Teleport id
                    int teleportidinfo = Reader.Int32();
                    //Number
                    byte number = Reader.Byte();
                    //Teleport selected
                    int teleportselect = Reader.Int32();
                    Reader.Close();
                    //Find price information
                    int price = ObjData.Manager.TeleportPrice.Find(pc => (pc.ID == number)).price;
                    //If the user has less gold then it costs
                    if (Character.Information.Gold < price)
                    {
                        //Send error message
                        client.Send(Packet.Message(OperationCode.SERVER_TELEPORTSTART, Messages.UIIT_MSG_INTERACTION_FAIL_NOT_ENOUGH_MONEY));
                        return;
                    }
                    //If the user level is lower then the required level                   
                    if (ObjData.Manager.TeleportPrice.Find(dd => (dd.ID == teleportselect)).level > 0 && Character.Information.Level < ObjData.Manager.TeleportPrice.Find(dd => (dd.ID == teleportselect)).level)
                    {
                        client.Send(Packet.Message(OperationCode.SERVER_TELEPORTSTART, Messages.UIIT_MSG_INTERACTION_FAIL_OUT_OF_REQUIRED_LEVEL_FOR_TELEPORT));
                        return;
                    }
                    //If the user is currently with job transport (TODO).

                    //Update players gold
                    Character.Information.Gold -= price;
                    //Update players gold in database
                    SaveGold();
                    //Close buffs
                    BuffAllClose();
                    //Send teleport packet #1
                    client.Send(Packet.TeleportStart());
                    //Set state
                    Character.InGame = false;
                    //Update location
                    Teleport_UpdateXYZ(Convert.ToByte(teleportselect));
                    //Despawn objects
                    ObjectDeSpawnCheck();
                    //Despawn player to other players
                    DeSpawnMe();
                    //Required
                    client.Send(Packet.TeleportStart2());
                    //Send loading screen image
                    client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[Convert.ToByte(teleportselect)].xSec, ObjData.Manager.PointBase[Convert.ToByte(teleportselect)].ySec));
                    //Set bool
                    Character.Teleport = true;
                    
                    
                    
                    /*
                    List<ObjData.slotItem> EquipedItems = new List<ObjData.slotItem>();
                    for (byte q = 0; q < 8; q++)
                    {
                        EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                    }
                    for (byte qp = 9; qp < 14; qp++)
                    {
                        EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, qp, 0));
                    }
                    //Needs fixing will check it later.
                    /*foreach (ObjData.slotItem sitem in EquipedItems)
                    {
                        if (ObjData.Manager.ItemBlue[sitem.dbID].totalblue > 0)
                        {
                            LoadBluesid(sitem.dbID);
                            AddRemoveBlues(this, sitem, false);
                        }
                    }*/
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Teleport select error {0}", ex);
                Log.Exception(ex);
            }
        }

        //###########################################################################################
        // Data
        //###########################################################################################
        public void Teleport_Data()
        {
            try
            {
                if (Character.Teleport)
                {
                    StopBerserkTimer();
                    CheckCharStats(Character);
                    client.Send(Packet.StartPlayerLoad());
                    client.Send(Packet.CharacterDataLoad(Character));

                    client.Send(Packet.EndPlayerLoad());
                    client.Send(Packet.PlayerUnknowPack(Character.Information.UniqueID));
                    client.Send(Packet.UnknownPacket());
                    SavePlayerPosition();


                    if (Character.Action.MonsterID.Count > 0)
                    {
                        Character.Action.MonsterID.Clear();
                    }
                    if (Character.Transport.Right)
                    {
                        WorldMgr.pet_obj o = Character.Transport.Horse;
                        Character.Transport.Spawned = true;
                        Character.Transport.Horse.Information = true;
                        Send(Packet.Player_UpToHorse(Character.Information.UniqueID, true, o.UniqueID));
                    }
                    if (Character.Attackpet.Active)
                    {
                        WorldMgr.pet_obj o = Character.Attackpet.Details;
                        //ObjData.slotItem item =
                        //client.Send(Packet.Pet_Information_grab(o, slot));
                    }
                    if (Character.Grabpet.Active)
                    {
                        //WorldMgr.pet_obj o = Character.Grabpet.Details;
                        //client.Send(Packet.Pet_Information_grab(o, slot));
                    }
                    ObjectSpawnCheck();
                    Character.Teleport = false;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Console.WriteLine(ex);
            }
        }
        //###########################################################################################
        // Update x y z
        //###########################################################################################
        void Teleport_UpdateXYZ(byte number)
        {

            Character.Position.xSec = ObjData.Manager.PointBase[number].xSec;
            Character.Position.ySec = ObjData.Manager.PointBase[number].ySec;
            Character.Position.x = (float)ObjData.Manager.PointBase[number].x;
            Character.Position.z = (float)ObjData.Manager.PointBase[number].z;
            Character.Position.y = (float)ObjData.Manager.PointBase[number].y;

            if (Character.Transport.Right)
            {
                Character.Transport.Horse.xSec = ObjData.Manager.PointBase[number].xSec;
                Character.Transport.Horse.ySec = ObjData.Manager.PointBase[number].ySec;
                Character.Transport.Horse.x = (float)ObjData.Manager.PointBase[number].x;
                Character.Transport.Horse.z = (float)ObjData.Manager.PointBase[number].z;
                Character.Transport.Horse.y = (float)ObjData.Manager.PointBase[number].y;
            }

            if (Character.Grabpet.Active)
            {
                Character.Grabpet.Details.xSec = ObjData.Manager.PointBase[number].xSec;
                Character.Grabpet.Details.ySec = ObjData.Manager.PointBase[number].ySec;
                Character.Grabpet.Details.x = (float)ObjData.Manager.PointBase[number].x + Rnd.Next(1, 3);
                Character.Grabpet.Details.z = (float)ObjData.Manager.PointBase[number].z;
                Character.Grabpet.Details.y = (float)ObjData.Manager.PointBase[number].y + Rnd.Next(1, 3);
            }

            if (Character.Attackpet.Active)
            {
                Character.Attackpet.Details.xSec = ObjData.Manager.PointBase[number].xSec;
                Character.Attackpet.Details.ySec = ObjData.Manager.PointBase[number].ySec;
                Character.Attackpet.Details.x = (float)ObjData.Manager.PointBase[number].x + Rnd.Next(1, 3);
                Character.Attackpet.Details.z = (float)ObjData.Manager.PointBase[number].z;
                Character.Attackpet.Details.y = (float)ObjData.Manager.PointBase[number].y + Rnd.Next(1, 3);
            }
            //return BitConverter.ToInt16(new byte[2] { ObjData.Manager.PointBase[number].xSec, ObjData.Manager.PointBase[number].ySec }, 0);
        }
        //###########################################################################################
        // Cave teleports
        //###########################################################################################
        void TeleportCave(int number)
        {
            try// Changed to cavePointbase for the telepad locations
            {
                BuffAllClose();

                DeSpawnMe();
                ObjectDeSpawnCheck();
                client.Send(Packet.TeleportOtherStart());

                Character.Position.xSec = ObjData.Manager.cavePointBase[number].xSec;
                Character.Position.ySec = ObjData.Manager.cavePointBase[number].ySec;
                Character.Position.x = (float)ObjData.Manager.cavePointBase[number].x;
                Character.Position.z = (float)ObjData.Manager.cavePointBase[number].z;
                Character.Position.y = (float)ObjData.Manager.cavePointBase[number].y;
                Character.InGame = false;
                Character.Teleport = true;

                client.Send(Packet.TeleportImage(ObjData.Manager.cavePointBase[number].xSec, ObjData.Manager.cavePointBase[number].ySec));
                Character.Teleport = true;
                Timer.Scroll.Dispose();
                Timer.Scroll = null;
                Character.Information.Scroll = false;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        //###########################################################################################
        // Reverse scrolls
        //###########################################################################################
        void HandleReverse(int itemid ,byte select, int objectlocationid)
        {
            try
            {
                //Remove loc // add loc to case 7
                if (Character.Position.Walking) return;
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;
                Character.Information.Scroll = true;
                switch (select)
                {
                    //Move to return point
                    case 2:
                        Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));                        
                        StartScrollTimer(1000);
                        SavePlayerReturn();
                        break;
                    //Move to last recall point
                    case 3:
                        DB ms = new DB("SELECT * FROM character_rev WHERE charname='" + Character.Information.Name + "'");
                        using (SqlDataReader reader = ms.Read())
                        {
                            while (reader.Read())
                            {
                                byte xSec = reader.GetByte(2);
                                byte ySec = reader.GetByte(3);
                                float x = reader.GetInt32(4);
                                float z = reader.GetInt32(6);
                                float y = reader.GetInt32(5);

                                BuffAllClose();
                                DeSpawnMe();
                                ObjectDeSpawnCheck();
                                client.Send(Packet.TeleportOtherStart());

                                Character.Position.xSec = xSec;
                                Character.Position.ySec = ySec;
                                Character.Position.x = x;
                                Character.Position.z = z;
                                Character.Position.y = y;

                                UpdateXY();

                                client.Send(Packet.TeleportImage(xSec, ySec));
                                Character.Teleport = true;
                                Timer.Scroll = null;
                                Character.Information.Scroll = false;
                            }
                        }
                        ms.Close();
                        break;
                    //Teleport to map location
                    case 7:
                        BuffAllClose();
                        DeSpawnMe();
                        ObjectDeSpawnCheck();
                        client.Send(Packet.TeleportOtherStart());
                        //TODO: Check formula coords are a bit off
                        Character.Position.xSec = ObjData.Manager.ReverseData[objectlocationid].xSec;
                        Character.Position.ySec = ObjData.Manager.ReverseData[objectlocationid].ySec;
                        Character.Position.x = (float)ObjData.Manager.ReverseData[objectlocationid].x;
                        Character.Position.z = (float)ObjData.Manager.ReverseData[objectlocationid].z;
                        Character.Position.y = (float)ObjData.Manager.ReverseData[objectlocationid].y;

                        UpdateXY();

                        client.Send(Packet.TeleportImage(ObjData.Manager.ReverseData[objectlocationid].xSec, ObjData.Manager.ReverseData[objectlocationid].ySec));
                        Character.Teleport = true;
                        Timer.Scroll = null;
                        Character.Information.Scroll = false;
                        break;
                }
            }

            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        //###########################################################################################
        // Thief scrolls
        //###########################################################################################
        void HandleThiefScroll(int ItemID)
        {
            try
            {
                //TODO: Add check if user is wearing thief suit.
                if (Character.Position.Walking) return;
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;

                BuffAllClose();

                DeSpawnMe();
                ObjectDeSpawnCheck();
                client.Send(Packet.TeleportOtherStart());

                byte xSec = 182;
                byte ySec = 96;
                float x = 9119;
                float z = 3;
                float y = 890;

                Character.Position.xSec = xSec;
                Character.Position.ySec = ySec;
                Character.Position.x = x;
                Character.Position.z = z;
                Character.Position.y = y;

                client.Send(Packet.TeleportImage(xSec, ySec));
                Character.Teleport = true;
                Timer.Scroll.Dispose();
                Timer.Scroll = null;
                Character.Information.Scroll = false;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        //###########################################################################################
        // Normal return scrolls
        //###########################################################################################
        void HandleReturnScroll(int ItemID)
        {
            try
            {
                if (Character.Position.Walking) return;
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;
                Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                Character.Information.Scroll = true;
                StartScrollTimer(ObjData.Manager.ItemBase[ItemID].Use_Time);
                SavePlayerReturn();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
