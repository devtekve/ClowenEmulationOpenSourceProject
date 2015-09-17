using System;
using System.Linq;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Monster Summon Scrolls
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleSummon(int scrollid)
        {
            try
            {
                //if (Character.Information.Level < 10) return;

                int count = 1;//Default should be set to 1

                //single scroll
                if (scrollid == 3936)
                {
                    count = 5;
                }
                //party scroll
                if (scrollid == 3935)
                {
                    if (Character.Network.Party == null) return;
                    if (Character.Network.Party.Members.Count < 5) return;

                    count = Character.Network.Party.Members.Count;
                }

                int model = GetStrongMobByLevel(Character.Information.Level);
                byte type = ObjData.Manager.ObjectBase[model].ObjectType;

                for (int i = 1; i <= count; i++)
                {
                    WorldMgr.Monsters Spawn = new WorldMgr.Monsters();

                    Spawn.ID = model;
                    Spawn.Type = type;
                    Spawn.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                    Spawn.UniqueID = Spawn.Ids.GetUniqueID;
                    Spawn.x = Character.Position.x;
                    Spawn.z = Character.Position.z;
                    Spawn.y = Character.Position.y;
                    Spawn.OriginalX = Spawn.x;
                    Spawn.OriginalY = Spawn.y;
                    Spawn.xSec = Character.Position.xSec;
                    Spawn.ySec = Character.Position.ySec;
                    Spawn.AutoMovement = true;
                    Spawn.State = 1;
                    Spawn.Move = 1;
                    Spawn.WalkingSpeed = ObjData.Manager.ObjectBase[Spawn.ID].SpeedWalk;
                    Spawn.RunningSpeed = ObjData.Manager.ObjectBase[Spawn.ID].SpeedRun;
                    Spawn.BerserkerSpeed = ObjData.Manager.ObjectBase[Spawn.ID].SpeedZerk;
                    Spawn.HP = ObjData.Manager.ObjectBase[model].HP;
                    Spawn.Agresif = ObjData.Manager.ObjectBase[model].Agresif;
                    Spawn.LocalType = 1;
                    Spawn.AutoSpawn = false;
                    Spawn.Kat = 1;
                    Helpers.Functions.aRound(ref Spawn.x, ref Spawn.y, 1);
                    Helpers.Manager.Objects.Add(Spawn);
                    Spawn.SpawnMe();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public int GetStrongMobByLevel(byte Level)
        {
            try
            {
                int LevelDiff = 110;
                int NearestModel = 0;

                foreach (ObjData.objectdata mob in ObjData.Manager.ObjectBase)
                {
                    if (mob != null)
                    {
                        if (mob.Name.Contains("_STRONG_"))
                        {
                            if (LevelDiff > Math.Abs(Level - mob.Level))
                            {
                                LevelDiff = Math.Abs(Level - mob.Level);
                                NearestModel = mob.ID;
                            }
                        }
                    }
                }
                return NearestModel;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }
    }
}
