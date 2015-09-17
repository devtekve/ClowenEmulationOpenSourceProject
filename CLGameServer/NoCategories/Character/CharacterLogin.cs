using CLGameServer.Client;
using CLFramework;
using System;
using System.Collections.Generic;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        #region Character Screen
        public void CharacterScreen()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte type = Reader.Byte();
                //Switch on byte type
                switch (type)
                {
                    case 1:
                        //WorldMgr.character creation
                        CharacterCreate();
                        break;
                    case 2:
                        //Character listening
                        CharacterListing();
                        break;
                    case 3:
                        //Character deletion
                        CharacterDelete();
                        break;
                    case 4:
                        //WorldMgr.character checking
                        CharacterCheck(PacketInformation.buffer);
                        break;
                    case 5:
                        //Character restoring
                        CharacterRestore();
                        break;
                    case 9:
                        //Character job information
                        CharacterJobInfo();
                        break;
                    case 16:
                        //Select job
                        CharacterJobPick(PacketInformation.buffer);
                        break;
                    default:
                        //We use this if we get a new case.
                        Console.WriteLine("Character Screen Type: " + type);
                        Disconnect("normal");
                        break;
                }
                //Close packet reader
                Reader.Close();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Send Load Packets
        public void Patch()
        {
            //Wrap our function inside a catcher
            try
            {
                //Send packets (Needs to be more specified).
                client.Send(Packet.AgentServer());
                client.Send(Packet.LoadGame_1());
                client.Send(Packet.LoadGame_2());
                client.Send(Packet.LoadGame_3());
                client.Send(Packet.LoadGame_4());
                client.Send(Packet.LoadGame_5());
                client.Send(Packet.LoadGame_6());
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Login Main
        public void LoginScreen()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                string name = Reader.Text();
                Reader.Close();
                //Anti hack checking sql query
                DB ms = new DB("SELECT name FROM character WHERE account='" + Player.AccountName + "' AND name='" + name + "'");
                //Check if the player account and character belongs together (count row).
                int checkinfo = ms.Count();
                //If there's no result
                if (checkinfo == 0)
                {
                    //Disconnect the user if hack attempt
                    client.Disconnect(client.clientSocket);
                    return;
                }
                //If there's a result we continue loading
                else
                {

                    //Create new character definition details
                    Character = new WorldMgr.character();
                    //Set character name
                    Character.Information.Name = name;
                    //Set player id
                    Character.Account.ID = Player.ID;
                    //Load player data
                    PlayerDataLoad();
                    Character.Ids = new GenerateUniqueID(Character.Information.CharacterID, GenerateUniqueID.IDS.Player);
                    Character.Information.UniqueID = Character.Ids.GetUniqueID;
                        
                    //Load job data
                    LoadJobData();
                    //Check same character
                    checkSameChar(name, Character.Information.UniqueID);
                    //Check character stats
                    CheckCharStats(Character);
                    //Add new cient
                    lock (Helpers.Manager.clients)
                    {
                        Helpers.Manager.clients.Add(this);
                    }
                    //Send packets required
                    client.Send(Packet.LoginScreen());
                    client.Send(Packet.StartPlayerLoad());
                    client.Send(Packet.CharacterDataLoad(Character));
                    client.Send(Packet.EndPlayerLoad());
                    client.Send(Packet.PlayerUnknowPack(Character.Information.UniqueID));
                    //client.Send(Packet.UnknownPacket()); //need to research this packet.
                    //Update online status
                    DB.query("UPDATE character SET online='1' WHERE id='" + Character.Information.CharacterID + "'");
                    //Update server information
                    Brain.ServerMgr.UpdateServerInfo();
                    //Open our timer
                    OpenTimer();
                    Character.InitializeCharacter();
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
                                AddRemoveBlues(this, sitem, true);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Check for same character
        void checkSameChar(string name, int id)
        {
            //Wrap our function inside a catcher
            try
            {
                //Lock the client
                lock (Helpers.Manager.clients)
                {
                    //For each client currently connected
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        //If the client is null, or the client character name is same as our login name
                        if (Helpers.Manager.clients[i] != null && Helpers.Manager.clients[i].Character.Information.Name == name || Helpers.Manager.clients[i].Character.Information.UniqueID == id)
                        {
                            //Disconnect the user
                            Helpers.Manager.clients[i].Disconnect("normal");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Ingame success
        public void InGameSuccess()
        {
            //Wrap our function inside a catcher
            try
            {
                //If character isnt ingame yet
                if (!Character.InGame)
                {
                    //Load premium ticket
                    LoadTicket(Character);
                    //Load guild information
                    LoadPlayerGuildInfo(true);
                    //Load spawns
                    ObjectSpawnCheck();
                    //Send stats packet
                    client.Send(Packet.PlayerStat(Character));
                    //Send player state packet
                    client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x02, false));
                    //Send complete load packet
                    client.Send(Packet.Completeload());
                    //Send player silk packet
                    client.Send(Packet.Silk(Player.Silk, Player.SilkPrem));
                    //Load friends
                    GetFriendsList();
                    //Set player online
                    DB.query("UPDATE character SET online='1' WHERE id='" + Character.Information.CharacterID + "'");
                    Character.InGame = true;
                    //Load guild data
                    GetGuildData();
                    //Update hp and mp
                    UpdateHp();
                    UpdateMp();
                    
                    //Load transport
                    LoadTransport();
                    //Load grabpet if active
                    //LoadGrabPet();
                    //Set safe state
                    Character.State.SafeState = false;
                    NotAttackableTimer(5000);
                    //Regen
                    HPregen(4000);
                    MPregen(4000);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
    }
}
