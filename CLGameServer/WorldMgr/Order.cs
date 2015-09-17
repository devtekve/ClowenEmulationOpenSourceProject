using CLFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CLGameServer.WorldMgr
{
    public class BuyPack
    {
        List<ObjData.slotItem> item = new List<ObjData.slotItem>(5);
        public void Add(ObjData.slotItem objects)
        {
            if (item.Count < 5)
            {
                item.Add(objects);
            }
            else
            {
                if (item.Count >= 5)
                {
                    item[0] = item[1];
                    item[1] = item[2];
                    item[2] = item[3];
                    item[3] = item[4];
                    item[4] = objects;
                }
            }
        }
        public ObjData.slotItem Get(byte index)
        {
            ObjData.slotItem ret = item[index];
            item.Remove(ret);
            return ret;
        }
    }
    public class player
    {
        public string AccountName, Password;
        public int ID, CreatingCharID;
        public long pGold;
        public byte wSlots;
        public int Silk, SilkPrem;
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public class character
    {
        public _Information Information;
        public _job Job;
        public _Messagespm Getmessageinfo;
        public _Stats Stat;
        public _pos Position;
        public _ticket Ticket;
        public _speed Speed;
        public _action Action;
        public _network Network;
        public _alchemy Alchemy;
        public _stall Stall;
        public _guild GuildNetWork;
        public _state State;
        public _Guide Guideinfo;
        public _account Account;
        public BuyPack Buy_Pack = new BuyPack();
        public _trans Transport;
        public _grabpet Grabpet;
        public _attackpet Attackpet;
        public GenerateUniqueID Ids;
        public Guildinfo Guild;
        public _Quest Quest;
        public _Blues Blues;
        public premium Premium;
        public List<int> Spawn = new List<int>();
        public bool[] aRound = new bool[10];
        public bool InGame, Spawning, deSpawning, Teleport, Transformed;

        public void InitializeCharacter()
        {
            Action.Buff.OverID = new int[20];
            Action.Buff.SkillID = new int[20];
            Action.Buff.UpdatedStats = new _Stats[20];
            Action.Buff.InfiniteBuffs = new Dictionary<string, byte>();
            Action.DeBuff.Effect.EffectID = new Effect.EffectNumbers[20];
            Action.DeBuff.Effect.EffectImpactTimer = new Timer[20];
            Action.DeBuff.Effect.SkillID = new int[20];
            Information.Item.Potion = new byte[20];
            Action.MonsterID = new List<int>(8);
            Action.movementskill = false;
            Network.Guild = new guild();
        }

        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

        public struct _account
        {
            public long StorageGold;
            public int ID;
            public byte StorageSlots;
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
        public struct _trans
        {
            public bool Right;
            public bool Spawned;
            public pet_obj Horse;
        }
        public struct _job
        {
            public int exp, state;
            public string Jobname;
            public byte type, rank, level;
            public bool jobactive;
        }
        public struct _grabpet
        {
            public bool Active;
            public bool Spawned;
            public bool Picking;
            public int Grabpetid;
            public pet_obj Details;
        }
        public struct _attackpet
        {
            public bool Active;
            public bool Spawned;
            public bool Picking;
            public int Uniqueid;
            public pet_obj Details;
        }
        public struct premium
        {
            public int TicketItemID, TicketID, OwnerID;
            public bool Active;
            public DateTime StartTime, EndTime;
        }
        public struct Guildinfo
        {
            public string Name, MessageTitle, Message, MyGrant, MyGuildName, UnionGuildName, UnionLeader, UnionGuildLeader;
            public int GuildStorageSlots, GuildWarGold, GuildID, MyGuildAuth, GuildMemberId, GuildPoints, MyGuildPoints, UnionCount, UnionMemberCount, UnionGuildLeaderID, UnionGuildID, UnionGuildLeaderModel;
            public byte GuildMemberCount, MyGuildRank, MyGuildPosition, Level, UnionLevel;
            public bool Inguild, UsingStorage;
        }
        public struct _Messagespm
        {
            public string from, to, message;
            public short status;
            public DateTime time;
        }
        public struct _Quest
        {
            public bool QuestActive;
            public int TalkToNpc, QuestItem, QuestDrop, QuestNPC;
        }
        public struct _Information
        {
            //Need to clean this up (TODO:)
            public bool SkyDroming, Storage, FirstLogin, Handle, Quit, Scroll, Casting, Skill, TuruncuZerk, WelcomeMessage, Murderer, Invisible;
            public int Model, SpBar, SkillPoint, CharacterID, Slots, Pvpstate, StallModel;
            public double Angle;
            public int UniqueID, MaskModel, Online;
            public byte AutoInverstExp, Level, HighLevel, BerserkBar, Volume, GM, Place, Title, Phy_Balance, Mag_Balance, Pvptype, BerserkOran, ExpandedStorage, Race;
            public short Attributes;
            public string Name;
            public long XP, Gold;
            public bool Berserking, PvpWait, PvP, Job, Autonotice, CheckParty , GuildPenalty;
            public List<byte> InventorylistSlot;
            public _item Item;
        }
        public struct _Blues
        {
            public int Luck, Resist_Frostbite, Resist_Eshock, Resist_Burn, Resist_Poison, Resist_Zombie, Resist_Stun, Resist_CSMP, Resist_Disease, Resist_Sleep, Resist_Fear, Resist_All, UniqueDMGInc, MonsterIgnorance;
            public _Stats[] UpdatedStats;
            public double hpregen, mpregen;
        }
        public struct _state
        {
            public bool Die, Busy, Sitting, Standing, Exchanging, Inparty, GuildInvite, Sendonce, UnionApply;
            public bool SafeState;
            public bool Frozen, Frostbite, Burned, Shocked;
            public byte LastState, DeadType;
        }
        public struct _Guide
        {
            public int[] G1;
            public int[] Gchk;
        }
        public struct _item
        {
            public int wID, sID;
            public short sAmount;
            public byte[] Potion;
        }

        public struct _Stats
        {
            public double MinPhyAttack, MaxPhyAttack, MinMagAttack, MaxMagAttack, PhyDef, MagDef, uMagDef, uPhyDef;
            public double Absorb_mp, UpdatededPhyAttack, UpdatededMagAttack, AttackPower, EkstraMetre;
            public short Strength, Intelligence;
            public short phy_Absorb, mag_Absorb;
            public byte BowRange;
            public double Hit, Parry;
            public int Hp, Mp, SecondHp, SecondMP, BlockRatio, CritParryRatio;
            public _skill Skill;
        }
        public struct _pos
        {
            public float x, y, z;
            public float wX, wY, wZ, kX, kY;
            public byte packetxSec, packetySec;
            public ushort packetX, packetZ, packetY;
            public byte xSec, ySec, wxSec, wySec;
            public double RecordedTime, Time;
            public bool Walking, Walk;
            public ushort Angle;
            public bool GM,WalkRun;
        }
        public struct _speed
        {
            public float WalkSpeed, RunSpeed, BerserkSpeed,INC;
        }
        public struct _skill
        {
            public int[] Mastery;
            public byte[] Mastery_Level;
            public int[] Skill;
            public int SkillCastingID;
            public int AmountSkill;
        }
        public struct _action
        {
            public int Target, UsingSkillID, CastingSkill, ImbueID, AttackingID;
            public bool Cast, nAttack, PickUping, sAttack, sCasting, movementskill, repair, normalattack;
            public _buff Buff;
            public _debuff DeBuff;
            public object Object;
            public _usingSkill Skill;
            public List<int> MonsterID;
            public byte sSira;
            public bool Check()

            {
                if (MonsterID == null) return false;
                if (MonsterID.Count >= 8) return true;
                return false;
            }
            public bool MonsterIDCheck(int id)
            {
                bool result = MonsterID.Exists(
                        delegate (int bk)
                        {
                            return bk == id;
                        }
                        );
                return result;
            }
        }
        public struct _usingSkill
        {
            public int MainSkill, MainCasting;
            public bool P_M;
            public byte Distance, Instant, NumberOfAttack, Targethits, Tdistance, Found, sSira;
            public int[] SkillID;
            public int[] FoundID;
            public bool[] TargetType;
            public byte OzelEffect;
            public bool canUse;
        }

        public struct _buff
        {
            public int[] OverID;
            public int[] SkillID;
            public Dictionary<string, byte> InfiniteBuffs;
            public byte slot, count;
            public short castingtime;
            public bool Casting;
            public _Stats[] UpdatedStats;
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

        public struct _network
        {
            public int TargetID;
            public party Party;
            public guild Guild;
            public _exchange Exchange;
            public stall Stall;
        }
        public struct _exchange
        {
            public List<ObjData.slotItem> ItemList;
            public long Gold;
            public bool Approved;
            public bool Window;
        }
        public struct _stall
        {
            public List<ObjData.slotItem> ItemList;
            public List<string> StallName;
            public List<int> Stallsid;
            public Thread StallThread;
            public bool Stallactive;
        }
        public struct _guild
        {
            public List<string> GuildList;
            public List<int> MemberList;
            public List<string> CharName;
            public string UniqueGuild;
        }
        public struct _alchemy
        {
            public bool working;
            public List<ObjData.slotItem> ItemList;
            public List<int> StonesList;
            public List<int> Elementlist;
            public Thread AlchemyThread;
            public ObjData.slotItem AlchemyItem;
            public ObjData.slotItem Elixir, Stone;
            public ObjData.slotItem LuckyPowder;
        }
        public struct _ticket
        {
            public int[] TicketItemID;
            public int[] TicketSecondTimeLeft;
            public int[] TicketTimeGaveOut;
            public int[] TicketFullTimeLeft;
            public bool TicketActive;
            public bool[] TicketSecondActive;
            public DateTime[] ticketbufftime;
            public DateTime[] ticketstarttime;
            public int[] TicketOverID;
            public DateTime[] ticketsecbufftime;
            public byte ticketcount;
            public int Exp;
            public _Stats PlusStat;
        }
    }
    public class pet_obj
    {
        public string Petname, OwnerName;
        public int Model;
        public int UniqueID, OwnerID;
        public Int64 exp;
        public double x, z, y;
        public byte xSec, ySec, Slots, Named, Level;
        public int Hp, Hpg;
        public bool Information, Walking, Attacking, Defensive;
        public GenerateUniqueID Ids;
        public float Walk = 45, Run = 95, Zerk = 100, Speed1, Speed2;
        public List<int> Spawn = new List<int>();

        public List<int> statussend()
        {
            try
            {

                if (Model != 0)
                {
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            try
                            {
                                if (Helpers.Manager.clients[i].Character.Position.x >= (x - 50) && Helpers.Manager.clients[i].Character.Position.x <= ((x - 50) + 100) && Helpers.Manager.clients[i].Character.Position.y >= (y - 50) && Helpers.Manager.clients[i].Character.Position.y <= ((y - 50) + 100))
                                {
                                    Helpers.Manager.clients[i].client.Send(Client.Packet.ChangeStatus(UniqueID, 3, 0));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return Spawn;
        }

        public List<int> Speedsend()
        {
            try
            {

                if (Model != 0)
                {
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            try
                            {
                                if (Helpers.Manager.clients[i].Character.Position.x >= (x - 50) && Helpers.Manager.clients[i].Character.Position.x <= ((x - 50) + 100) && Helpers.Manager.clients[i].Character.Position.y >= (y - 50) && Helpers.Manager.clients[i].Character.Position.y <= ((y - 50) + 100))
                                {
                                    Helpers.Manager.clients[i].client.Send(Client.Packet.SetSpeed(UniqueID, Speed1, Speed2));//Global Speed Update
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return Spawn;
        }

        public List<int> SpawnMe()
        {
            try
            {

                if (Model != 0)
                {
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            try
                            {
                                if (!Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                                {
                                    if (Helpers.Manager.clients[i].Character.Position.x >= (x - 50) && Helpers.Manager.clients[i].Character.Position.x <= ((x - 50) + 100) && Helpers.Manager.clients[i].Character.Position.y >= (y - 50) && Helpers.Manager.clients[i].Character.Position.y <= ((y - 50) + 100))
                                    {
                                        Spawn.Add(Helpers.Manager.clients[i].Character.Information.UniqueID);
                                        Helpers.Manager.clients[i].client.Send(Client.Packet.ObjectSpawn(this));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return Spawn;
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                Helpers.Manager.clients[i].client.Send(buff);
                            }
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
        }
        public void Send(PlayerMgr sys, byte[] buff)
        {
            try
            {
                if (Spawned(sys.Character.Information.UniqueID))
                {
                    sys.client.Send(buff);
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Send error: ", ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Client.Packet.ObjectDeSpawn(UniqueID);
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                Helpers.Manager.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                GenerateUniqueID.Delete(UniqueID);
                Helpers.Manager.HelperObject.Remove(this);
                Dispose();
            }
        }
        public void DeSpawnMe(bool t)
        {
            try
            {
                byte[] buff = Client.Packet.ObjectDeSpawn(UniqueID);
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                Helpers.Manager.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
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
    }
    public partial class spez_obj
    {
        public string Name;
        public int ID;
        public int UniqueID;
        public GenerateUniqueID Ids;
        public byte xSec, ySec;
        public double x, z, y;
        public short spezType;
        public int Radius; // radius for harmony
        public List<int> Inside = new List<int>();
        public List<int> Spawn = new List<int>();
        public Time timer;


        public struct Time
        {
            public Timer Delete;
            public Timer HarmonyBuff;
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

        protected void delete_callback(object e)
        {
            try
            {
                if (this != null)
                {
                    DeSpawnMe();
                    //RandomID.Delete(UniqueID);
                    Helpers.Manager.SpecialObjects.Remove(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        public void StopDeleteTimer()
        {
            if (timer.Delete != null)
            {
                timer.Delete.Dispose();
                timer.Delete = null;
            }
        }
        void StartDeleteTimer(int time)
        {
            if (timer.Delete == null) timer.Delete = new Timer(new TimerCallback(delete_callback), 0, time, 0);
        }

        public void SpawnMe(int duration)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            PlayerMgr sys = Helpers.Manager.clients[i];
                            if (x >= (sys.Character.Position.x - 50) && x <= ((sys.Character.Position.x - 50) + 100) && y >= (sys.Character.Position.y - 50) && y <= ((sys.Character.Position.y - 50) + 100) && Spawned(sys.Character.Information.UniqueID) == false)
                            {
                                Spawn.Add(sys.Character.Information.UniqueID);
                                sys.client.Send(Client.Packet.ObjectSpawn(this));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                }
                if (duration != 0) StartDeleteTimer(duration);

                if (Name.Contains("HARMONY")) { StartHarmonyBuff(); }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Client.Packet.ObjectDeSpawn(UniqueID);
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                Helpers.Manager.clients[i].client.Send(buff);

                                //end buff
                                if (Formule.GetSurroundRange(new ObjData.vektor(Helpers.Manager.clients[i].Character.Information.UniqueID, Helpers.Manager.clients[i].Character.Position.packetX, Helpers.Manager.clients[i].Character.Position.packetZ, Helpers.Manager.clients[i].Character.Position.packetY, Helpers.Manager.clients[i].Character.Position.xSec, Helpers.Manager.clients[i].Character.Position.ySec), new ObjData.vektor(0, (float)x, (float)z, (float)y, xSec, ySec), Radius))
                                {
                                    if (Helpers.Manager.clients[i].Character.Action.Buff.InfiniteBuffs.ContainsKey(Name))
                                    {
                                        Helpers.Manager.clients[i].SkillBuffEnd(Helpers.Manager.clients[i].Character.Action.Buff.InfiniteBuffs[Name]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                }

                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private void StartHarmonyBuff()
        {
            if (timer.HarmonyBuff != null) timer.HarmonyBuff.Dispose();
            timer.HarmonyBuff = new Timer(new TimerCallback(HarmonyBuff_callback), 0, 0, 500);
        }
        private void HarmonyBuff_callback(object e)
        {
            try
            {
                foreach (int p in Spawn)
                {
                    PlayerMgr s = Helpers.GetInformation.GetPlayer(p);

                    //double distance = Formule.gamedistance((float)x, (float)y, s.Character.Position.x, s.Character.Position.y);
                    double distance = Formule.gamedistance(this, s.Character.Position);
                    if (distance <= Radius && !s.Character.Action.Buff.InfiniteBuffs.ContainsKey(Name))
                    {
                        byte slot = s.SkillBuffGetFreeSlot();
                        if (slot == 255) return;

                        //add properties
                        foreach (KeyValuePair<string, int> a in ObjData.Manager.SkillBase[ID].Properties1)
                        {
                            if (s.SkillAdd_Properties(s, a.Key, true, slot)) { return; };
                        }

                        s.Character.Action.Buff.SkillID[slot] = ID;
                        s.Character.Action.Buff.OverID[slot] = s.Character.Ids.GetBuffID();
                        s.Character.Action.Buff.slot = slot;
                        s.Character.Action.Buff.count++;

                        s.Send(s.Character.Spawn, Client.Packet.SkillIconPacket(s.Character.Information.UniqueID, ID, s.Character.Action.Buff.OverID[s.Character.Action.Buff.slot], false));
                        s.Character.Action.Buff.InfiniteBuffs.Add(Name, s.Character.Action.Buff.slot);

                        Inside.Add(s.Character.Information.UniqueID);
                    }
                    else if (distance >= Radius && s.Character.Action.Buff.InfiniteBuffs.ContainsKey(Name))
                    {
                        if (Inside.Contains(s.Character.Information.UniqueID))
                        {
                            s.SkillBuffEnd(s.Character.Action.Buff.InfiniteBuffs[Name]);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }

    public sealed class targetObject
    {
        private float o_x, o_y, o_z;
        private double magdef, phydef;
        public Monsters os;
        public PlayerMgr sys, main;
        public bool type;
        private int id;
        private short absorbphy, absorbmag;
        private int hps;
        private byte xsec, ysec;
        private byte state;
        private double mabsrob;
        public targetObject(object o, PlayerMgr player)
        {
            try
            {
                os = null;
                o_x = 0;
                o_y = 0;
                o_z = 0;
                magdef = 0;
                phydef = 0;
                type = false;

                if (o == null) return;
                main = player;
                if (main == null) return;
                if (o.GetType().ToString() == "CLGameServer.WorldMgr.Monsters")
                {
                    os = o as Monsters;
                    if (os.Die) { player.StopAttackTimer(); return; }
                    o_x = (float)os.x;
                    o_y = (float)os.y;
                    o_z = (float)os.z;
                    xsec = os.xSec;
                    ysec = os.ySec;
                    magdef = ObjData.Manager.ObjectBase[os.ID].MagDef;
                    phydef = ObjData.Manager.ObjectBase[os.ID].PhyDef;
                    id = os.UniqueID;
                    type = false;
                    hps = os.HP;
                    state = os.State;
                    main.Character.Action.MonsterID.Add(os.UniqueID);
                    mabsrob = 0;
                    os.Target = player;
                }
                if (o.GetType().ToString() == "CLGameServer.PlayerMgr")
                {
                    sys = o as PlayerMgr;
                    o_x = sys.Character.Position.x;
                    o_y = sys.Character.Position.y;
                    o_z = sys.Character.Position.z;
                    xsec = sys.Character.Position.xSec;
                    ysec = sys.Character.Position.ySec;
                    magdef = sys.Character.Stat.MagDef;
                    phydef = sys.Character.Stat.PhyDef;
                    id = sys.Character.Information.UniqueID;
                    absorbphy = sys.Character.Stat.phy_Absorb;
                    absorbmag = sys.Character.Stat.mag_Absorb;
                    state = sys.Character.State.LastState;
                    hps = sys.Character.Stat.SecondHp;
                    type = true;
                    mabsrob = sys.Character.Stat.Absorb_mp;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void GetDead()
        {
            try
            {
                if (type)
                {
                    if (main.Character.Information.PvP && sys.Character.Information.PvP)
                        sys.Character.State.DeadType = 2;
                    else
                        sys.Character.State.DeadType = 1;
                    sys.BuffAllClose();
                    sys.Character.State.Die = true;
                }
                else
                {
                    os.Die = true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Sleep(byte types)
        {
            try
            {
                if (!type)
                {
                    Random rnd = new Random();
                    os.State = types;
                    os.StartObjeSleep(5000);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void MP(int mpdusur)
        {
            try
            {

                if (type)
                {
                    sys.Character.Stat.SecondMP -= mpdusur;
                    if (sys.Character.Stat.SecondMP < 0) sys.Character.Stat.SecondMP = 0;
                    sys.UpdateMp();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public byte HP(int hpdusur)
        {
            try
            {
                if (type)
                {
                    sys.Character.Stat.SecondHp -= hpdusur;
                    sys.UpdateHp();
                    if (sys.Character.Stat.SecondHp <= 0)
                    {
                        sys.Character.Stat.SecondHp = 0;
                        sys.Character.State.Die = true;
                        sys.BuffAllClose();
                        main.StopAttackTimer();
                        sys.StopAttackTimer();
                        sys.StopSkillTimer();
                        return 128;
                    }
                }
                else
                {
                    if (os != null)
                    {
                        os.CheckUnique();
                        if (!os.GetDie)
                            os.HP -= hpdusur;

                        os.AddAgroDmg(main.Character.Information.UniqueID, hpdusur);
                        os.CheckAgro();

                        main.GetBerserkOrb();

                        if (os.HP <= 0)
                        {
                            if (!os.GetDie)
                            {
                                PlayerMgr tg = (PlayerMgr)os.GetTarget();
                                if (tg.Character.Action.MonsterID != null && tg.Character.Action.MonsterIDCheck(os.UniqueID)) tg.Character.Action.MonsterID.Remove(os.UniqueID);
                                os.CheckUnique(tg.Character.Information.Name);
                                os.AttackStop();
                                os.StopMovement();
                                os.GetDie = true;
                                os.SetExperience();
                                os.MonsterDrop();
                                main.StopAttackTimer();
                                os.StartDeadTimer(4000);
                            }
                            return 128;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }
        public double MagDef
        {
            get { return magdef; }
        }
        public double PhyDef
        {
            get { return phydef; }
        }
        public float x
        {
            get { return o_x; }
        }
        public float z
        {
            get { return o_z; }
        }
        public float y
        {
            get { return o_y; }
        }
        public int ID
        {
            get { return id; }
        }
        public short AbsrobPhy
        {
            get { return absorbphy; }
        }
        public short AbsrobMag
        {
            get { return absorbmag; }
        }
        public int GetHp
        {
            get { return hps; }
        }
        public byte xSec
        {
            get { return xsec; }
        }
        public byte ySec
        {
            get { return ysec; }
        }
        public byte State
        {
            get { return state; }
        }
        public double mAbsorb()
        {
            if (type)
                return mabsrob;
            else return 0;
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public sealed partial class stall
    {
        public List<stallItem> ItemList;
        public List<int> Members = new List<int>();
        public List<SRClient> MembersClient = new List<SRClient>();

        public int ownerID;
        public bool isOpened;
        public string StallName;
        public string WelcomeMsg;

        public sealed class stallItem
        {
            public ObjData.slotItem Item;
            public ulong price;
            public byte stallSlot;
        }
        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void Send(byte[] buff, SRClient client)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                if (MembersClient[b] != client)
                    MembersClient[b].Send(buff);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public sealed partial class party
    {
        public byte Type, Race;
        public List<int> Members = new List<int>();
        public List<SRClient> MembersClient = new List<SRClient>();
        public int LeaderID;
        public bool IsFormed, InParty, SingleSend;
        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void UpdateCoordinate()
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                PlayerMgr s = Helpers.GetInformation.GetPlayer(Members[b]);
                MembersClient[b].Send(Client.Packet.Party_Data(6, Members[b]));
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public sealed partial class party
    {
        //Party name information
        public string partyname = "";
        //Party id
        public int ptid = 0;
        //Purpose
        public byte ptpurpose;
        //Minimal level
        public byte minlevel;
        //Maximal level
        public byte maxlevel;
    }
    public sealed partial class guild
    {
        public List<int> Members = new List<int>();
        public List<int> Unions = new List<int>();
        public List<int> UnionMembers = new List<int>();
        public List<SRClient> MembersClient = new List<SRClient>();
        public List<ObjData.guild_player> MembersInfo = new List<ObjData.guild_player>();

        public int GuildOwner, Guildid, UniqueUnion, UnionLeader;
        public long StorageGold;
        public string Name, GrantName;
        public int DonateGP, LastDonate;
        public byte FWrank, TotalMembers;
        public bool joinRight, withdrawRight, unionRight, guildstorageRight, noticeeditRight, SingleSend, UsingStorage, UnionActive;

        public byte Level, MaxMembers;
        public string NewsTitle, NewsMessage;
        public int PointsTotal, StorageSlots, Wargold;

        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
