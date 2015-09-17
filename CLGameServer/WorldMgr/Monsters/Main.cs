using CLFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CLGameServer.WorldMgr
{
    public class _agro
    {
        public int playerID;
        public int playerDMD;
    }
    public partial class Monsters
    {
        public List<int> Spawn = new List<int>();
        public byte xSec, ySec, Agresif, oldAgresif, Type, Kat, State, LocalType, spawnOran, LastState, LastState2;
        public double AttackSpeed = 1.0, WalkingTime, RecordedTime;
        public float x, z, y, OriginalX, OriginalY, wx, wy, WalkingSpeed, RunningSpeed, BerserkerSpeed;
        public short area;
        public ushort Angle;
        public object Target;
        public int ID, HP, aTime, MonsterAgroCount, UniqueID, LastCasting;
        public sbyte Move;
        public bool UniqueGuardMode = false, Busy, Runing, AutoMovement, Attacking, Die, GetDie, AutoSpawn, bSleep, Frozen, Frostbite, Burned, Shock, Darkness;
        public bool[] aRound = new bool[10];
        public bool[] guard = new bool[3];
        public List<_agro> AgressiveDMG = new List<_agro>();
        public GenerateUniqueID Ids;
        public _debuff DeBuff;
        public void InitalizeNpcs()
        {
            DeBuff.Effect.EffectID = new Effect.EffectNumbers[20];
            DeBuff.Effect.EffectImpactTimer = new Timer[20];
            DeBuff.Effect.SkillID = new int[20];

            SpawnWatch.Start();
        }
        public struct _debuff
        {
            public _effect Effect;
        }
        public struct _effect
        {
            public Effect.EffectNumbers[] EffectID;
            public Timer[] EffectImpactTimer;
            public int[] SkillID;
        }
        public object GetTarget()
        {

            int id = 0;
            if (AgressiveDMG != null && AgressiveDMG.Count > 0)
                for (byte b = 0; b < AgressiveDMG.Count; b++)
                {
                    if (AgressiveDMG[b].playerDMD == AgressiveDMG.Max(f => f.playerDMD))
                    {
                        id = AgressiveDMG[b].playerID;
                        break;
                    }
                }
            return Helpers.GetInformation.GetPlayer(id);
        }
        public void DeleteTarget()
        {
            try
            {
                for (byte b = 0; b < AgressiveDMG.Count; b++)
                {
                    if (AgressiveDMG[b].playerDMD == AgressiveDMG.Max(f => f.playerDMD))
                    {
                        if (AgressiveDMG.Count > 1)
                        {
                            AgressiveDMG.Remove(AgressiveDMG[b]);
                            return;
                        }
                        else AttackStop();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void AddAgroDmg(int playerid, int dmg)
        {
            try
            {
                if (AgressiveDMG != null)
                {
                    if (AgressiveDMG.Exists(t => t.playerID == playerid))
                    {
                        AgressiveDMG[AgressiveDMG.FindIndex(getindex => getindex.playerID == playerid)].playerDMD += dmg;
                    }
                    else
                    {
                        _agro AddNewAgro = new _agro();
                        AddNewAgro.playerID = playerid;
                        AddNewAgro.playerDMD = dmg;
                        AgressiveDMG.Add(AddNewAgro);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public _agro GetAgroClass(int id)
        {
            for (byte b = 0; b <= AgressiveDMG.Count - 1; b++)
            {
                if (AgressiveDMG[b].playerID == id) return AgressiveDMG[b];
            }
            return null;
        }
        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate (int bk)
                    {
                        return bk == id;
                    }
                    );

            return result;
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
