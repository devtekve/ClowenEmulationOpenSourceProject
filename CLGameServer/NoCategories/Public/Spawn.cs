using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Spawn system
        /////////////////////////////////////////////////////////////////////////////////       
        public void ObjectSpawnCheck()
        {
            try
            {
                if (Character.deSpawning) return;
                int spawnrange = 100;
                lock (this)
                {
                    //Make sure character info is not null or not spawning yet allready.
                    if (Character != null && !Character.Spawning)
                    {
                        //Set spawn state to true so cannot be doubled
                        Character.Spawning = true;

                        //Repeat for each client ingame
                        #region Clients
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            //Get defined information for the client
                            PlayerMgr playerspawn = Helpers.Manager.clients[i];
                            //Make sure that the spawning case is not ourselfs, or not spawned yet and not null
                            if (playerspawn != null && playerspawn != this && !Character.Spawned(playerspawn.Character.Information.UniqueID) && playerspawn.Character.Information.Name != Character.Information.Name)
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the player
                                if (playerspawn.Character.Position.x >= (Character.Position.x - 50) && playerspawn.Character.Position.x <= ((Character.Position.x - 50) + spawnrange) && playerspawn.Character.Position.y >= (Character.Position.y - 50) && playerspawn.Character.Position.y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                    //Make sure the unique id is not null
                                    if (playerspawn.Character.Information.UniqueID != 0)
                                    {
                                        Character.Spawn.Add(playerspawn.Character.Information.UniqueID);
                                        client.Send(Packet.ObjectSpawn(playerspawn.Character));
                                    }
                                    //Spawn ourselfs to the other players currently in spawn range.
                                    ObjectPlayerSpawn(playerspawn);
                                }
                            }
                        }
                        #endregion
                        //Repeat for each helper object
                        #region Helper objects
                        for (int i = 0; i < Helpers.Manager.HelperObject.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Helpers.Manager.HelperObject[i] != null && !Helpers.Manager.HelperObject[i].Spawned(Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (Character.Position.x >= (Helpers.Manager.HelperObject[i].x - 50) && Character.Position.x <= ((Helpers.Manager.HelperObject[i].x - 50) + spawnrange) && Character.Position.y >= (Helpers.Manager.HelperObject[i].y - 50) && Character.Position.y <= ((Helpers.Manager.HelperObject[i].y - 50) + spawnrange))
                                {
                                    if (Helpers.Manager.HelperObject[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Helpers.Manager.HelperObject[i].Spawn.Add(Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Helpers.Manager.HelperObject[i]));
                                    }
                                }
                            }
                        }
                        #endregion
                        /*
                        #region Special objects
                        for (int i = 0; i < Helpers.Manager.SpecialObjects.Count; i++)
                        {
                            //If the special object is not null , or not spawned for us yet and the unique id is not null
                            if (Helpers.Manager.SpecialObjects[i] != null && !Helpers.Manager.SpecialObjects[i].Spawned(Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (Character.Position.x >= (Helpers.Manager.SpecialObjects[i].x - 50) && Character.Position.x <= ((Helpers.Manager.SpecialObjects[i].x - 50) + spawnrange) && Character.Position.y >= (Helpers.Manager.SpecialObjects[i].y - 50) && Character.Position.y <= ((Helpers.Manager.SpecialObjects[i].y - 50) + spawnrange))
                                {
                                    if (Helpers.Manager.SpecialObjects[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Helpers.Manager.SpecialObjects[i].Spawn.Add(Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Helpers.Manager.SpecialObjects[i]));
                                        //Console.WriteLine("Spawning {0}", ObjData.Manager.ObjectBase[Helpers.Manager.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                        #endregion
                         */
                        //Repeat for each object
                        #region Objects
                        for (int i = 0; i < Helpers.Manager.Objects.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Helpers.Manager.Objects[i] != null && !Helpers.Manager.Objects[i].Spawned(Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (Character.Position.x >= (Helpers.Manager.Objects[i].x - 50) && Character.Position.x <= ((Helpers.Manager.Objects[i].x - 50) + spawnrange) && Character.Position.y >= (Helpers.Manager.Objects[i].y - 50) && Character.Position.y <= ((Helpers.Manager.Objects[i].y - 50) + spawnrange))
                                {
                                    if (Helpers.Manager.Objects[i].UniqueID != 0 && !Helpers.Manager.Objects[i].Die)
                                    {
                                        //Add our spawn
                                        Helpers.Manager.Objects[i].Spawn.Add(Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Helpers.Manager.Objects[i]));
                                        //Console.WriteLine("Spawning {0}", ObjData.Manager.ObjectBase[Helpers.Manager.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                        #endregion
                        //Repeat for each world item
                        #region Helper objects
                        for (int i = 0; i < Helpers.Manager.WorldItem.Count; i++)
                        {
                            //If the helper object is not null , or not spawned for us yet and the unique id is not null
                            if (Helpers.Manager.WorldItem[i] != null && !Helpers.Manager.WorldItem[i].Spawned(Character.Information.UniqueID))
                            {
                                //If our position is lower or higher then 50 + spawnrange of that of the object
                                if (Character.Position.x >= (Helpers.Manager.WorldItem[i].x - 50) && Character.Position.x <= ((Helpers.Manager.WorldItem[i].x - 50) + spawnrange) && Character.Position.y >= (Helpers.Manager.WorldItem[i].y - 50) && Character.Position.y <= ((Helpers.Manager.WorldItem[i].y - 50) + spawnrange))
                                {
                                    if (Helpers.Manager.WorldItem[i].UniqueID != 0)
                                    {
                                        //Add our spawn
                                        Helpers.Manager.WorldItem[i].Spawn.Add(Character.Information.UniqueID);
                                        //Send visual packet
                                        client.Send(Packet.ObjectSpawn(Helpers.Manager.WorldItem[i]));
                                    }
                                }
                            }
                        }
                        #endregion
                        //If we are riding a horse and its not spawned to the player yet
                        #region Transports
                        if (Character.Transport.Right)
                        {
                            //If not spawned
                            if (!Character.Transport.Spawned)
                            {
                                //Set bool true
                                Character.Transport.Spawned = true;
                                //Spawn horse object
                                Character.Transport.Horse.SpawnMe();
                                //Send visual update player riding horse
                                Character.Transport.Horse.Send(Packet.Player_UpToHorse(Character.Information.UniqueID, true, Character.Transport.Horse.UniqueID));
                            }
                        }
                        #endregion
                        //Reset bool to false so we can re-loop the function
                        Character.Spawning = false;
                        ObjectDeSpawnCheck();
                    }
                    //If something wrong happened and we are null, we set our bool false as well.
                    Character.Spawning = false;
                }
            }
            catch (Exception ex)
            {
                //If any exception happens we disable the loop bool for re-use
                Character.Spawning = false;
                Console.WriteLine("Spawn check error {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Spawn system (Spawn our char to others).
        /////////////////////////////////////////////////////////////////////////////////   
        void ObjectPlayerSpawn(PlayerMgr s)
        {
            try
            {
                if (!s.Character.Spawned(Character.Information.UniqueID) && Character.Information.UniqueID != 0 && !s.Character.Spawning)
                {
                    //We loop the spawn check for the player that needs it.
                    s.ObjectSpawnCheck();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Object player spawn error {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn system
        /////////////////////////////////////////////////////////////////////////////////  
        public void ObjectDeSpawnCheck()
        {
            //We wrap our function inside a catcher
            try
            {
                if (Character.Spawning) return;
                //Set default spawn range
                int spawnrange = 110;
                //Make sure that the character is not null, and not despawning allready!
                if (Character != null && !Character.deSpawning && this != null)
                {
                    //Set our loop (active) bool to true so we cant do it double same way.
                    Character.deSpawning = true;
                    //Region helper objects
                    #region Helper objects
                    //Repeat for each helper object
                    for (int i = 0; i < Helpers.Manager.HelperObject.Count; i++)
                    {
                        //If our object is not null, and the object is spawned to our character
                        if (Helpers.Manager.HelperObject[i] != null && Helpers.Manager.HelperObject[i].Spawned(Character.Information.UniqueID))
                        {
                            //Make sure that its going out of range instead of in range
                            if (Character.Position.x >= (Helpers.Manager.HelperObject[i].x - 50) && Character.Position.x <= ((Helpers.Manager.HelperObject[i].x - 50) + spawnrange) && Character.Position.y >= (Helpers.Manager.HelperObject[i].y - 50) && Character.Position.y <= ((Helpers.Manager.HelperObject[i].y - 50) + spawnrange))
                            {
                            }
                            //When out of range start despawn
                            else
                            {
                                //Make sure we are on the same sectors (To prevent overlaps).
                                //Make sure the despawning object is not null
                                if (Helpers.Manager.HelperObject[i].UniqueID != 0)
                                {
                                    //Then we remove the spawn
                                    Helpers.Manager.HelperObject[i].Spawn.Remove(Character.Information.UniqueID);
                                    //And send despawn packet
                                    client.Send(Packet.ObjectDeSpawn(Helpers.Manager.HelperObject[i].UniqueID));
                                }
                            }
                        }
                    }
                    #endregion
                    //Region for objects
                    #region Objects
                    for (int i = 0; i < Helpers.Manager.Objects.Count; i++)
                    {
                        //Make sure the object in this case is not null
                        if (Helpers.Manager.Objects[i] != null)
                        {
                            //If the object is spawned to us but not in death state
                            if (Helpers.Manager.Objects[i].Spawned(Character.Information.UniqueID) && !Helpers.Manager.Objects[i].Die)
                            {
                                //The range we chosen
                                if (Helpers.Manager.Objects[i].x >= (Character.Position.x - 50) && Helpers.Manager.Objects[i].x <= ((Character.Position.x - 50) + spawnrange) && Helpers.Manager.Objects[i].y >= (Character.Position.y - 50) && Helpers.Manager.Objects[i].y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                }
                                //If out of range start despawn
                                else
                                {
                                    //Make sure the id and unique id are not null
                                    if (Helpers.Manager.Objects[i].UniqueID != 0 && !Helpers.Manager.Objects[i].Die)
                                    {
                                        //Start removing our spawn
                                        Helpers.Manager.Objects[i].Spawn.Remove(Character.Information.UniqueID);
                                        //Despawn packet sending
                                        client.Send(Packet.ObjectDeSpawn(Helpers.Manager.Objects[i].UniqueID));
                                        //Console.WriteLine("DE- Spawning {0}", ObjData.Manager.ObjectBase[Helpers.Manager.Objects[i].ID].Name);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    /*
                    #region Special objects
                    for (int i = 0; i < Helpers.Manager.SpecialObjects.Count; i++)
                    {
                        //Make sure the object in this case is not null
                        if (Helpers.Manager.SpecialObjects[i] != null)
                        {
                            //If the object is spawned to us but not in death state
                            if (Helpers.Manager.SpecialObjects[i].Spawned(Character.Information.UniqueID))
                            {
                                //The range we chosen
                                if (Helpers.Manager.SpecialObjects[i].x >= (Character.Position.x - 50) && Helpers.Manager.SpecialObjects[i].x <= ((Character.Position.x - 50) + spawnrange) && Helpers.Manager.SpecialObjects[i].y >= (Character.Position.y - 50) && Helpers.Manager.SpecialObjects[i].y <= ((Character.Position.y - 50) + spawnrange))
                                {
                                }
                                //If out of range start despawn
                                else
                                {
                                    //Sector check as we do above.
                                    //if (Character.Position.xSec == Helpers.Manager.Objects[i].xSec && Character.Position.ySec == Helpers.Manager.Objects[i].ySec)
                                    {
                                        //Make sure the id and unique id are not null
                                        if (Helpers.Manager.SpecialObjects[i].UniqueID != 0)
                                        {
                                            //Start removing our spawn
                                            Helpers.Manager.SpecialObjects[i].Spawn.Remove(Character.Information.UniqueID);
                                            //Despawn packet sending
                                            client.Send(Packet.ObjectDeSpawn(Helpers.Manager.SpecialObjects[i].UniqueID));
                                            //Console.WriteLine("DE- Spawning {0}", ObjData.Manager.ObjectBase[Helpers.Manager.Objects[i].ID].Name);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                     */
                    //Region for clients
                    #region Clients
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        if (Helpers.Manager.clients[i] != null && Helpers.Manager.clients[i] != this && Character.Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                        {
                            if (Helpers.Manager.clients[i].Character.Position.x >= (Character.Position.x - 50) && Helpers.Manager.clients[i].Character.Position.x <= ((Character.Position.x - 50) + spawnrange) && Helpers.Manager.clients[i].Character.Position.y >= (Character.Position.y - 50) && Helpers.Manager.clients[i].Character.Position.y <= ((Character.Position.y - 50) + spawnrange))
                            {
                            }
                            else
                            {
                                if (Character.Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                                {
                                    //Extra check before we send the packet update
                                    if (Helpers.Manager.clients[i].Character.Information.UniqueID != 0)
                                    {
                                        Character.Spawn.Remove(Helpers.Manager.clients[i].Character.Information.UniqueID);
                                        client.Send(Packet.ObjectDeSpawn(Helpers.Manager.clients[i].Character.Information.UniqueID));
                                    }
                                }
                                ObjectDePlayerSpawn(Helpers.Manager.clients[i]);
                            }
                        }
                    }
                    #endregion
                    //Region for items
                    #region Items
                    for (int i = 0; i < Helpers.Manager.WorldItem.Count; i++)
                    {
                        if (Helpers.Manager.WorldItem[i] != null && Helpers.Manager.WorldItem[i].Spawned(Character.Information.UniqueID) && Helpers.Manager.WorldItem[i].UniqueID != 0)
                        {
                            if (Helpers.Manager.WorldItem[i].x >= (Character.Position.x - 50) && Helpers.Manager.WorldItem[i].x <= ((Character.Position.x - 50) + spawnrange) && Helpers.Manager.WorldItem[i].y >= (Character.Position.y - 50) && Helpers.Manager.WorldItem[i].y <= ((Character.Position.y - 50) + spawnrange))
                            {
                            }
                            else
                            {
                                if (Helpers.Manager.WorldItem[i].Spawned(Character.Information.UniqueID) && Helpers.Manager.WorldItem[i].Model != 0)
                                {
                                    if (Helpers.Manager.WorldItem[i].UniqueID != 0)
                                    {
                                        Helpers.Manager.WorldItem[i].Spawn.Remove(Character.Information.UniqueID);
                                        client.Send(Packet.ObjectDeSpawn(Helpers.Manager.WorldItem[i].UniqueID));
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    //Set bool to false so we can use the loop again
                    Character.deSpawning = false;
                }
                //Set bool to false so we can use the loop again (incase something happened and we are null).
                Character.deSpawning = false;
            }

            catch (Exception ex)
            {
                //If any exception is made we disable the loop
                Character.deSpawning = false;
                Console.WriteLine("Despawn error {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn system (Spawn our char to others).
        /////////////////////////////////////////////////////////////////////////////////    
        void ObjectDePlayerSpawn(PlayerMgr s)
        {

            try
            {
                if (s.Character.Spawned(Character.Information.UniqueID) && !s.Character.deSpawning)
                {
                    if (s.Character.Information.UniqueID != 0)
                    {
                        s.ObjectDeSpawnCheck();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Systems despawn error {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // DE-Spawn
        /////////////////////////////////////////////////////////////////////////////////    
        void DeSpawnMe()
        {
            //Wrap our function inside a catcher
            try
            {
                //Checks before continuing
                if (Character.Network.Exchange.Window) Exchange_Close();
                if (Character.Action.nAttack) StopAttackTimer();
                if (Character.Action.sAttack) StopAttackTimer();
                if (Character.Action.sCasting) StopAttackTimer();
                if (Character.Stall.Stallactive) StallClose();
                //Clients
                #region Client spawns
                for (int b = 0; b < Helpers.Manager.clients.Count; b++)
                {
                    if (Helpers.Manager.clients[b] != null && Helpers.Manager.clients[b].Character.Spawned(Character.Information.UniqueID) && Helpers.Manager.clients[b] != this)
                    {
                        if (Helpers.Manager.clients[b].Character.Information.UniqueID != 0)
                        {
                            Helpers.Manager.clients[b].Character.Spawn.Remove(Character.Information.UniqueID);
                            Helpers.Manager.clients[b].client.Send(Packet.ObjectDeSpawn(Character.Information.UniqueID));
                        }
                    }
                }
                Character.Spawn.Clear();
                #endregion
                //Helper objects
                #region Helper objects
                for (int i = 0; i < Helpers.Manager.HelperObject.Count; i++)
                {
                    if (Helpers.Manager.HelperObject[i] != null && Helpers.Manager.HelperObject[i].Spawned(Character.Information.UniqueID))
                    {
                        if (Character.Information.UniqueID != 0 && Helpers.Manager.HelperObject[i].UniqueID != 0)
                        {
                            Helpers.Manager.HelperObject[i].Spawn.Remove(Character.Information.UniqueID);
                        }
                    }
                }
                #endregion
                //Objects
                #region Objects
                for (int i = 0; i < Helpers.Manager.Objects.Count; i++)
                {
                    if (Helpers.Manager.Objects[i] != null && Helpers.Manager.Objects[i].Spawned(Character.Information.UniqueID))
                    {
                        if (Character.Information.UniqueID != 0 && Helpers.Manager.Objects[i].UniqueID != 0)
                        {
                            Helpers.Manager.Objects[i].Spawn.Remove(Character.Information.UniqueID);
                        }
                    }
                }
                #endregion
                //Drops
                #region Drops
                for (int i = 0; i < Helpers.Manager.WorldItem.Count; i++)
                {
                    if (Helpers.Manager.WorldItem[i] != null && Helpers.Manager.WorldItem[i].Spawned(Character.Information.UniqueID) && Helpers.Manager.WorldItem[i].UniqueID != 0)
                    {
                        if (Helpers.Manager.WorldItem[i].Spawned(Character.Information.UniqueID))
                        {
                            if (Character.Information.UniqueID != 0 && Helpers.Manager.WorldItem[i].UniqueID != 0)
                            {
                                Helpers.Manager.WorldItem[i].Spawn.Remove(Character.Information.UniqueID);
                            }
                        }
                    }
                }
                #endregion
                //Char spawns
                #region Char spawns
                for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                {
                    if (Helpers.Manager.clients[i] != this && Helpers.Manager.clients[i] != null)
                    {
                        if (Character.Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                        {
                            if (Character.Information.UniqueID != 0 && Helpers.Manager.clients[i].Character.Information.UniqueID != 0)
                            {
                                Character.Spawn.Remove(Helpers.Manager.clients[i].Character.Information.UniqueID);
                            }
                        }
                    }
                }
                #endregion
                //Check if our character is not null
                if (Character.Information.UniqueID != 0)
                {
                    //Check if we have transport active
                    if (Character.Transport.Right)
                    {
                        //Set bools for despawning
                        Character.Transport.Spawned = false;
                        Character.Transport.Horse.Information = false;
                        Character.Transport.Horse.DeSpawnMe(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Despawn me error {0}", ex);
                Log.Exception(ex);
            }
        }
    }
}
