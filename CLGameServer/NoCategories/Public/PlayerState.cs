using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Death
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SitDown()
        {
            if (Character.Stall.Stallactive || Character.Transport.Right || Character.State.Busy || Character.Position.Walking)
            {
                return;
            }
            if (Character.State.Standing == false)
            {
                Character.State.Standing = true;
                StartSitDownTimer();
                if (Character.State.Sitting == false)
                {
                    // eğer karakter yürüyorsa durdur ve oturt
                    if (Character.Position.Walking)
                    {
                        if (Timer.Movement != null)
                        {
                            Timer.Movement.Dispose();
                        }
                        Send(Packet.StatePack(Character.Information.UniqueID, 0, 1, false));
                    }
                    Send(Character.Spawn, Packet.ChangeStatus(Character.Information.UniqueID, 4, 4));
                    HPregen(1100);
                    MPregen(1500);
                    Character.State.Sitting = true;
                }
                else
                {
                    Send(Character.Spawn, Packet.ChangeStatus(Character.Information.UniqueID, 4, 0));
                    StopMPRegen();
                    StopHPRegen();
                    Character.State.Sitting = false;
                }
            }

        }
        public void Player_Up()
        {
            try
            {
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                reader.Close();
                /////////////////////////////////////////////////////////////////////////////////////
                // Normal Death / Return to return point
                /////////////////////////////////////////////////////////////////////////////////////
                switch (type)
                {
                    case 1:
                        //Normal death
                        if (Character.State.Die)
                        {
                            //Check if character is walking should not be happening
                            if (Character.Position.Walking) return;
                            //Save information for reverse scrolls
                            SavePlayerReturn();
                            //Close buffs
                            BuffAllClose();
                            //Set bool ingame
                            Character.InGame = false;
                            //Despawn
                            DeSpawnMe();
                            //Despawn objects
                            ObjectDeSpawnCheck();
                            //Send teleport packet
                            client.Send(Packet.TeleportOtherStart());
                            //Update location
                            Teleport_UpdateXYZ(Character.Information.Place);
                            //Set hp to max hp / 2
                            Character.Stat.SecondHp = Character.Stat.Hp / 2;
                            //Send teleport image
                            client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[Character.Information.Place].xSec, ObjData.Manager.PointBase[Character.Information.Place].ySec));
                            //Set bools
                            Character.Teleport = true;
                            Character.State.Die = false;
                        }
                        break;
                    case 2:
                        //Ressurect at same location
                        if (Character.State.Die)
                        {
                            //Check if character is walking should not be happening
                            if (Character.Position.Walking) return;
                            //Check level of character (not sure if lvl 10 is retail info of ressurect).
                            if (Character.Information.Level > 10) return;
                            //Stop berserk timer if it were active
                            StopBerserkTimer();
                            //Start sending packets for teleport
                            client.Send(Packet.TeleportOtherStart());
                            //Reset state information
                            Send(Packet.StatePack(Character.Information.UniqueID, 0, 1, false));
                            //Set the character hp to max hp / 2
                            Character.Stat.SecondHp = Character.Stat.Hp / 2;
                            //Character.State.SafeState = true;
                            //NotAttackableTimer(5000);
                            UpdateHp();
                            //Send packet for updating hp
                            client.Send(Packet.UpdatePlayer(Character.Information.UniqueID, 0x20, 1, Character.Stat.SecondHp));
                            //Set bool
                            Character.State.Die = false;
                            //Send state pack
                            Send(Packet.StatePack(Character.Information.UniqueID, 4, 0, false));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Death error {0}", ex);
                Log.Exception(ex);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Question Mark
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void QuestionMark()
        {
            Character.Information.AutoInverstExp = PacketInformation.buffer[0];
            Send(Packet.QuestionMark(Character.Information.UniqueID, Character.Information.AutoInverstExp));
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Leaving Game
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LeaveGame()
        {
            try
            {

                Character.Information.Quit = true;
                client.Send(Packet.StartingLeaveGame(5, PacketInformation.buffer[0]));
                StartWaitingTimer(5000);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Cancel Leave Game
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CancelLeaveGame()
        {
            try
            {
                client.Send(Packet.CancelLeaveGame());
                Character.Information.Quit = false;
                Timer.Logout.Dispose();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Disconnect
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void KickPlayer(PlayerMgr Target)
        {

            try
            {
                Target.client.Send((Packet.ChatPacket(7, Target.Character.Information.UniqueID, "You Have been kicked!", "")));
                if (Player != null)
                {
                    DB.query("UPDATE users SET online='" + 0 + "' WHERE id='" + Target.Player.AccountName + "'");
                    Target.Player.Dispose();
                    Target.Player = null;
                }
                if (Target.Character != null)
                {
                    if (Target.Character.Transport.Right) Target.Character.Transport.Horse.DeSpawnMe();
                    if (Target.Character.Grabpet.Active) Target.Character.Grabpet.Details.DeSpawnMe();
                    if (Target.Character.Network.Exchange.Window) Target.Exchange_Close();
                    if (Target.Character.State.Sitting)
                    {
                        Target.StopMPRegen();
                        Target.StopHPRegen();
                    }
                    Target.StopAttackTimer();
                    Target.BuffAllClose();
                    Target.DeSpawnMe();
                    Target.StopMPRegen();
                    Target.StopHPRegen();
                    Target.SavePlayerPosition();
                    Target.SavePlayerInfo();
                    Target.Character.InGame = false;

                    client.Disconnect(Target.client.clientSocket);
                    Target.client.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Disconnect(string type)
        {
            try
            {
                /*Disconnect information:
                 *mormal = normal disconnect
                 *ban = ban user for hack attempt*/

                if (type == "normal")
                {
                    #region Normal Disconnect
                    
                    if (Character != null)
                    {
                        if (Character.Transport.Right) Character.Transport.Horse.DeSpawnMe();
                        if (Character.Grabpet.Active) Character.Grabpet.Details.DeSpawnMe();
                        if (Character.Network.Exchange.Window) Exchange_Close();
                        if (Character.State.Sitting || HPRegen != null || MPRegen != null)
                        {
                            StopMPRegen();
                            StopHPRegen();
                        }
                        if (!Character.State.Sendonce)
                        {
                            Character.State.Sendonce = true;
                            LoadBlues(Character);
                            #region equipped items
                            List<ObjData.slotItem> EquipedItems = new List<ObjData.slotItem>();
                            //For each equiped item add
                            for (byte q = 0; q < 13; q++)
                            {
                                EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                            }
                            /*
                            //Load default values for resistance
                            #region Load Resistance default values
                            Character.Blues.Luck = 100;
                            Character.Blues.Resist_All = 100;
                            Character.Blues.Resist_Burn = 100;
                            Character.Blues.Resist_CSMP = 100;
                            Character.Blues.Resist_Disease = 100;
                            Character.Blues.Resist_Eshock = 100;
                            Character.Blues.Resist_Fear = 100;
                            Character.Blues.Resist_Frostbite = 100;
                            Character.Blues.Resist_Poison = 100;
                            Character.Blues.Resist_Sleep = 100;
                            Character.Blues.Resist_Stun = 100;
                            Character.Blues.Resist_Zombie = 100;
                            Character.Blues.MonsterIgnorance = 100;
                            Character.Blues.UniqueDMGInc = 100;
                            #endregion
                            */
                            //Load blues for each item
                            foreach (ObjData.slotItem sitem in EquipedItems)
                            {
                                if (ObjData.Manager.ItemBlue.ContainsKey(sitem.dbID))
                                {
                                    LoadBluesid(sitem.dbID);
                                    if (ObjData.Manager.ItemBlue[sitem.dbID].totalblue != 0)
                                        AddRemoveBlues(this, sitem, false);
                                }
                            }
                            #endregion
                        }
                        StopAttackTimer();
                        BuffAllClose();
                        DeSpawnMe();
                        StopMPRegen();
                        StopHPRegen();
                        SavePlayerPosition();
                        SavePlayerInfo();
                        Character.InGame = false;
                    }
                    if (Player != null)
                    {
                        DB.query("UPDATE users SET online='" + 0 + "' WHERE id='" + Player.AccountName + "'");
                        Player.Dispose();
                        Player = null;
                    }
                    client.Disconnect(client.clientSocket);
                    client.Close();
                    PrintLastPack();

                    Helpers.Manager.clients.Remove(this);
                    
                    if (Character == null)
                    {
                        Console.WriteLine("[Client] Has Disconnected [Online Players: {0}]", Helpers.Manager.GetOnlineClientCount);  
                    }
                    #endregion
                }
                if (type == "ban")
                {
                    #region Ban disconnect user
                    
                    if (Character != null)
                    {
                        if (Character.Transport.Right) Character.Transport.Horse.DeSpawnMe();
                        if (Character.Grabpet.Active) Character.Grabpet.Details.DeSpawnMe();
                        if (Character.Network.Exchange.Window) Exchange_Close();
                        if (Character.State.Sitting)
                        {
                            StopMPRegen();
                            StopHPRegen();
                        }
                        if (!Character.State.Sendonce)
                        {
                            Character.State.Sendonce = true;
                            //Load blue data
                            LoadBlues(Character);

                            #region equipped items
                            List<ObjData.slotItem> EquipedItems = new List<ObjData.slotItem>();
                            //For each equiped item add
                            for (byte q = 0; q < 13; q++)
                            {
                                EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                            }
                            /*
                            //Load default values for resistance
                            #region Load Resistance default values
                            Character.Blues.Luck = 100;
                            Character.Blues.Resist_All = 100;
                            Character.Blues.Resist_Burn = 100;
                            Character.Blues.Resist_CSMP = 100;
                            Character.Blues.Resist_Disease = 100;
                            Character.Blues.Resist_Eshock = 100;
                            Character.Blues.Resist_Fear = 100;
                            Character.Blues.Resist_Frostbite = 100;
                            Character.Blues.Resist_Poison = 100;
                            Character.Blues.Resist_Sleep = 100;
                            Character.Blues.Resist_Stun = 100;
                            Character.Blues.Resist_Zombie = 100;
                            Character.Blues.MonsterIgnorance = 100;
                            Character.Blues.UniqueDMGInc = 100;
                            #endregion
                            */
                            //Load blues for each item
                            foreach (ObjData.slotItem sitem in EquipedItems)
                            {
                                if (ObjData.Manager.ItemBlue.ContainsKey(sitem.dbID))
                                {
                                    LoadBluesid(sitem.dbID);
                                    if (ObjData.Manager.ItemBlue[sitem.dbID].totalblue != 0)
                                        AddRemoveBlues(this, sitem, false);
                                }
                            }
                            #endregion
                        }
                        StopAttackTimer();
                        BuffAllClose();
                        DeSpawnMe();
                        StopMPRegen();
                        StopHPRegen();
                        SavePlayerPosition();
                        SavePlayerInfo();
                        Character.Dispose();
                        Character.InGame = false;
                        
                    }
                    if (Player != null)
                    {
                        DB.query("UPDATE users SET online='" + 0 + "',ban='1',banreason='Banned for hacking.' WHERE id='" + Player.AccountName + "'");
                        Player.Dispose();
                        Player = null;
                    }
                    client.Disconnect(client.clientSocket);
                    client.Close();
                    Helpers.Manager.clients.Remove(this);
                    #endregion
                }

                if (Character != null) 
                    Character.Dispose();
                if (this != null)
                    Dispose();
            }    
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void PrintLastPack()
        {
            try
            {
                if (client.BuffList != null && Character != null)
                {
                    StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\PlayerData\Log\" + Character.Information.CharacterID + ".ClientErrorLog");
                    foreach (byte[] i in client.BuffList)
                    {
                        Writer.WriteLine("[{0}]: -------------------------------------------------", DateTime.Now.ToString("yyyy.MM.dd H:mm"));
                        Writer.WriteLine("Last packet:");
                        Writer.WriteLine(client.BytesToString(i));
                        Writer.WriteLine("---- end ----------------------------------------------");
                    }
                    Writer.Close();
                }
            }
            catch
            {
                //Console.WriteLine("debugWriteBug()::{0}", Character.Information.Name);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Emotes
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Emote()
        {
            client.Send(Packet.Player_Emote(Character.Information.UniqueID, PacketInformation.buffer[0]));
            Send(Packet.Player_Emote(Character.Information.UniqueID, PacketInformation.buffer[0]));
        }
        protected void Pet_SetNewSpeed()
        {
            Character.Grabpet.Details.Run = Character.Speed.RunSpeed;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Actions / Sit / Stand / Walk etc
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Doaction()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                List<int> toall = Character.Spawn;
                byte type = Reader.Byte();

                switch (type)
                {
                    case 2:
                        if (Character.Transport.Right)
                        {
                            Send(toall, Packet.SetSpeed(Character.Transport.Horse.UniqueID, Character.Transport.Horse.Speed1, Character.Transport.Horse.Speed2));
                            Send(toall, Packet.ChangeStatus(Character.Transport.Horse.UniqueID, type, 0));
                        }
                        Send(toall, Packet.ChangeStatus(Character.Information.UniqueID, type, 0));
                        break;
                    case 3:
                        if (Character.Transport.Right)
                        {
                            Send(toall, Packet.SetSpeed(Character.Transport.Horse.UniqueID, Character.Transport.Horse.Speed1, Character.Transport.Horse.Speed2));
                            Send(toall, Packet.ChangeStatus(Character.Transport.Horse.UniqueID, type, 0));
                        }
                        Send(toall, Packet.ChangeStatus(Character.Information.UniqueID, type, 0));
                        break;
                    case 4:
                        SitDown();
                        break;
                    default:
                        Send(toall, Packet.ChangeStatus(Character.Information.UniqueID, type, 0));
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player Change Hp / Mp
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetHpUp(int amount)
        {
            //Setup +

            Character.Stat.SecondHp = Character.Stat.SecondHp + amount;
            UpdateHp();

        }
        void SetMpUp(int amount)
        {
            //Setup +
            Character.Stat.SecondMP = Character.Stat.SecondMP + amount;
            UpdateMp();
        }
        public void DbUpdateStats()
        {
            try
            {
                DB.query("UPDATE character SET strength='" + Character.Stat.Strength + "' , intelligence='" + Character.Stat.Intelligence + "' , hp='" + Character.Stat.Hp + "' , mp='" + Character.Stat.Mp + "' WHERE name='" + Character.Information.Name + "'");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void UpdateStrengthMinus(sbyte amount)
        {
            Character.Stat.MinPhyAttack -= (0.45 * amount);
            Character.Stat.MaxPhyAttack -= (0.65 * amount);
            Character.Stat.PhyDef -= (0.40 * amount);
        }
        public void UpdateIntelligenceMinus(sbyte amount)
        {
            Character.Stat.MinMagAttack -= (0.45 * amount);
            Character.Stat.MaxMagAttack -= (0.65 * amount);
            Character.Stat.MagDef -= (0.40 * amount);
        }
    }
}
