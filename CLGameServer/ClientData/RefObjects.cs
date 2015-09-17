using CLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
namespace CLGameServer.ObjData
{
    public class Manager
    {
        public static item_database[] ItemBase = new item_database[40000];
        public static objectdata[] ObjectBase = new objectdata[39000];
        public static point[] PointBase = new point[250];
        public static cavepoint[] cavePointBase = new cavepoint[250];// Added for cave telepad locations
        public static s_data[] SkillBase = new s_data[35000];
        public static int[] MasteryBase = new int[111];
        public static levelgold[] LevelGold = new levelgold[141];
        public static List<JobLevel> Joblevelinfo = new List<JobLevel>(141);
        public static List<shop_data> ShopData = new List<shop_data>(500);
        public static long[] LevelData = new long[141];
        public static Dictionary<int, itemblue> ItemBlue = new Dictionary<int, itemblue>();
        public static List<MagicOption> MagicOptions = new List<MagicOption>(500);
        public static List<region> RegionBase = new List<region>(1954);
        public static List<region> safeZone = new List<region>(1000);
        public static List<region> Cave = new List<region>(50);
        public static List<CaveTeleports> CaveTeleports = new List<CaveTeleports>(80);
        public static List<TeleportPrice> TeleportPrice = new List<TeleportPrice>(800);
        public static reverse[] ReverseData = new reverse[43];
        public static Dictionary<short, SectorObject> MapObject = new Dictionary<short, SectorObject>();
        public static List<quest_database> QuestData = new List<quest_database>(900);
        //###########################################################
        // Drop databases
        //###########################################################
        public static drop_database SoxDataBase = new drop_database();
        public static drop_database StoneDataBase = new drop_database();
        public static drop_database ElixirDataBase = new drop_database();
        public static drop_database MaterialDataBase = new drop_database();
        public static drop_database ArmorDataBase = new drop_database();
        public static drop_database WeaponDataBase = new drop_database();
        public static drop_database EtcDatabase = new drop_database();
        public static drop_database JewelDataBase = new drop_database();
        public static Dictionary<string, drop_database> DropBase = new Dictionary<string, drop_database>();
        //###########################################################
        public static float[] AngleSin = new float[360];
        public static float[] AngleCos = new float[360];
        static Manager()
        {
            for (short i = 0; i < 360; i++)  // precalculated sin/cos tables for degrees 0-359 for speedup
            {
                AngleSin[i] = (float)Math.Sin(i * (Math.PI / 180));
                AngleCos[i] = (float)Math.Cos(i * (Math.PI / 180));
            }
            /*
			2*3.14*a/360
			2 = Çemberin İç Açılarının toplamının Radyan cinsindeki değeri
			3.14 = PI Değeri
			a = açı(yani o Anki Çizilen çemberin herhangi bir kısmındaki açısı)
			360 = çemberin iç açılarının toplamı
			for(int a=0;a<360;a++)
			{
			cemberaci = 2*3.14*a/360;
			cember.X = Math.Sin(cemberaci);
			cember.Y = Math.Cos(cemberaci);
			cemberolustur(cember.X, cember.Y);
			}
			*/
        }
    }
    public class MagicOption
    {
        public int ID { get; set; }

        public string[] ApplicableOn = new string[4];

        public string Name { get; set; }

        public int Level { get; set; }

        public double OptionPercent { get; set; }

        public string Type { get; set; }

        public static string[] PossibleBluesOnDrop = new string[5] { "MATTR_INT", "MATTR_STR", "MATTR_LUCK", "MATTR_HP", "MATTR_MP" };
        public int MinValue, MaxValue;
    }
    public static class SpawnData
    {
        public static List<int> TempSpawn;
    }
    public sealed class trader_data
    {
        public int Amount;
        public int Stars;
        public stars Details;
        public enum stars
        {
            ZEROSTARS = 0,
            ONESTAR = 1,
            TWOSTARS = 2,
            THREESTARS = 3,
            FOURSTARS = 4,
            FIVESTARS = 5
        }
    }
    public struct guild_player
    {
        public int MemberID, Model, DonateGP;
        public string Name, GrantName;
        public byte Level, FWrank, Rank;
        public byte Xsector, Ysector;
        public bool Online;
        public bool joinRight, withdrawRight, unionRight, guildstorageRight, noticeeditRight;
    }
    public class GuildUniqueList
    {
        public string GuildUnique;
    }
    public sealed class drop_database
    {
        public List<int> ID;

        public drop_database()
        {
            ID = new List<int>();
        }
        public byte GetQuantity(byte mobType, string itemType)
        {
            byte Quantity = 0;
            switch (itemType)
            {
                case "armors":
                case "weapons":
                case "jewelery":
                case "sox":
                case "tablets":
                case "elixir":
                case "potions":
                case "scrolls":
                case "alchemymaterial":
                    Quantity = 1;
                    break;
                case "arrows":
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.ETC)) Quantity = Convert.ToByte(150 / Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.ETC)) Quantity = Convert.ToByte(200 / Rnd.Next(1, 3));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.ETC)) Quantity = Convert.ToByte(100 / Rnd.Next(1, 2));
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.ETC)) Quantity = 50;
                    if (Quantity == 1) Quantity = 50;
                    break;
            }
            return Quantity;
        }
        public int GetAmount(byte mobType, string itemType)
        {
            byte Amountinfo = 0;
            switch (itemType)
            {
                case "sox"://Seal drops (Should be defined per seal type).
                    #region Seal Items
                    if (mobType == 4 && (Rnd.Next(0, 400) < 3 * Helpers.Settings.Rate.ItemSox)) Amountinfo = 1;
                    if (mobType == 3 && (Rnd.Next(0, 400) < 5 * Helpers.Settings.Rate.ItemSox)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                    if (mobType == 1 && (Rnd.Next(0, 400) < 2 * Helpers.Settings.Rate.ItemSox)) Amountinfo = 1;
                    if (mobType == 0 && (Rnd.Next(0, 400) < 2 * Helpers.Settings.Rate.ItemSox)) Amountinfo = 1;
                    #endregion
                    break;
                case "tablets"://Tablets (Defined per degree / Level drop).
                    #region Alchemy Tablets
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.Alchemy)) Amountinfo = 1;
                    #endregion
                    break;
                case "elixir"://Elixir drops speak for itself.
                    #region Elixirs
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.Alchemy)) Amountinfo = 1;
                    #endregion
                    break;
                case "alchemymaterial"://Etc drops would contains (Potions, Arrows, Material etc).
                    #region Alchemy Materials
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(2, 4));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(4, 6));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.Alchemy)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.Alchemy)) Amountinfo = 1;
                    #endregion
                    break;
                case "arrows"://This contains bolts and arrows).
                    #region Ammos
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 3));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.ETC)) Amountinfo = 1;
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.ETC)) Amountinfo = 1;
                    #endregion
                    break;
                case "potions"://Potions (Enough said).
                    #region Potions
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(4, 9));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.ETC)) Amountinfo = 1;
                    #endregion
                    break;
                case "event_items"://Event items (Letters, scrolls etc Should add in config later for event handler.).
                    break;
                case "quest_items"://Quest items (Will be called from the quest active list (id of drops).
                    break;
                case "scrolls"://Return scrolls (And related to that).
                    #region return scrolls
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.ETC)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.ETC)) Amountinfo = 1;
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.ETC)) Amountinfo = 1;
                    #endregion
                    break;
                case "jewelery":
                case "weapons"://Weapon drops(speaks for itself).
                case "armors"://Armor drops (Garm, Prot etc).
                    #region Normal Items
                    if (mobType == 4 && (Rnd.Next(0, 300) < 7 * Helpers.Settings.Rate.Item)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 3 && (Rnd.Next(0, 300) < 300 * Helpers.Settings.Rate.Item)) Amountinfo = Convert.ToByte(Rnd.Next(1, 2));
                    if (mobType == 1 && (Rnd.Next(0, 300) < 5 * Helpers.Settings.Rate.Item)) Amountinfo = 1;
                    if (mobType == 0 && (Rnd.Next(0, 300) < 3 * Helpers.Settings.Rate.Item)) Amountinfo = 1;
                    #endregion
                    break;
            }
            return Amountinfo;
        }
        public byte GetSpawnType(string itemType)
        {
            byte type = 0;
            switch (itemType)
            {
                case "tablets":
                case "elixir":
                case "potions":
                case "scrolls":
                case "alchemymaterial":
                    type = 3;
                    break;
                case "armors":
                case "weapons":
                case "jewelery":
                case "sox":
                    type = 2;
                    break;
                case "arrows":
                    type = 3;
                    break;
            }
            return type;
        }
        public int GetDrop(int moblevel, int mobID, string itemType, int filterRace)
        {
            List<int> filter = new List<int>();
            switch (itemType)
            {
                case "tablets"://Tablets (Defined per degree / Level drop).
                    #region tablets
                    if (ObjData.Manager.ObjectBase[mobID].Level < 8)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 1);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 16 && ObjData.Manager.ObjectBase[mobID].Level >= 8)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 2);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 24 && ObjData.Manager.ObjectBase[mobID].Level >= 16)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 3);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 32 && ObjData.Manager.ObjectBase[mobID].Level >= 24)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 4);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 42 && ObjData.Manager.ObjectBase[mobID].Level >= 32)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 5);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 52 && ObjData.Manager.ObjectBase[mobID].Level >= 42)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 6);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 64 && ObjData.Manager.ObjectBase[mobID].Level >= 52)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 7);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 76 && ObjData.Manager.ObjectBase[mobID].Level >= 64)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 8);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 90 && ObjData.Manager.ObjectBase[mobID].Level >= 76)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 9);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 101 && ObjData.Manager.ObjectBase[mobID].Level >= 90)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 10);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level > 101)
                    {
                        filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Degree == 11);
                    }
                    #endregion
                    break;
                case "elixir"://Elixir drops speak for itself.
                    #region elixir
                    filter = ID;
                    #endregion  
                    break;
                case "alchemymaterial"://Etc drops would contains (Potions, Arrows, Material etc).
                    #region Alchemy materials
                    string truncatedMobName = ObjData.Manager.ObjectBase[mobID].Name.Substring(ObjData.Manager.ObjectBase[mobID].Name.IndexOf("_") + 1);
                    filter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Name.Contains(truncatedMobName));
                    #endregion
                    break;
                case "arrows"://This contains bolts and arrows (Define eu/ch happens before).
                    #region Arrows
                    if (ObjData.Manager.ObjectBase[mobID].Type == 1)
                    {
                        filter.Add(62); // arrow
                    }
                    else
                    {
                        filter.Add(10376); // bolt
                    }
                    #endregion
                    break;
                case "potions"://Potions (+unipills)
                    #region Potions
                    filter.Add(9);
                    filter.Add(16);
                    filter.Add(23);

                    if (ObjData.Manager.ObjectBase[mobID].Level < 20)
                    {
                        filter.Add(4);
                        filter.Add(11);
                        filter.Add(18);
                        filter.Add(55);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 40 && ObjData.Manager.ObjectBase[mobID].Level >= 20)
                    {
                        filter.Add(5);
                        filter.Add(12);
                        filter.Add(19);
                        filter.Add(56);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 60 && ObjData.Manager.ObjectBase[mobID].Level >= 40)
                    {
                        filter.Add(6);
                        filter.Add(13);
                        filter.Add(20);
                        filter.Add(57);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level < 80 && ObjData.Manager.ObjectBase[mobID].Level >= 60)
                    {
                        filter.Add(7);
                        filter.Add(14);
                        filter.Add(21);
                        filter.Add(58);
                    }
                    else if (ObjData.Manager.ObjectBase[mobID].Level >= 80)
                    {
                        filter.Add(8);
                        filter.Add(15);
                        filter.Add(22);
                        filter.Add(59);
                    }
                    #endregion
                    break;
                case "event_items"://Event items (Letters, scrolls etc Should add in config later for event handler.)
                    #region Event items
                    // todo: event system 
                    #endregion
                    break;
                case "quest_items"://Quest items (Will be called from the quest active list (id of drops).
                    #region Quest related items
                    // todo: quest system 
                    #endregion
                    break;
                case "scrolls"://Return scrolls (And related to that).
                    #region Scrolls
                    filter.Add(61);
                    #endregion
                    break;
                case "weapons"://Weapon drops(speaks for itself).
                case "armors"://Armor drops (Garm, Prot etc).
                case "sox"://Seal drops (Should be defined per seal type).
                case "jewelery":
                    #region Normal wearable items
                    List<int> LevelFilter = ID.FindAll(item => ObjData.Manager.ItemBase[item].Level >= ObjData.Manager.ObjectBase[mobID].Level - 4 && ObjData.Manager.ItemBase[item].Level <= ObjData.Manager.ObjectBase[mobID].Level + 4);
                    filter = LevelFilter.FindAll(item => ObjData.Manager.ItemBase[item].Race == filterRace);
                    #endregion
                    break;
            }
            if (filter.Count <= 0)
                return -1;
            else
                return filter[Rnd.Next(0, filter.Count - 1)];
        }
    }
    public sealed class quest_database
    {
        public string Questname, Rewardname, QuestNPC;
        public int Questid, Rewardid, QuestNpcID, QuestLevel;
        public enum requirments
        {
            LEVEL,
            RACE,
            ITEMS
        }
    }
    public sealed class item_database
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < ObjData.Manager.ItemBase.Length; i++)
            {
                if (ObjData.Manager.ItemBase[i] != null && ObjData.Manager.ItemBase[i].Name == name) return i;
            }
            return 0;
        }

        public int ID;
        public byte Level, SOX, Gender, Race, SoulBound, SealType, MaxBlueAmount, Degree, Accountbound, Tradable;
        public int TypeID3, TypeID4, TypeID1, Shop_price, Item_Mall_Type, Max_Stack, Use_Time, Use_Time2, Sell_Price, TypeID2, Storage_price, Armorinfo, ItemMallType, SkillID, EARTH_ELEMENTS_AMOUNT_REQ, WATER_ELEMENTS_AMOUNT_REQ, FIRE_ELEMENTS_AMOUNT_REQ, WIND_ELEMENTS_AMOUNT_REQ;
        public double ATTACK_DISTANCE;
        public string Name, ObjectName, StoneName, EARTH_ELEMENTS_NAME, WATER_ELEMENTS_NAME, FIRE_ELEMENTS_NAME, WIND_ELEMENTS_NAME;
        public bool Equip = false;
        public ItemType Itemtype;
        public enum ItemType { WEARABLE, CH_SHIELD, EU_SHIELD, AMMO, HAT, TROUSERS, HANDS, SHOES, SUIT, SHOULDER, COS, EARRING, RING, NECKLACE, SWORD, BLADE, GLAVIE, SPEAR, BOW, TRADERSUIT, HUNTERSUIT, THIEFSUIT, ALCHEMY, AVATAR, EVENT, EU_SWORD, EU_TSWORD, EU_AXE, EU_DARKSTAFF, EU_TSTAFF, EU_CROSSBOW, EU_DAGGER, EU_HARP, EU_STAFF, FORTRESS, MAGICSTONE, ATTRSTONE, MAGICTABLET, ATTRTABLET, ARROW, BOLT };
        public ArmorType Type;
        public enum ArmorType { NULL, ARMOR, PROTECTOR, GARMENT, ROBE, HEAVY, LIGHT, GM, AVATAR, AVATARATTACH, AVATARHAT, THIEF, HUNTER };
        public EtcType Etctype;
        public enum EtcType
        {
            NULL, ITEMMALL, HP_POTION, MP_POTION, VIGOR_POTION, STALLDECORATION, MONSTERMASK, ELIXIR, RETURNSCROLL, REVERSESCROLL, BANDITSCROLL, SUMMONSCROLL, INVENTORYEXPANSION, GLOBALCHAT,
            WAREHOUSE, CHANGESKIN, HPSTATPOTION, MPSTATPOTION, BERSERKPOTION, AVATAR28D, SPEED_POTION, ALCHEMY_MATERIAL, ELEMENTS, DESTROYER_RONDO, VOID_RONDO, ITEMCHANGETOOL, STONES, ASTRALSTONE,
            GUILD_ICON
        };
        public PetType Pettype;
        public enum PetType { NULL, GRABPET, ATTACKPET, TRANSPORT, JOBTRANSPORT };
        public QuestType Questtype;
        public enum QuestType { QUEST };
        public Tickets Ticket;
        public enum Tickets
        {
            SILVER_1_DAY, SILVER_4_WEEKS, SILVER_8_WEEKS, SILVER_12_WEEKS, SILVER_16_WEEKS,
            GOLD_1_DAY, GOLD_4_WEEKS, GOLD_8_WEEKS, GOLD_12_WEEKS, GOLD_16_WEEKS,
            SKILL_SILVER_1_DAY, SKILL_SILVER_4_WEEKS, SKILL_SILVER_8_WEEKS, SKILL_SILVER_12_WEEKS, SKILL_SILVER_16_WEEKS,
            SKILL_GOLD_1_DAY, SKILL_GOLD_4_WEEKS, SKILL_GOLD_8_WEEKS, SKILL_GOLD_12_WEEKS, SKILL_GOLD_16_WEEKS,
            BEGINNER_HELPERS,//Temp will make them better when they work.
            PREMIUM_QUEST_TICKET,
            OPEN_MARKET,
            DUNGEON_EGYPT,
            DUNGEON_FORGOTTEN_WORLD,
            BATTLE_ARENA,
            WAREHOUSE,
            AUTO_POTION
        };
        attack_items attack = new attack_items();
        public attack_items Attack { get { return attack; } }

        def_items def = new def_items();
        public def_items Defans { get { return def; } }
    }
    public sealed class attack_items
    {
        public double Min_LPhyAttack, Min_HPhyAttack, PhyAttackInc, Min_LMagAttack, Min_HMagAttack, MagAttackINC, MinAttackRating, MaxAttackRating;
        public byte MinCrit, MaxCrit;
    }
    public sealed class def_items
    {
        public double MinMagDef, MagDefINC, MinPhyDef, PhyDefINC;
        public double PhyAbsorb, MagAbsorb, AbsorbINC, Durability, Parry;
        public byte MinBlock, MaxBlock;
    }
    public sealed class objectdata
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < ObjData.Manager.ObjectBase.Length; i++)
            {
                if (ObjData.Manager.ObjectBase[i] != null && ObjData.Manager.ObjectBase[i].Name == name) return i;
            }
            return 0;
        }
        public int ID;
        public string Name;
        public int HP, Exp;
        public int[] Skill = new int[9];
        public byte amountSkill;
        public int MagDef, PhyDef, ParryRatio, HitRatio;
        public byte Agresif, Type, ObjectType, Level, Race;
        public float SpeedWalk, SpeedRun, SpeedZerk;
        public string[] Shop = new string[10];
        public string[] Tab = new string[30];
        public string StoreName;
        public float Speed1, Speed2;
    }
    public class vektor
    {
        public float x, y, z;
        public byte xSec, ySec;
        public int ID;
        public vektor(int SetUniqueID, float SetXPosition, float SetZPosition, float SetYPosition, byte SetxSection, byte SetySection)
        {
            x = SetXPosition;
            z = SetZPosition;
            y = SetYPosition;
            xSec = SetxSection;
            ySec = SetySection;
            ID = SetUniqueID;
        }
        public vektor() : this(0,0,0,0,0,0)
        {
            // Default Values
        }
    }
    public sealed class slotItem
    {
        public int ID, dbID, Durability, BlueStr;
        public byte PlusValue, Type, Slot;
        public short Amount = 1;
    }
    public sealed class point
    {
        public double x, z, y;
        public byte xSec, ySec, test;
        public int ID, Number, Price;
        public string Name;
    }
    public sealed class cavepoint // Added for cave telepad locations
    {
        public double x, z, y;
        public byte xSec, ySec, test;
        public int ID, Number, Price;
        public string Name;
    }
    public class s_data
    {
        public enum Definedtype
        {
            Imbue, Buff, Attack
        }
        public enum RadiusTypes
        { FRONTRANGERADIUS = 1, SURROUNDRANGERADIUS = 2, TRANSFERRANGE = 3, PENETRATION = 4, PENETRATIONRANGED = 5, MULTIPLETARGET = 6, ONETARGET = 7 };

        public enum SkillTypes
        { PASSIVE = 0, ACTIVE = 2, IMBUE = 1 };

        public enum ItemTypes
        { SHIELD, EUSHIELD, BOW, ONEHANDED, TWOHANDED, AXE, WARLOCKROD, CLERICROD, STAFF, XBOW, DAGGER, BARD, LIGHTARMOR, DEVILSPIRIT };

        public enum TargetTypes
        { MOB = 0x001, PLAYER = 0x010, NOTHING = 0x100 };

        public Dictionary<string, int> Properties1, Properties2, Properties3, Properties4, Properties5, Properties6;
        public List<ItemTypes> ReqItems;
        public List<summon_data> SummonList;
        public int MinAttack, MaxAttack, PhyPer, Action_ReuseDelay, Per, efrUnk1;
        public int ID, SkillPoint, NextSkill, tmpProp, Mana, MagPer, CastingTime, Action_PreparingTime, Action_CastingTime, Action_ActionDuration, Action_CoolTime, Action_FlyingSpeed, AmountEffect, Time, Distance, SimultAttack, AttackCount;
        public short Mastery;
        public byte sType;
        public string Name, Series;
        public byte Weapon1, Weapon2;
        public bool isAttackSkill;
        public RadiusTypes RadiusType;
        public SkillTypes SkillType;
        public Definedtype Definedname;
        public TargetTypes TargetType;
        public bool canSelfTargeted;
        public bool needPVPstate;

        public struct summon_data
        {
            public int ID;
            public byte Type, MinSummon, MaxSummon;
        }
        public s_data()
        {
            SummonList = new List<summon_data>();
            ReqItems = new List<ItemTypes>();
            Properties1 = new Dictionary<string, int>();
            Properties2 = new Dictionary<string, int>();
            Properties3 = new Dictionary<string, int>();
            Properties4 = new Dictionary<string, int>();
            Properties5 = new Dictionary<string, int>();
            Properties6 = new Dictionary<string, int>();
        }
    }
    public class JobLevel
    {
        public byte level, jobtitle;
        public ulong exp;
    }
    public class levelgold
    {
        public short min, max;
    }
    public class shop_data
    {
        public string tab;
        public string[] Item = new string[300];
        public static shop_data GetShopIndex(string name)
        {
            
            shop_data result = Manager.ShopData.Find(delegate (shop_data bk)
            {
                return bk.tab == name;
            }
                    );
            return result;
            //if (result == id) return true;
            /*for (int i = 0; i < ObjData.Manager.ShopData.Count; i++)
            {
                if (ObjData.Manager.ShopData[i].tab == name) return ObjData.Manager.ShopData[i];
            }
            return null;*/
        }
    }
    public class itemblue
    {
        public int totalblue = 0;
        public ArrayList blue;
        public ArrayList blueamount;
    }
    public sealed class region
    {
        public int ID;
        public int SecX;
        public int SecY;
        public string Name;
    }
    public class CaveTeleports
    {
        public string name;
        public byte xsec, ysec;
        public double x, z, y;
    }
    public class reverse
    {
        public int ID;
        public short area;
        public double x, z, y;
        public byte xSec, ySec;
    }
    public class TeleportPrice
    {
        public int price;
        public int ID;
        public int level;
    }
    public sealed class SectorObject
    {
        public struct n7nEntity
        {
            public struct sPoint
            {
                public float x, y, z;
            };

            public struct sLine
            {
                public short PointA, PointB;
                public byte flag;
            };

            public byte ObjectMapflag;
            public sPoint Position;
            public List<sPoint> Points;
            public List<sLine> OutLines;
        }

        public float GetHeightAt(float x, float y)
        {
            return heightmap[(int)y * 97 + (int)x];
        }

        public float[] heightmap = new float[9409];
        public int entityCount;
        public List<n7nEntity> entitys = new List<n7nEntity>();
    }
}
