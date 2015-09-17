using System;
using CLFramework;
using System.Collections.Generic;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public SRClient client;
        public Decode                   PacketInformation;
        public WorldMgr.player                   Player;
        public WorldMgr.character                Character;
        public PlayerMgr(SRClient s)
        {
            client = s;
        }
        public void Dispose()
        {
            
            GC.Collect(GC.GetGeneration(this));
        }
        public void Send(List<int> list, byte[] buff)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                            if (Helpers.Manager.clients[i] != null)
                            {
                                if (Helpers.Manager.clients[i] != this)
                                {
                                    if (CheckSpawned(list, Helpers.Manager.clients[i].Character.Information.UniqueID) && Character.InGame)
                                    {
                                        if (Helpers.Manager.clients[i].Character.Spawned(this.Character.Information.UniqueID) && Helpers.Manager.clients[i].Character.InGame)
                                            Helpers.Manager.clients[i].client.Send(buff);
                                    }
                                }
                                else
                                    client.Send(buff);

                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static bool CheckSpawned(List<int> Spawn, int id)
        {
            bool result = Spawn.Exists(
                    delegate (int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        if (Helpers.Manager.clients[i] != null)
                        {
                            if (Helpers.Manager.clients[i] != this)
                            {
                                if (Character.Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID) && Character.InGame)
                                {
                                    if (Helpers.Manager.clients[i].Character.Spawned(Character.Information.UniqueID) && Helpers.Manager.clients[i].Character.InGame)
                                        Helpers.Manager.clients[i].client.Send(buff);
                                }
                            }
                            else
                            {
                                client.Send(buff);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public bool MonsterCheck(int id)
        {
            try
            {
                if (Character.Action.MonsterID != null)
                    for (int i = 0; i < Character.Action.MonsterID.Count; i++)
                    {
                        if (Character.Action.MonsterID != null && Character.Action.MonsterID[i] != 0 && Character.Action.MonsterID[i] == id) return true;
                    }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
    }
}
