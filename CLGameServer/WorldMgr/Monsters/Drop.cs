using CLFramework;
using System;
using System.Collections.Generic;

namespace CLGameServer.WorldMgr
{
    public partial class Monsters
    {
        #region Get Informations
        public List<int> GetLevelItem(byte level)
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        string namecheck = ObjData.Manager.ObjectBase[ID].Name;
                        {
                            if (ObjData.Manager.ItemBase[i].Level != 0 && ObjData.Manager.ItemBase[i].Level >= level - 4 && ObjData.Manager.ItemBase[i].Level <= (level + 4) && ObjData.Manager.ItemBase[i].SOX == 0 && GetItemType(ObjData.Manager.ItemBase[i].ID) == 0)
                            {
                                if (i != 0)
                                    item.Add(i);
                            }
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public List<int> GetBlueRandom()
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < ObjData.Manager.MagicOptions.Count; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        {
                            if (i != 0)
                                item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public List<int> GetMaterials(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].TypeID4 == 20 && ObjData.Manager.ItemBase[i].Level >= level - 1 && ObjData.Manager.ItemBase[i].Level <= (level + 1) && ObjData.Manager.ItemBase[i].TypeID2 == 3 && ObjData.Manager.ItemBase[i].Item_Mall_Type == 0 && ObjData.Manager.ItemBase[i].Race == 3)//&& ObjData.Manager.ItemBase[i].Class_B == 11)//(ObjData.Manager.ItemBase[i].Name.Contains("ETC"))
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public List<int> GetPotions(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.HP_POTION || ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.MP_POTION || ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.VIGOR_POTION)
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public List<int> GetElixir(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Etctype == ObjData.item_database.EtcType.ELIXIR)
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public List<int> GetLevelItemSOX(byte level)
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
                {
                    if (ObjData.Manager.ItemBase[i] != null)
                    {
                        if (ObjData.Manager.ItemBase[i].Level >= level - 4 && ObjData.Manager.ItemBase[i].Level <= (level + 4) && GetItemType(ObjData.Manager.ItemBase[i].ID) == 0 && ObjData.Manager.ItemBase[i].SOX == 2)
                        {
                            if (i != 0)
                                item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public static byte GetItemType(int id)
        {
            try
            {
                if (ObjData.Manager.ItemBase[id].TypeID2 == 1) return 0;
                else if (ObjData.Manager.ItemBase[id].TypeID2 == 3) return 1;
                else if (ObjData.Manager.ItemBase[id].TypeID2 == 4) return 4;
                else if (ObjData.Manager.ItemBase[id].TypeID2 == 8) return 8;
                else if (ObjData.Manager.ItemBase[id].TypeID2 == 9) return 9;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 255;
        }
        #endregion
        public void MonsterDrop()
        {
            try
            {
                if (GetDie || Die)
                {
                    if (Type != 16)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Set Target Information
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        PlayerMgr sys = (PlayerMgr)GetTarget();
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // If There's no target return
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (sys == null) return;

                        sbyte Leveldiff = (sbyte)(sys.Character.Information.Level - ObjData.Manager.ObjectBase[ID].Level);
                        int Amountinfo = 0;

                        if (Math.Abs(Leveldiff) < 10 || Math.Abs(Leveldiff) == 0)
                        {
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Gold Drop
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            #region Gold
                            int Golddrop = Rnd.Next(ObjData.Manager.LevelGold[ObjData.Manager.ObjectBase[ID].Level].min, ObjData.Manager.LevelGold[ObjData.Manager.ObjectBase[ID].Level].max);
                            Amountinfo = 0;
                            if (Type == 4 && (Rnd.Next(0, 200) < 200 * Helpers.Settings.Rate.Gold)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                            if (Type == 3 && (Rnd.Next(0, 200) < 200 * Helpers.Settings.Rate.Gold)) Amountinfo = Convert.ToByte(Rnd.Next(4, 6));
                            if (Type == 1 && (Rnd.Next(0, 200) < 200 * Helpers.Settings.Rate.Gold)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                            if (Type == 0 && (Rnd.Next(0, 200) < 100 * Helpers.Settings.Rate.Gold)) Amountinfo = 1;

                            for (byte a = 1; a <= Amountinfo;)
                            {
                                WorldMgr.Items Gold_Drop = new WorldMgr.Items();

                                Gold_Drop.amount = Golddrop * Helpers.Settings.Rate.Gold;
                                Gold_Drop.Model = 1;

                                if (Gold_Drop.amount < 1000)
                                    Gold_Drop.Model = 1;
                                else if (Gold_Drop.amount > 1000 && Gold_Drop.amount < 10000)
                                    Gold_Drop.Model = 2;
                                else if (Gold_Drop.amount > 10000)
                                    Gold_Drop.Model = 3;

                                Gold_Drop.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.World);
                                Gold_Drop.UniqueID = Gold_Drop.Ids.GetUniqueID;
                                Gold_Drop.x = x;
                                Gold_Drop.z = z;
                                Gold_Drop.y = y;
                                Gold_Drop.xSec = xSec;
                                Gold_Drop.ySec = ySec;
                                Gold_Drop.Type = 1;
                                Gold_Drop.downType = true;
                                Gold_Drop.fromType = 5;
                                Gold_Drop.CalculateNewPosition();

                                Helpers.Manager.WorldItem.Add(Gold_Drop);
                                Gold_Drop.Send(Client.Packet.ObjectSpawn(Gold_Drop), true);
                                a++;

                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                // Send Info To Grabpet
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                if (((PlayerMgr)GetTarget()).Character.Grabpet.Active)
                                {
                                    ((PlayerMgr)GetTarget()).Pet_PickupItem(Gold_Drop);
                                }
                            }
                            #endregion
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Drop Database
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            #region Drop Databases
                            foreach (KeyValuePair<string, ObjData.drop_database> p in ObjData.Manager.DropBase)
                            {
                                Amountinfo = p.Value.GetAmount(ObjData.Manager.ObjectBase[ID].Type, p.Key);
                                if (Amountinfo > 0)
                                {
                                    for (byte c = 1; c <= Amountinfo; c++)
                                    {
                                        Items Dropped_Item = new Items();
                                        Dropped_Item.Model = p.Value.GetDrop(ObjData.Manager.ObjectBase[ID].Level, ID, p.Key, sys.Character.Information.Race);
                                        if (Dropped_Item.Model == -1) continue;
                                        Dropped_Item.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.World);
                                        Dropped_Item.UniqueID = Dropped_Item.Ids.GetUniqueID;
                                        Dropped_Item.PlusValue = Function.Items.RandomPlusValue();
                                        Dropped_Item.MagAtt = Function.Items.RandomStatValue();
                                        Dropped_Item.x = x;
                                        Dropped_Item.z = z;
                                        Dropped_Item.y = y;
                                        Dropped_Item.xSec = xSec;
                                        Dropped_Item.ySec = ySec;
                                        Dropped_Item.Type = p.Value.GetSpawnType(p.Key);
                                        Dropped_Item.fromType = 5;
                                        Dropped_Item.downType = true;
                                        Dropped_Item.fromOwner = UniqueID;
                                        Dropped_Item.amount = p.Value.GetQuantity(Type, p.Key);
                                        Dropped_Item.Owner = ((PlayerMgr)GetTarget()).Character.Account.ID;
                                        Dropped_Item.CalculateNewPosition();
                                        Helpers.Manager.WorldItem.Add(Dropped_Item);
                                        Dropped_Item.Send(Client.Packet.ObjectSpawn(Dropped_Item), true);
                                    }
                                }
                            }
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Drop system error: {0}", ex);
            }
        }
    }
}
