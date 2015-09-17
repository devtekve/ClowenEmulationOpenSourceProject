using System;
using CLFramework;
using System.Collections.Generic;

namespace CLGameServer.Helpers
{
    class GetInformation
    {
        public static Settings.SrevoServerInfo GetServerByEndPoint(string ip, int port)
        {
            Settings.SrevoServerInfo LS = null;

            foreach (KeyValuePair<int, Settings.SrevoServerInfo> LSI in Settings.LSList)
            {
                if (LSI.Value.ip == ip && LSI.Value.ipcport == port)
                {
                    LS = LSI.Value;
                }
            }
            return LS;
        }
        public static WorldMgr.pet_obj GetPet(int id)
        {
            for (int i = 0; i < Manager.HelperObject.Count; i++)
            {
                if (Manager.HelperObject[i] != null && Manager.HelperObject[i].UniqueID == id)
                    return Manager.HelperObject[i];
            }
            return null;
        }
        public static WorldMgr.party GetPartyInfo(int id)
        {
            for (int i = 0; i < Manager.Party.Count; i++)
            {
                if (Manager.Party[i].ptid == id)
                {
                    return Manager.Party[i];
                }
            }
            return null;
        }
        public static PlayerMgr GetPlayerName(string name)
        {
            lock (Manager.clients)
            {
                for (int i = 0; i < Manager.clients.Count; i++)
                {
                    try
                    {
                        if (Manager.clients[i] != null && Manager.clients[i].Character.Information.Name == name)
                            return Manager.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            return null;
        }
        public static PlayerMgr GetGuildPlayer(int id)
        {
            lock (Manager.clients)
            {
                for (int i = 0; i < Manager.clients.Count; i++)
                {
                    try
                    {
                        if (Manager.clients[i] != null && Manager.clients[i].Character.Network.Guild.Guildid == id)
                            return Manager.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            return null;
        }
        public static PlayerMgr GetPlayer(int id)
        {
            lock (Manager.clients)
            {
                for (int i = 0; i < Manager.clients.Count; i++)
                {
                    try
                    {
                        if (Manager.clients[i] != null && Manager.clients[i].Character.Information.UniqueID == id)
                            return Manager.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            return null;
        }
        public static PlayerMgr GetPlayerMainid(int id)
        {
            lock (Manager.clients)
            {
                for (int i = 0; i < Manager.clients.Count; i++)
                {
                    try
                    {
                        if (Manager.clients[i] != null && Manager.clients[i].Character.Information.CharacterID == id)
                            return Manager.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            return null;
        }
        public static PlayerMgr GetPlayers(int id)
        {
            try
            {
                lock (Manager.clients)
                {
                    for (int i = 0; i < Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Manager.clients[i] != null && Manager.clients[i].Character.Information.UniqueID == id)
                                return Manager.clients[i];
                        }
                        catch (Exception ex)
                        {
                           Log.Exception(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public static PlayerMgr GetPlayerid(int id)
        {
            try
            {
                lock (Manager.clients)
                {
                    for (int i = 0; i < Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Manager.clients[i] != null && Manager.clients[i].Character.Information.CharacterID == id)
                                return Manager.clients[i];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("GetPlayerid Error on index {1}/{2}: {0}", ex, i, Manager.clients.Count);
                            Log.Exception(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public static WorldMgr.Monsters GetObject(string NpcName)
        {
            for (int i = 1; i < Manager.Objects.Count;)
            {
                if (Manager.Objects[i] != null && ObjData.Manager.ObjectBase[Manager.Objects[i].ID].Name == NpcName)
                {
                    for (int z = 0; z <= Manager.Objects.Count - 1; z++)
                    {
                        if (Manager.Objects[z] != null && Manager.Objects[z].ID == i)
                            return Manager.Objects[z];
                    }
                }
                i++;
            }
            return null;
        }
        public static WorldMgr.Monsters GetObject(int id)
        {
            try
            {
                for (int i = 0; i <= Manager.Objects.Count - 1; i++)
                {
                    if (Manager.Objects[i] != null && Manager.Objects[i].UniqueID == id)
                        return Manager.Objects[i];
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public static int GetPartyleader(int id)
        {
            try
            {
                for (int i = 0; i < Manager.Party.Count; i++)
                {
                    if (Manager.Party[i].ptid == id)
                        return Manager.Party[i].LeaderID;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }

        public static WorldMgr.Items GetWorldItem(int id)
        {
            try
            {
                for (int i = 0; i <= Manager.WorldItem.Count - 1; i++)
                {
                    if (Manager.WorldItem[i] != null && Manager.WorldItem[i].UniqueID == id)
                        return Manager.WorldItem[i];
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public static object GetObjects(int id)
        {
            try
            {
                WorldMgr.Monsters o = GetObject(id);
                if (o != null) return o;

                PlayerMgr sys = GetPlayers(id);
                if (sys != null) return sys;

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
    }
}
