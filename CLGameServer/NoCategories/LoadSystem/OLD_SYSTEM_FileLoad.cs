using System;
using CLFramework;
using System.IO;
using System.Collections.Generic;
using CLGameServer.ObjData;
namespace CLGameServer
{
    public class FileDB
    {
        public static void Load()
        {
            LoadSkillData(@"\data\skilldata_5000.txt");
            LoadSkillData(@"\data\skilldata_10000.txt");
            LoadSkillData(@"\data\skilldata_15000.txt");
            LoadSkillData(@"\data\skilldata_20000.txt");
            LoadSkillData(@"\data\skilldata_25000.txt");
            LoadSkillData(@"\data\skilldata_30000.txt");
            LoadSkillData(@"\data\skilldata_35000.txt");
            SetTimerItems(@"\data\refrentitem.txt");
            ItemDatabase(@"\data\itemdata_5000.txt");
            ItemDatabase(@"\data\itemdata_10000.txt");
            ItemDatabase(@"\data\itemdata_15000.txt");
            ItemDatabase(@"\data\itemdata_20000.txt");
            ItemDatabase(@"\data\itemdata_25000.txt");
            ItemDatabase(@"\data\itemdata_30000.txt");
            ItemDatabase(@"\data\itemdata_35000.txt");
            ItemDatabase(@"\data\itemdata_40000.txt");
            ObjectDataBase(@"\data\characterdata_5000.txt");
            ObjectDataBase(@"\data\characterdata_10000.txt");
            ObjectDataBase(@"\data\characterdata_15000.txt");
            ObjectDataBase(@"\data\characterdata_20000.txt");
            ObjectDataBase(@"\data\characterdata_25000.txt");
            ObjectDataBase(@"\data\characterdata_30000.txt");
            ObjectDataBase(@"\data\characterdata_35000.txt");
            ObjectDataBase(@"\data\characterdata_40000.txt");
            LoadDrops();
            LoadNpcs();
            LoadObject();
            LoadQuestData(@"\data\questdata.txt");
            LoadCaveTeleports();
            cavedata();
            TeleportBuilding();
            LoadCaveTeleports();
            LoadCaves();
            LoadTeleportPrice();
            LoadRegionCodes();
            LoadMasteryData();
            LoadGoldData();
            LoadJobLevels();
            LoadLevelData();
            LoadShopTabData();
            LoadMagicOptions();
            ReverseData();
            //LoadMapObject("mapobject.dat");
            Console.WriteLine("############ Done loading ############");
            Console.WriteLine("Server is now online...");
        }
        public static void LoadDrops()
        {
            #region Databases
            Manager.DropBase.Add("alchemymaterial", Manager.MaterialDataBase);
            Manager.DropBase.Add("sox", Manager.SoxDataBase);
            Manager.DropBase.Add("armors", Manager.ArmorDataBase);
            Manager.DropBase.Add("weapons", Manager.WeaponDataBase);
            Manager.DropBase.Add("jewelery", Manager.JewelDataBase);
            Manager.DropBase.Add("tablets", Manager.StoneDataBase);
            Manager.DropBase.Add("elixir", Manager.ElixirDataBase);
            Manager.DropBase.Add("arrows", Manager.EtcDatabase);
            Manager.DropBase.Add("potions", Manager.EtcDatabase);
            Manager.DropBase.Add("scrolls", Manager.EtcDatabase);
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////
        // Item database
        /////////////////////////////////////////////////////////////////////////////
        public static void ItemDatabase(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            int id = 0;
            string s1 = null;
            
            //###########################################################
            // Begin item database
            //###########################################################
            for (int t = 0; t <= TxtFile.amountLine - 1; t++)
            {
                //Load from file
                #region Load file info
                s1 = TxtFile.lines[t].ToString();
                TxtFile.commands = s1.Split('\t');
                item_database it = new item_database();
                id = Convert.ToInt32(TxtFile.commands[1]);
                it.Name = TxtFile.commands[2];
                it.ID = Convert.ToInt32(id);
                it.TypeID1 = Convert.ToInt32(TxtFile.commands[9]);
                it.TypeID2 = Convert.ToInt32(TxtFile.commands[10]);
                it.TypeID3 = Convert.ToInt32(TxtFile.commands[11]);
                it.TypeID4 = Convert.ToInt32(TxtFile.commands[12]);
                it.Race = Convert.ToByte(TxtFile.commands[14]);
                it.SOX = Convert.ToByte(TxtFile.commands[15]);
                it.SoulBound = Convert.ToByte(TxtFile.commands[18]);
                it.Shop_price = Convert.ToInt32(TxtFile.commands[26]);
                it.Storage_price = Convert.ToInt32(TxtFile.commands[30]);
                it.Sell_Price = Convert.ToInt32(TxtFile.commands[31]);
                it.Level = Convert.ToByte(TxtFile.commands[33]);
                it.Max_Stack = Convert.ToInt32(TxtFile.commands[57]);
                it.Gender = Convert.ToByte(TxtFile.commands[58]);
                it.Degree = Convert.ToByte(TxtFile.commands[61]);
                //Stone related (Stone creation)
                #region Stone creation
                it.EARTH_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[118]);
                it.EARTH_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[119]);
                it.WATER_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[120]);
                it.WATER_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[121]);
                it.FIRE_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[122]);
                it.FIRE_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[123]);
                it.WIND_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[124]);
                it.WIND_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[125]);
                #endregion
                it.Defans.Durability = Convert.ToDouble(TxtFile.commands[63].Replace('.', ','));
                it.Defans.MinPhyDef = Convert.ToDouble(TxtFile.commands[65].Replace('.', ','));
                it.Defans.PhyDefINC = Convert.ToDouble(TxtFile.commands[67].Replace('.', ','));
                it.Defans.Parry = Convert.ToDouble(TxtFile.commands[68].Replace('.', ','));
                it.Defans.MinBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[74].Replace('.', ','))));
                it.Defans.MaxBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[75].Replace('.', ','))));
                it.Defans.MinMagDef = Convert.ToDouble(TxtFile.commands[76].Replace('.', ','));
                it.Defans.MagDefINC = Convert.ToDouble(TxtFile.commands[78].Replace('.', ','));
                it.Defans.PhyAbsorb = Convert.ToDouble(TxtFile.commands[79].Replace('.', ','));
                it.Defans.MagAbsorb = Convert.ToDouble(TxtFile.commands[80].Replace('.', ','));
                it.Defans.AbsorbINC = Convert.ToDouble(TxtFile.commands[81].Replace('.', ','));
                it.Equip = Convert.ToBoolean(Convert.ToByte(TxtFile.commands[93]));
                it.ATTACK_DISTANCE = Convert.ToInt16(TxtFile.commands[94]);
                it.Attack.Min_LPhyAttack = Convert.ToDouble(TxtFile.commands[95].Replace('.', ','));
                it.Attack.Min_HPhyAttack = Convert.ToDouble(TxtFile.commands[97].Replace('.', ','));
                it.Attack.PhyAttackInc = Convert.ToDouble(TxtFile.commands[99].Replace('.', ','));
                it.Attack.Min_LMagAttack = Convert.ToDouble(TxtFile.commands[100].Replace('.', ','));
                it.Attack.Min_HMagAttack = Convert.ToDouble(TxtFile.commands[102].Replace('.', ','));
                it.Attack.MagAttackINC = Convert.ToDouble(TxtFile.commands[104].Replace('.', ','));
                it.Attack.MinAttackRating = Convert.ToDouble(TxtFile.commands[113].Replace('.', ','));
                it.Attack.MaxAttackRating = Convert.ToDouble(TxtFile.commands[114].Replace('.', ','));
                it.Attack.MinCrit = Convert.ToByte(Convert.ToDouble(TxtFile.commands[116].Replace('.', ',')));
                it.Attack.MaxCrit = Convert.ToByte(Convert.ToDouble(TxtFile.commands[117].Replace('.', ',')));
                it.ObjectName = TxtFile.commands[119];
                it.Use_Time = Convert.ToInt32(TxtFile.commands[118]);
                it.Use_Time2 = Convert.ToInt32(TxtFile.commands[122]);
                it.MaxBlueAmount = Convert.ToByte(TxtFile.commands[158]);
                // additional infos todo: structure parse
                if(it.ObjectName.Contains("SKILL"))
                foreach(s_data sd in Manager.SkillBase)
                {
                    if (sd != null)
                    if (sd.Name == it.ObjectName)
                    {
                        it.SkillID = sd.ID;
                        break;
                    }
                }
                Manager.ItemBase[id] = it;
                #endregion
                //Item slot types all ot
                #region Item Slot Types
                if (it.Name.Contains("SHIELD") && it.Name.Contains("EU") && !it.Name.Contains("ETC"))
				{
                    it.Itemtype = item_database.ItemType.EU_SHIELD; 
					it.Equip = true;
				}
                else if (it.Name.Contains("SHIELD") && it.Name.Contains("CH") && !it.Name.Contains("ETC"))
				{
                    it.Itemtype = item_database.ItemType.CH_SHIELD;
					it.Equip = true;
				}
                else if (it.TypeID3 == 4 && it.TypeID4 == 1 && it.TypeID2 == 3)
				{
                    it.Itemtype = item_database.ItemType.ARROW;
				}
				else if (it.TypeID3 == 4 && it.TypeID4 == 2 && it.TypeID2 == 3)
				{
					it.Itemtype = item_database.ItemType.BOLT;
				}
				else if (it.TypeID3 == 1 && it.TypeID4 == 1 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.HAT;
					it.Equip = true;
				}
				else if (it.TypeID3 == 3 && it.TypeID4 == 2 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.SHOULDER;
					it.Equip = true;
				}
				else if (it.TypeID3 == 3 && it.TypeID4 == 3 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.SUIT;
					it.Equip = true;
				}
				else if (it.TypeID3 == 3 && it.TypeID4 == 4 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.TROUSERS;
					it.Equip = true;
				}
				else if (it.TypeID3 == 3 && it.TypeID4 == 5 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.HANDS;
					it.Equip = true;
				}
				else if (it.TypeID3 == 3 && it.TypeID4 == 6 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.SHOES;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 7 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_SWORD;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 8 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_TSWORD;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 9 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_AXE;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 10 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_DARKSTAFF;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 11 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_TSTAFF;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 12 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_CROSSBOW;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 13 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_DAGGER;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 14 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_HARP;
					it.Equip = true;
				}
				else if (it.TypeID3 == 6 && it.TypeID4 == 15 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EU_STAFF;
					it.Equip = true;
				}
				else if (it.TypeID3 == 5 && it.TypeID4 == 1 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.EARRING;
					it.Equip = true;
				}
				else if (it.TypeID3 == 5 && it.TypeID4 == 3 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.RING;
					it.Equip = true;
				}
				else if (it.TypeID3 == 5 && it.TypeID4 == 2 && it.TypeID2 == 1)
				{
					it.Itemtype = item_database.ItemType.NECKLACE;
					it.Equip = true;
				}
				else if (it.TypeID1 == 3 && it.TypeID4 == 2 && it.TypeID2 == 1 && it.TypeID3 == 6)
				{
					it.Itemtype = item_database.ItemType.SWORD;
					it.Equip = true;
				}
				else if (it.TypeID1 == 3 && it.TypeID4 == 3 && it.TypeID2 == 1 && it.TypeID3 == 6)
				{
					it.Itemtype = item_database.ItemType.BLADE;
					it.Equip = true;
				}
				else if (it.TypeID1 == 3 && it.TypeID4 == 4 && it.TypeID2 == 1 && it.TypeID3 == 6)
				{
					it.Itemtype = item_database.ItemType.SPEAR;
					it.Equip = true;
				}
				else if (it.TypeID1 == 3 && it.TypeID4 == 5 && it.TypeID2 == 1 && it.TypeID3 == 6)
				{
					it.Itemtype = item_database.ItemType.GLAVIE;
					it.Equip = true;
				}
				else if (it.TypeID1 == 3 && it.TypeID4 == 6 && it.TypeID2 == 1 && it.TypeID3 == 6)
				{
					it.Itemtype = item_database.ItemType.BOW;
					it.Equip = true;
				}
                #endregion
                //Item armor type:
                #region Item Armor Types
                if (it.Name.Contains("LIGHT") && it.Name.Contains("EU"))
				{
					it.Equip = true;
					it.Type = item_database.ArmorType.LIGHT;
				}

				else if (it.Name.Contains("LIGHT") && it.Name.Contains("CH"))
				{ 
					it.Equip = true;
					it.Type = item_database.ArmorType.PROTECTOR; 
				}
				else if (it.Name.Contains("HEAVY") && it.Name.Contains("EU"))
				{ 
					it.Equip = true;
					it.Type = item_database.ArmorType.HEAVY; 
				}
				else if (it.Name.Contains("HEAVY") && it.Name.Contains("CH"))
				{ 
					it.Equip = true;
					it.Type = item_database.ArmorType.ARMOR; 
				}
				else if (it.Name.Contains("CLOTHES") && it.Name.Contains("EU"))
				{ 
					it.Equip = true;
					it.Type = item_database.ArmorType.ROBE; 
				}
				else if (it.Name.Contains("CLOTHES") && it.Name.Contains("CH"))
				{ 
					it.Equip = true;
					it.Type = item_database.ArmorType.GARMENT; 
				}
				else if (it.Name.Contains("C_SUPER"))
				{ 
					it.Equip = true; 
					it.Type = item_database.ArmorType.GM;
				}
				#endregion
				//Jewelry
				#region Jewelry
				else if (it.Name.Contains("_RING_") && !it.Name.Contains("ETC"))
					it.Itemtype = item_database.ItemType.RING;
				else if (it.Name.Contains("_EARRING_") && !it.Name.Contains("ETC"))
					it.Itemtype = item_database.ItemType.EARRING;
				else if (it.Name.Contains("_NECK") && !it.Name.Contains("ETC"))
					it.Itemtype = item_database.ItemType.NECKLACE;
				#endregion
				//Grabpets
				#region Grabpet
				else if (it.Name.Contains("COS_P") && it.TypeID1 == 3 && it.TypeID3 == 1 && it.TypeID4 == 2 && it.TypeID2 == 2)
					it.Pettype = item_database.PetType.GRABPET;
				#endregion
				//Attackpets
				#region Attack pets
				else if (it.Name.Contains("COS_P") && it.TypeID1 == 3 && it.TypeID3 == 1 && it.TypeID4 == 1 && it.TypeID2 == 2)
					it.Pettype = item_database.PetType.ATTACKPET;
				#endregion
				//Transport horses
				#region Transport horses
				else if (it.Name.Contains("COS_T"))
					it.Pettype = item_database.PetType.JOBTRANSPORT;
				#endregion
				//Normal horses
				#region Transport horses
				else if (it.Name.Contains("COS_C"))
					it.Pettype = item_database.PetType.TRANSPORT;
				#endregion
				//Quest items , define more later.
				#region Quest Items
				else if (it.Name.Contains("QNQ"))
					it.Questtype = item_database.QuestType.QUEST;
				#endregion
				#region Avatars
				else if (it.TypeID2 == 1 && it.TypeID3 == 13 && it.TypeID4 == 1)
				{
					it.Type = item_database.ArmorType.AVATARHAT;
					it.Itemtype = item_database.ItemType.AVATAR;
					it.Equip = true;
				}
				else if (it.TypeID2 == 1 && it.TypeID3 == 13 && it.TypeID4 == 2)
				{
					it.Type = item_database.ArmorType.AVATAR;
					it.Itemtype = item_database.ItemType.AVATAR;
					it.Equip = true;
				}
				else if (it.TypeID2 == 1 && it.TypeID3 == 13 && it.TypeID4 == 3)
				{
					it.Type = item_database.ArmorType.AVATARATTACH;
					it.Itemtype = item_database.ItemType.AVATAR;
					it.Equip = true;
				}
				else if (it.TypeID2 == 1 && it.TypeID3 == 14 && it.TypeID4 == 1) // Devil
				{
					//it.Type = item_database.ArmorType.;
					it.Itemtype = item_database.ItemType.AVATAR;
					it.Equip = true;
				}
				else if (it.Name.Contains("ITEM_ETC_E051021")) // Flags
				{
					//it.Type = item_database.ArmorType.;it.TypeID1 == 3 && it.TypeID2 == 3 && it.TypeID3 == 9 && it.TypeID3 == 0
					it.Type = item_database.ArmorType.AVATAR;
					it.Itemtype = item_database.ItemType.AVATAR;
					Console.WriteLine(it.Equip);
					it.Equip = true;

				}
				#endregion
				//Potions
				#region Potions
				#region Normal potions
				else if (it.Name.Contains("CANDY") && it.Name.Contains("RED"))
					it.Etctype = item_database.EtcType.HP_POTION;
				else if (it.Name.Contains("POTION") && it.Name.Contains("HP") && it.TypeID4 == 1 && it.Item_Mall_Type != 2)
					it.Etctype = item_database.EtcType.HP_POTION;
				else if (it.Name.Contains("CANDY") && it.Name.Contains("BLUE"))
					it.Etctype = item_database.EtcType.MP_POTION;
				else if (it.Name.Contains("POTION") && it.Name.Contains("MP") && it.TypeID4 == 2 && it.Item_Mall_Type != 2)
					it.Etctype = item_database.EtcType.MP_POTION;
				else if (it.Name.Contains("CANDY") && it.Name.Contains("VIOLET"))
					it.Etctype = item_database.EtcType.VIGOR_POTION;
				else if (it.Name.Contains("ITEM_ETC_ALL_POTION"))
					it.Etctype = item_database.EtcType.VIGOR_POTION;

				#endregion
				#region HP / MP Changing potions
				else if (it.Name.Contains("HP_INC"))
					it.Etctype = item_database.EtcType.HPSTATPOTION;
				else if (it.Name.Contains("MP_INC"))
					it.Etctype = item_database.EtcType.MPSTATPOTION;
				#endregion
				#region Speed Potions
				else if (it.Name.Contains("POTION_SPEED"))
					it.Etctype = item_database.EtcType.SPEED_POTION;
				#endregion
				#region Berserk potions
				else if (it.Name.Contains("HWAN_POTION"))
					it.Etctype = item_database.EtcType.BERSERKPOTION;
				#endregion
				#endregion
				//Tickets all types
				#region Silver Skill Tickets
				else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("1D") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_SILVER_1_DAY;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("4W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_SILVER_4_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("8W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_SILVER_8_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("12W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_SILVER_12_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("16W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_SILVER_16_WEEKS;
				#endregion
				#region Gold Skill Tickets
				else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("1D") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_GOLD_1_DAY;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("4W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_GOLD_4_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("8W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_GOLD_8_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("12W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_GOLD_12_WEEKS;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("16W") && it.Name.Contains("SKILL"))
					it.Ticket = item_database.Tickets.SKILL_GOLD_16_WEEKS;
				#endregion
				#region Silver Tickets
				else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("1D"))
					it.Ticket = item_database.Tickets.SILVER_1_DAY;
				else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("4W"))
					it.Ticket = item_database.Tickets.SILVER_4_WEEKS;
				else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("8W"))
					it.Ticket = item_database.Tickets.SILVER_8_WEEKS;
				else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("12W"))
					it.Ticket = item_database.Tickets.SILVER_12_WEEKS;
				else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("16W"))
					it.Ticket = item_database.Tickets.SILVER_16_WEEKS;
				#endregion
				#region Gold Tickets
				else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("1D"))
					it.Ticket = item_database.Tickets.GOLD_1_DAY;
				else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("4W"))
					it.Ticket = item_database.Tickets.GOLD_4_WEEKS;
				else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("8W"))
					it.Ticket = item_database.Tickets.GOLD_8_WEEKS;
				else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("12W"))
					it.Ticket = item_database.Tickets.GOLD_12_WEEKS;
				else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("16W"))
					it.Ticket = item_database.Tickets.GOLD_16_WEEKS;
				#endregion
				#region Premium Quest Tickets
				else if (it.Name.Contains("TICKET") && it.Name.Contains("PREM"))
					it.Ticket = item_database.Tickets.PREMIUM_QUEST_TICKET;
				#endregion
				#region Open Market
				else if (it.Name.Contains("OPEN_MARKET"))
					it.Ticket = item_database.Tickets.OPEN_MARKET;
				#endregion
				#region Dungeon tickets
				else if (it.Name.Contains("TICKET") && it.Name.Contains("EGYPT"))
					it.Ticket = item_database.Tickets.DUNGEON_EGYPT;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("FORGOTTEN"))
					it.Ticket = item_database.Tickets.DUNGEON_FORGOTTEN_WORLD;
				else if (it.Name.Contains("TICKET") && it.Name.Contains("BATTLE_ARENA"))
					it.Ticket = item_database.Tickets.BATTLE_ARENA;
				else if (it.Name.Contains("ITEM_ETC_TELEPORT_HOLE"))
					it.Ticket = item_database.Tickets.DUNGEON_FORGOTTEN_WORLD;
				#endregion
				#region Warehouse
				else if (it.Name.Contains("TICKET") && it.Name.Contains("WAREHOUSE"))
					it.Ticket = item_database.Tickets.WAREHOUSE;
				#endregion
				#region Auto Potion Ticket
				else if (it.Name.Contains("TICKET") && it.Name.Contains("AUTO_POTION"))
					it.Ticket = item_database.Tickets.AUTO_POTION;
				#endregion
				#region Beginner tickets
				else if (it.Name.Contains("ETC") && it.Name.Contains("_HELP"))
					it.Ticket = item_database.Tickets.BEGINNER_HELPERS;
				#endregion
				#region Global chat
				else if (it.Name.Contains("GLOBAL") && it.Name.Contains("CHAT"))
					it.Etctype = item_database.EtcType.GLOBALCHAT;
				#endregion
				#region Stall decoration
				else if (it.Name.Contains("BOOTH"))
					it.Etctype = item_database.EtcType.STALLDECORATION;
				#endregion
				#region Monster Masks
				else if (it.Name.Contains("TRANS_MONSTER"))
					it.Etctype = item_database.EtcType.MONSTERMASK;
				#endregion
				//Elixirs
				#region Elixirs
				else if (it.Name.Contains("ETC") && it.Name.Contains("REINFORCE") && it.Name.Contains("RECIPE") && it.Name.Contains("_B") && it.TypeID4 == 1)
					it.Etctype = item_database.EtcType.ELIXIR;
				#endregion
				//Job suits
				#region Job Suits
				//Hunter suits
				else if (it.Name.Contains("TRADE_HUNTER") && it.Name.Contains("CH"))
					it.Type = item_database.ArmorType.HUNTER;
				else if (it.Name.Contains("TRADE_HUNTER") && it.Name.Contains("EU"))
					it.Type = item_database.ArmorType.HUNTER;
				//Thief suits
				else if (it.Name.Contains("TRADE_THIEF") && it.Name.Contains("CH"))
					it.Type = item_database.ArmorType.THIEF;
				else if (it.Name.Contains("TRADE_THIEF") && it.Name.Contains("EU"))
					it.Type = item_database.ArmorType.THIEF;
				#endregion
				//Return scrolls
				#region Return scrolls
				else if (it.Name.Contains("SCROLL_RETURN"))
					it.Etctype = item_database.EtcType.RETURNSCROLL;
				#endregion
				//Reverse scrolls
				#region Reverse scrolls
				else if (it.Name.Contains("SCROLL") && it.Name.Contains("REVERSE"))
					it.Etctype = item_database.EtcType.REVERSESCROLL;
				#endregion
				//Thief scrolls
				#region Thief scrolls
				else if (it.Name.Contains("SCROLL") && it.Name.Contains("THIEF"))
					it.Etctype = item_database.EtcType.BANDITSCROLL;
				#endregion
				//Summon scrolls
				#region Summon scrolls
				else if (it.Name.Contains("SUMMON") && it.Name.Contains("SCROLL"))
					it.Etctype = item_database.EtcType.SUMMONSCROLL;
				#endregion
				//Skin change scrolls
				#region Skin change scrolls
				else if (it.Name.Contains("SKIN_CHANGE"))
					it.Etctype = item_database.EtcType.CHANGESKIN;
				#endregion
				//Inventory expansion
				#region Inventory expansions
				else if (it.Name.Contains("INVENTORY") && it.Name.Contains("ADDITION"))
					it.Etctype = item_database.EtcType.INVENTORYEXPANSION;
				#endregion
				//Warehouse expansion
				#region Warehouse expansions
				else if (it.Name.Contains("WAREHOUSE") && it.Name.Contains("ADDITION"))
					it.Etctype = item_database.EtcType.WAREHOUSE;
				#endregion
				//Alchemy materials
				#region Alchemy materials
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAT"))
					it.Etctype = item_database.EtcType.ALCHEMY_MATERIAL;
				#endregion
				//Tablets
				#region Tablets
				// later have to differ assimilate stones that has % not like astral and steady
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICTABLET"))
					it.Itemtype = item_database.ItemType.MAGICTABLET;
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ATTRTABLET"))
					it.Itemtype = item_database.ItemType.MAGICTABLET;
				#endregion
				//Elements
				#region Elements
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ELEMENT"))
					it.Etctype = item_database.EtcType.ELEMENTS;
				#endregion
				//Stones (Note: Need to filter it more detailed later to deny some stones).
				#region Stones
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ATTRSTONE") && !it.Name.Contains("AST"))
					it.Etctype = item_database.EtcType.STONES;
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICSTONE") && !it.Name.Contains("AST"))
					it.Etctype = item_database.EtcType.STONES;
				else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICSTONE") && it.Name.Contains("ASTRAL"))
					it.Etctype = item_database.EtcType.ASTRALSTONE;
				#endregion
				//Destroyer rondo
				#region Destroyer rondo
				else if (it.Name.Contains("ITEM_ETC_ARCHEMY_RONDO_02"))
					it.Etctype = item_database.EtcType.DESTROYER_RONDO;
				#endregion
				//Gender switch tools
				#region Gender switch
				else if (it.TypeID1 == 3 && it.TypeID2 == 3 && it.TypeID3 == 13 && it.TypeID4 == 8)
					it.Etctype = item_database.EtcType.ITEMCHANGETOOL;
                #endregion
                //Guild items
                if (it.Name.Contains("GUILD_CREST"))
                    it.Etctype = item_database.EtcType.GUILD_ICON;

                //########################################################
                // Silk Prices Definitions
                //########################################################     
                #region Silk pricing
                if (it.Etctype == item_database.EtcType.AVATAR28D ||
                    it.Etctype == item_database.EtcType.CHANGESKIN ||
                    it.Etctype == item_database.EtcType.GLOBALCHAT ||
                    it.Etctype == item_database.EtcType.INVENTORYEXPANSION ||
                    it.Etctype == item_database.EtcType.RETURNSCROLL ||
                    it.Etctype == item_database.EtcType.REVERSESCROLL ||
                    it.Etctype == item_database.EtcType.STALLDECORATION ||
                    it.Etctype == item_database.EtcType.WAREHOUSE ||
                    it.Pettype == item_database.PetType.GRABPET ||
                    it.Type == item_database.ArmorType.AVATAR ||
                    it.Type == item_database.ArmorType.AVATARHAT ||
                    it.Type == item_database.ArmorType.AVATARATTACH)
                    it.Shop_price = 0;
                #endregion
                //########################################################
                // Drop Database
                //########################################################
                //Armors
                #region Armors
                if ((it.Name.Contains("LIGHT") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("LIGHT") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("HEAVY") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("HEAVY") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("CLOTHES") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("CLOTHES") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER"))
                    && it.SOX == 0)
                {
                    Manager.ArmorDataBase.ID.Add(it.ID);
                }
                #endregion
                //Weapons
                #region Weapons
                if ((it.Name.Contains("ITEM_EU_AXE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_CROSSBOW") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_DAGGER") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_DARKSTAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_HARP") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_STAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_SWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_TSTAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_TSWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_TBLADE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_BLADE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_BOW") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_SHIELD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SHIELD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SPEAR") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")))
                {
                    Manager.WeaponDataBase.ID.Add(it.ID);
                }
                #endregion
                //Jewelerys
                #region Jewelerys
                if ((it.Name.Contains("RING") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_CH") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")) ||
                    (it.Name.Contains("NECKLACE") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_EU") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")))
                {
                    Manager.JewelDataBase.ID.Add(it.ID);
                }
                #endregion
                //Seal items
                #region Seal items
                if (it.SOX == 2 && !it.Name.Contains("_SET") && !it.Name.Contains("_HONOR") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("BASIC") && !it.Name.Contains("DEF"))
                {
                    Manager.SoxDataBase.ID.Add(it.ID);
                }
                #endregion
                //Stones
                #region Stones
                else if (it.Itemtype == item_database.ItemType.MAGICTABLET && !it.Name.Contains("ASTRAL") || it.Itemtype == item_database.ItemType.ATTRTABLET && !it.Name.Contains("ASTRAL"))
                {
                    Manager.StoneDataBase.ID.Add(it.ID);
                }
                #endregion
                //Alchemy materials
                #region Alchemy materials
                else if (it.Etctype == item_database.EtcType.ALCHEMY_MATERIAL)
                {
                    Manager.MaterialDataBase.ID.Add(it.ID);
                }
                #endregion
                //Elixirs
                #region Elixirs
                else if (it.Etctype == item_database.EtcType.ELIXIR)
                {
                    Manager.ElixirDataBase.ID.Add(it.ID);
                }
                #endregion
                //########################################################
                // Soulbound settings (Need to see if i can find in pk2
                //########################################################
                #region Soulbound information
                //Tmp
                if (it.Name.Contains("PRE_MALL"))
                    it.Accountbound = 0;
                
                #endregion
                //########################################################
                // Shop price fixing
                //########################################################
                #region Shop pricing
                if (it.Name.Contains("RETURN") && it.Name.Contains("_01"))
                    it.Shop_price = 6000;
                else if (it.Name.Contains("RETURN") && it.Name.Contains("_02"))
                    it.Shop_price = 12000;
                #endregion
                //Distances
                #region Distances
                it.ATTACK_DISTANCE /= 10.0;
                #endregion
                //Race fixes
                #region Race fixes
                if (it.Name.Contains("_EU_"))
                    it.Race = 1;
                else if (it.Name.Contains("_CH_"))
                    it.Race = 0;
                #endregion
                //1 damage weapon fixes.
                #region Weapon fixes
                if (it.Name.Contains("DEF") && it.Name.Contains("STAFF"))
                {
                    it.Attack.Min_HPhyAttack = it.Attack.Min_HMagAttack;
                    it.Attack.Min_LPhyAttack = it.Attack.Min_LMagAttack;
                }
                #endregion
                //if (it.ID == 9268)
                //    Console.WriteLine("Info: {0}, {1} {2} {3} {4}", it.Name, it.Attack.Min_HMagAttack, it.Attack.Min_HPhyAttack, it.Attack.Min_LMagAttack, it.Attack.Min_LPhyAttack);
            }
            Console.WriteLine("[INFO] Loaded "+ TxtFile.amountLine +" items");
        }
        /////////////////////////////////////////////////////////////////////////////
        // Load objects
        /////////////////////////////////////////////////////////////////////////////
        public static void LoadObject()
        {
            TxtFile.ReadFromFile(@"\data\npcpos.txt", '\t');
            int npcamount = Helpers.Settings.Rate.MonsterSpawn;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                TxtFile.commands =  TxtFile.lines[l].ToString().Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[0]);
                string namecheck = Manager.ObjectBase[ID].Name;
                if (namecheck.Contains("MOB_"))
                {
                    for (int i = 1; i <= npcamount; i++)
                    {
                        WorldMgr.Monsters o = new WorldMgr.Monsters();
                        short AREA = short.Parse(TxtFile.commands[1]);
                        float x = Convert.ToInt32(TxtFile.commands[2]);
                        float z = Convert.ToInt32(TxtFile.commands[3]);
                        float y = Convert.ToInt32(TxtFile.commands[4]);
                        o.ID = ID;
                        o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                        o.UniqueID = o.Ids.GetUniqueID;

                        o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                        o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                        o.x = Formule.gamex(x, o.xSec);
                        o.z = z;
                        o.y = Formule.gamey(y, o.ySec);
                        if (i > 1)
                        {
                            Helpers.Functions.aRound(ref o.x, ref o.y, Rnd.Next(1, 6));
                        }
                        
                        o.OriginalX = o.x;
                        o.OriginalY = o.y;
                        
                        o.State = 1;
                        o.Move = 1;
                        o.AutoSpawn = true;
                        o.State = 2;
                        o.HP = Manager.ObjectBase[ID].HP;
                        o.Kat = 1;
                        o.AgressiveDMG = new List<WorldMgr._agro>();
                        o.WalkingSpeed = Manager.ObjectBase[o.ID].SpeedWalk;
                        o.RunningSpeed = Manager.ObjectBase[o.ID].SpeedRun;
                        o.BerserkerSpeed = Manager.ObjectBase[o.ID].SpeedZerk;
                        o.Agresif = Manager.ObjectBase[o.ID].Agresif;
                        if (o.Type == 1) o.Agresif = 1;
                        o.spawnOran = 20;
                        if (o.WalkingSpeed == 0 && o.RunningSpeed == 0)
                        {
                            o.AutoMovement = false;
                            o.LocalType = 5;//fix for static flowers,ishades,etc
                        }
                        else
                        {
                            o.AutoMovement = true;
                            o.LocalType = Manager.ObjectBase[ID].Type;
                        }
                        if (o.AutoMovement) o.StartRunTimer(Rnd.Next(1, 5) * 1000);

                        if (Manager.ObjectBase[ID].ObjectType != 3)
                        {
                            o.Type = Helpers.Functions.RandomType(Manager.ObjectBase[ID].Level, ref o.Kat, false, ref o.Agresif);
                            o.HP *= o.Kat;
                            if (o.Type == 1)
                                o.Agresif = 1;
                            Helpers.Manager.Objects.Add(o);

                        }
                        else
                        {
                            o.AutoSpawn = false;
                            o.Type = Manager.ObjectBase[ID].ObjectType;
                            //GlobalUnique.AddObject(o);
                            
                        }
                        if (namecheck.Contains("CH")) Manager.ObjectBase[ID].Race = 0;
                        if (namecheck.Contains("EU")) Manager.ObjectBase[ID].Race = 1;
                        if (!namecheck.Contains("CH") && (!namecheck.Contains("EU"))) Manager.ObjectBase[ID].Race = 2;
                        o.InitalizeNpcs();
                    }
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " monsters");
        }
        /////////////////////////////////////////////////////////////////////////////
        // Load npcs
        /////////////////////////////////////////////////////////////////////////////
        public static void LoadNpcs()
        {
            TxtFile.ReadFromFile(@"\data\npcpos.txt", '\t');
            string input = null;
            string s = null;
            string[] npcangle1;
            int count = TxtFile.amountLine;
            uint index = 0;
            int countme = 0;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[0]);
                byte race = Manager.ObjectBase[ID].Type;
                string namecheck = Manager.ObjectBase[ID].Name;
                
                if (namecheck.Contains("NPC_"))
                {
                    countme += 1;
                    TextReader Npcangle = new StreamReader(Environment.CurrentDirectory + @"\data\NpcAngles.txt");
                    WorldMgr.Monsters o = new WorldMgr.Monsters();
                    index++;
                    short AREA = short.Parse(TxtFile.commands[1]);
                    float x = Convert.ToSingle(TxtFile.commands[2]);
                    float z = Convert.ToSingle(TxtFile.commands[3]);
                    float y = Convert.ToSingle(TxtFile.commands[4]);


                    byte movement = 0;
                    o.Agresif = movement;
                    o.AutoMovement = true;
                    o.ID = ID;
                    o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                    o.UniqueID = o.Ids.GetUniqueID;
                    o.area = AREA;
                    o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                    o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                    o.x = Formule.gamex(x, o.xSec);
                    o.z = z;
                    o.y = Formule.gamey(y, o.ySec);
                    o.OriginalX = o.x;
                    o.OriginalY = o.y;
                    o.State = 1;
                    o.Move = 1;
                    o.LocalType = 2;
                    o.AutoSpawn = true;
                    o.State = 2;
                    o.HP = Manager.ObjectBase[ID].HP;
                    o.Kat = 1;
                    o.AgressiveDMG = new List<WorldMgr._agro>();
                    o.spawnOran = 20;

                    while ((input = Npcangle.ReadLine()) != null)
                    {
                        npcangle1 = input.Split(',');
                        if (ID == ushort.Parse(npcangle1[0]) && AREA == ushort.Parse(npcangle1[2]))
                        {
                            o.Angle = ushort.Parse(npcangle1[1]);
                            break;
                        }
                    }
                    Npcangle.Close();

                    if (Manager.ObjectBase[ID].ObjectType != 3)
                    {
                        o.Type = Helpers.Functions.RandomType(Manager.ObjectBase[ID].Level, ref o.Kat, false, ref o.Agresif);
                        o.HP *= o.Kat;
                        if (o.Type == 1) o.Agresif = 1;
                        Helpers.Manager.Objects.Add(o);
                    }
                    else
                    {
                        o.AutoSpawn = false;
                        o.Type = Manager.ObjectBase[ID].ObjectType;
                        // This is unique NPC_MONSTER
                    }
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " npc's");
        }
        /////////////////////////////////////////////////////////////////////////////
        // Objects
        /////////////////////////////////////////////////////////////////////////////
        public static void ObjectDataBase(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            string s = null;
            int count = TxtFile.amountLine;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[1]);
                objectdata o = new objectdata();
                o.ID = ID;
                o.Name = TxtFile.commands[2];
                o.Level = Convert.ToByte(TxtFile.commands[57]);
                o.Exp = Convert.ToInt32(TxtFile.commands[79]);
                o.HP = Convert.ToInt32(TxtFile.commands[59]);
                o.Type = Convert.ToByte(TxtFile.commands[11]);
                o.ObjectType = Convert.ToByte(TxtFile.commands[15]);
                o.PhyDef = Convert.ToInt32(TxtFile.commands[71]);
                o.MagDef = Convert.ToInt32(TxtFile.commands[72]);
                o.HitRatio = Convert.ToInt32(TxtFile.commands[75]);
                o.ParryRatio = Convert.ToInt32(TxtFile.commands[77]);
                o.Agresif = Convert.ToByte(TxtFile.commands[93]);
                o.Skill = new int[500];
                o.Speed1 = Convert.ToInt32(TxtFile.commands[46]);
                o.Speed2 = Convert.ToInt32(TxtFile.commands[47]);

                o.SpeedWalk = Convert.ToInt32(TxtFile.commands[46]);
                o.SpeedRun = Convert.ToInt32(TxtFile.commands[47]);
                o.SpeedZerk = Convert.ToInt32(TxtFile.commands[48]);
                if (o.Name.Contains("CHAR_CH")) o.Race = 0;
                if (o.Name.Contains("CHAR_EU")) o.Race = 1;

                if (o.Name.Contains("THIEF_NPC") || o.Name.Contains("HUNTER_NPC")) o.Type = 4; o.Agresif = 1;
                for (byte sk = 0; sk <= 8; sk++)
                {
                    if (Convert.ToInt32(TxtFile.commands[83 + sk]) != 0 && Manager.SkillBase[Convert.ToInt32(TxtFile.commands[83 + sk])].MagPer != 0)
                    {
                        o.Skill[o.amountSkill] = Convert.ToInt32(TxtFile.commands[83 + sk]);
                        o.amountSkill++;
                    }
                }
                Manager.ObjectBase[ID] = o;

            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " objects");
        }
        /////////////////////////////////////////////////////////////////////////////
        // Teleports
        /////////////////////////////////////////////////////////////////////////////
        public static void TeleportBuilding()
        {
            TxtFile.ReadFromFile(@"\data\teleportbuilding.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                if (!(short.Parse(TxtFile.commands[41]) == 0))
                {
                    WorldMgr.Monsters o = new WorldMgr.Monsters();
                    int ID = Convert.ToInt32(TxtFile.commands[1]);
                    short AREA = short.Parse(TxtFile.commands[41]);
                    float x = Convert.ToSingle(TxtFile.commands[43]);
                    float z = Convert.ToSingle(TxtFile.commands[44]);
                    float y = Convert.ToSingle(TxtFile.commands[45]);
                    o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                    o.UniqueID = o.Ids.GetUniqueID;
                    objectdata os = new objectdata();
                    os.Name = TxtFile.commands[2];
                    Manager.ObjectBase[ID] = os;
                    o.ID = ID;
                    o.area = AREA;
                    o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                    o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                    o.x = Formule.gamex(x,o.xSec);
                    o.z = z;
                    o.y = Formule.gamey(y, o.ySec);
                    o.HP = 0x000000C0;
                    o.LocalType = 3;

                    Helpers.Manager.Objects.Add(o);
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " teleport buildings");
            TeleportData();
        }

        /////////////////////////////////////////////////////////////////////////////
        // cave Teleport data
        /////////////////////////////////////////////////////////////////////////////
        public static void cavedata()// this is added as the location where you end up in cave from telepad also formula was added for coords
        {
            TxtFile.ReadFromFile(@"\data\cavespawns.txt", '\t');

            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                cavepoint o = new cavepoint();
                int Number = Convert.ToInt32(TxtFile.commands[1]);
                int ID = Convert.ToInt32(TxtFile.commands[3]);
                short AREA = short.Parse(TxtFile.commands[5]);
                double x = Convert.ToDouble(TxtFile.commands[6]);
                double z = Convert.ToDouble(TxtFile.commands[7]);
                double y = Convert.ToDouble(TxtFile.commands[8]);

                o.test = Convert.ToByte(TxtFile.commands[12]);
                o.Name = TxtFile.commands[2];
                o.ID = ID;
                o.Number = Number;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                if (!(o.xSec < 8))
                {
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                }
                else
                {
                    o.x = Formule.cavegamex((float)x);
                    o.z = z;
                    o.y = Formule.cavegamey((float)y);

                }
                Manager.cavePointBase[Number] = o;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Internal cave spawns");
        }
        
        /////////////////////////////////////////////////////////////////////////////
        // Teleport data
        /////////////////////////////////////////////////////////////////////////////
        public static void TeleportData()
        {
            TxtFile.ReadFromFile(@"\data\teleportdata.txt", '\t');

            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                point o = new point();
                int Number = Convert.ToInt32(TxtFile.commands[1]);
                int ID = Convert.ToInt32(TxtFile.commands[3]);
                short AREA = short.Parse(TxtFile.commands[5]);
                double x = Convert.ToDouble(TxtFile.commands[6]);
                double z = Convert.ToDouble(TxtFile.commands[7]);
                double y = Convert.ToDouble(TxtFile.commands[8]);

                o.test = Convert.ToByte(TxtFile.commands[12]);
                o.Name = TxtFile.commands[2];
                o.ID = ID;
                o.Number = Number;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                if (!(o.xSec < 8))
                {
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                }
                else
                {
                    o.x = Formule.cavegamex((float)x);//formula was added for coords
                    o.z = z;
                    o.y = Formule.cavegamey((float)y);

                }
                Manager.PointBase[Number] = o;
            }
        }
        /////////////////////////////////////////////////////////////////////////////
        // Get teleports
        /////////////////////////////////////////////////////////////////////////////
        public static byte GetTeleport(string name)
        {
            try
            {
                byte rNum = Convert.ToByte(name);
                return rNum;
            }
            catch
            {
                for (byte b = 0; b <= Manager.PointBase.Length - 1; b++)
                {
                    if (Manager.PointBase[b] != null && Manager.PointBase[b].Name == name) return b;
                }
                return 1;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Skill data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadSkillData(string path)
        {
            TxtFile.ReadFromFile(path, '\t');
            for (int LineIndex = 0; LineIndex <= TxtFile.amountLine - 1; LineIndex++)
            {
                TxtFile.commands = TxtFile.lines[LineIndex].ToString().Split('\t');
                s_data sd = new s_data();
                sd.ID = Convert.ToInt32(TxtFile.commands[1]);
                sd.Name = TxtFile.commands[3];
                sd.Series = TxtFile.commands[5];
                sd.SkillType = (s_data.SkillTypes)Convert.ToByte(TxtFile.commands[8]);
                sd.NextSkill = Convert.ToInt32(TxtFile.commands[9]);
                sd.Action_PreparingTime = Convert.ToInt32(TxtFile.commands[11]);
                sd.Action_CastingTime = Convert.ToInt32(TxtFile.commands[12]);
                sd.Action_ActionDuration = Convert.ToInt32(TxtFile.commands[13]);
                sd.CastingTime = sd.Action_PreparingTime + sd.Action_CastingTime + sd.Action_ActionDuration;
                sd.Action_ReuseDelay = Convert.ToInt32(TxtFile.commands[14]);
                sd.Action_CoolTime = Convert.ToInt32(TxtFile.commands[15]);
                sd.Action_FlyingSpeed = Convert.ToInt32(TxtFile.commands[16]);
                
                sd.Mastery = Convert.ToInt16(TxtFile.commands[34]);
                sd.SkillPoint = Convert.ToInt32(TxtFile.commands[46]);
                sd.Weapon1 = Convert.ToByte(TxtFile.commands[50]);
                sd.Weapon2 = Convert.ToByte(TxtFile.commands[51]);
                sd.Mana = Convert.ToInt32(TxtFile.commands[53]);
                sd.tmpProp = Convert.ToInt32(TxtFile.commands[75]);
                sd.AmountEffect = 0;
                sd.AttackCount = Convert.ToInt32(TxtFile.commands[77]);
                sd.RadiusType = s_data.RadiusTypes.ONETARGET; //default type
                sd.isAttackSkill = false;

                int propIndex = 69;
                bool effectEnd = false;
                int skillInfo;
                //Define missed information
                //Imbue
                if (sd.Name.Contains("_GIGONGTA_"))
                    sd.Definedname = s_data.Definedtype.Imbue;
                try
                {
                    while ((skillInfo = Convert.ToInt32(TxtFile.commands[propIndex])) != 0 && !effectEnd)
                    {
                        propIndex++;

                        string nameString = ASCIIIntToString(skillInfo);

                        switch (nameString)
                        {
                            // get value - only to client
                            case "getv":
                            case "MAAT":
                            // warrior
                            case "E2AH":
                            case "E2AA":
                            case "E1SA":
                            case "E2SA":
                            // rogue
                            case "CBAT":
                            case "CBRA":
                            case "DGAT":
                            case "DGHR":
                            case "DGAA":
                            case "STDU":
                            case "STSP":
                            case "RPDU":
                            case "RPTU":
                            case "RPBU":
                            // wizard
                            case "WIMD":
                            case "WIRU":
                            case "EAAT":
                            case "COAT":
                            case "FIAT":
                            case "LIAT":
                            // warlock
                            case "DTAT":
                            case "DTDR":
                            case "BLAT":
                            case "TRAA":
                            case "BSHP":
                            case "SAAA":
                            // bard
                            case "MUAT":
                            case "BDMD":
                            case "MUER":
                            case "MUCR":
                            case "DSER":
                            case "DSCR":
                            // cleric
                            case "HLAT":
                            case "HLRU":
                            case "HLMD":
                            case "HLFS":
                            case "HLMI":
                            case "HLBP":
                            case "HLSM":
                            // attribute only - no effect
                            case "nmh": // Healing stone (socket stone)
                            case "nmf": // Movement stone (socket stone)
                            case "eshp":
                            case "reqn":
                            case "fitp":
                            case "ao":   // fortress ??
                            case "rpkt": // fortress repair kit
                            case "hitm": // Taunt the enemy into attacking only you
                            case "efta": // bard tambour
                            case "lks2": // warlock damage buff
                            case "hntp": // tag point
                            case "trap": // csapda??
                            case "cbuf": //itembuff
                            case "nbuf": // ??(ticketnél volt) nem látszika  buff másnak?
                            case "bbuf": //debuff
                            case null:
                                break;
                            case "setv":  //set value
                                string setvType = ASCIIIntToString(Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;

                                switch (setvType)
                                {   // warrior
                                    case "E1SA": // phy attack % //done
                                    case "E2SA": // phy attack % //done
                                    case "E2AA":
                                    case "E2AH": // hit rate inc //done
                                    // rogue
                                    case "CBAT":
                                    case "CBRA":
                                    case "DGAT":
                                    case "DGHR":
                                    case "DGAA":
                                    case "STSP": // speed %
                                    case "STDU": // set stealth duration
                                    case "RPDU": // phy attack %
                                    case "RPBU": // poison duration 
                                    // wizard
                                    case "WIMD": // wizard mana decrease %
                                    case "WIRU": // Increase the range of magic attacks
                                    case "EAAT": // Magical Attack Power %Increase earth
                                    case "COAT": // Magical Attack Power %Increase cold
                                    case "FIAT": // Magical Attack Power %Increase fire
                                    case "LIAT": // Magical Attack Power %Increase fire
                                    // warlock
                                    case "DTAT": // Magical Attack Power %Increase
                                    case "DTDR": // Increase the abnormal status duration inflicted by Dark magic
                                    case "BLAT": // Magical Attack Power %Increase Blood Line row 
                                    case "TRAA": // Increases a Warlocks trap damage 
                                    case "BSHP": // HP draining skill attack power increase
                                    // bard
                                    case "MUAT": // Magical Attack Power % Increase
                                    case "MUER": // Increase the range of harp magic
                                    case "BDMD": // Lowers the MP consumption of skills
                                    case "MUCR": // Resistance Ratio % Increase.
                                    case "DSER": // Increase the range for dance skill.
                                    case "DSCR": // Increase resistance ratio. You don't stop dancing even under attack 
                                    // cleric
                                    case "HLAT": // Increase the damage of cleric magic. %
                                    case "HLRU": // HP recovery % Inrease
                                        sd.Properties1.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;
                                        break;
                                    // cleric
                                    case "HLFS": // charity
                                    case "HLMI": // charity
                                    case "HLBP": // charity
                                    case "HLSM": // charity
                                        sd.Properties1.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;
                                        sd.Properties2.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;
                                        
                                        break;
                                }
                                break;
                            // 1 properties
                            case "tant":
                            case "rcur": // randomly cure number of bad statuses
                            case "ck":
                            case "ovl2":
                            case "mwhs":
                            case "scls":
                            case "mwmh":
                            case "mwhh":
                            case "rmut":
                            case "abnb":
                            case "mscc":
                            case "bcnt": // cos bag count [slot]
                            case "chpi": // cos hp increase [%]
                            case "chst": // cos speed increase [%]
                            case "csum": // cos summon [coslevel]
                            case "jobs": // job skill [charlevel]
                            case "hwit": // ITEM_ETC_SOCKET_STONE_HWAN ?? duno what is it
                            case "spi": // Gives additional skill points when hunting monsters. [%inc]
                            case "dura": // skill duration
                            case "msid": // mod def ignore prob%
                            case "hwir": // honor buff new zerk %
                            case "hst3": // honor buff speed %inc
                            case "hst2": // rogue haste speed %inc
                            case "lkdd": // Share % damage taken from a selected person. (link damage)
                            case "gdr":  // gold drop rate %inc
                            case "chcr": // target loses % HP
                            case "cmcr": // target loses % MP
                            case "dcmp": // MP Cost % Decrease
                            case "mwdt": // Weapon Magical Attack Power %Reflect
                            case "pdmg": // Absolute Damage
                            case "lfst": // life steal Absorb HP
                            case "puls": // pulsing skill frequenzy
                            case "pwtt": // Weapon Physical Attack Power reflect.
                            case "pdm2": // ghost killer
                            case "luck": // lucky %inc
                            case "alcu": // alchemy lucky %inc
                            case "terd": // parry reduce
                            case "thrd": // Attack ratio reduce
                            case "ru": // range incrase
                            case "hste": // speed %inc
                            case "da": // downattack %inc
                            case "reqc": // required status?
                            case "dgmp": // damage mana absorb
                            case "dcri": // critical parry inc
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 2 properties
                            case "mc":
                            case "atca":
                            case "reat":
                            case "defr":
                            case "msr": // Triggered at a certain chance, the next spell cast does not cost MP. [%chance to trigger|%mana reduce]
                            case "kb": // knockback
                            case "ko":  // knockdown
                            case "zb": // zombie
                            case "fz":  // frozen
                            case "fb":  // frostbite
                            case "spda": // Shield physical defence % reduce. Physical attack power increase.
                            case "expi": // sp|exp %inc PET?
                            case "stri": // str increase
                            case "inti": // int increase
                            case "rhru": // Increase the amount of HP healed. %
                            case "dmgt": // Absorption %? 
                            case "dtnt": // Aggro Reduce
                            case "mcap": // monster mask lvl cap
                            case "apau": // attack power inc [phy|mag]
                            case "lkag": // Share aggro
                            case "dttp": // detect stealth [|maxstealvl]
                            case "tnt2": // taunt inc | aggro %increase
                            case "expu": // exp|sp %inc
                            case "msch": // monster transform
                            case "dtt": // detect invis [ | maxinvilvl]
                            case "hpi": // hp incrase [inc|%inc]
                            case "mpi": // mp incrase [inc|%inc]
                            case "odar": // damage absorbation
                            case "resu": // resurrection [lvl|expback%]
                            case "er": // evasion | parry %inc 
                            case "hr": // hit rating inc | attack rating inc 
                            case "tele": // light teleport [sec|meter*10]
                            case "tel2": // wizard teleport [sec|meter*10]
                            case "tel3": // warrior sprint teleport [sec|m]
                            case "onff": // mana consume per second
                            case "br":  // blocking ratio [|%inc]
                            case "cr":  // critical inc
                            case "dru": // damage %inc [phy|mag]
                            case "irgc": // reincarnation [hp|mp]
                            case "pola": // Preemptive attack prevention [hp|mp]
                            case "curt": // negative status effect reduce target player
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 3 properties
                            case "curl": //anti effect: cure [|effect cure amount|effect level]
                            case "real":
                            case "skc":
                            case "bldr": // Reflects damage upon successful block.
                            case "ca": // confusion
                            case "rt":  // restraint (wizard) << restraint i guess it should restrain the target or put to ground as in same spot like chains on feet :) 
                            case "fe": // fear 
                            case "sl": // dull
                            case "st": // stun
                            case "se": // sleep
                            case "es":  // lightening
                            case "bu":  // burn
                            case "ps":  // poison
                            case "lkdh": // link Damage % MP Absorption (Mana Switch)
                            case "stns": // Petrified status
                            case "hide": // stealth hide
                            case "lkdr": // Share damage
                            case "defp": // defensepower inc [phy|mag]
                            case "bgra": // negative status effect reduce
                            case "cnsm": // consume item
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 4 properties
                            case "csit": // division
                            case "tb": // hidden
                            case "my": // short sight
                            case "ds": // disease
                            case "csmd":  // weaken
                            case "cspd":  // decay
                            case "cssr": // impotent
                            case "dn": // darkness
                            case "mom": // duration | Berserk mode Attack damage/Defence/Hit rate/Parry rate will increase % | on every X mins
                            case "pmdp": // maximum physical defence strength decrease %
                            case "pmhp": // hp reduce
                            case "dmgr": // damage return [prob%|return%||]
                            case "lnks": // Connection between players
                            case "pmdg": // damage reduce [dura|phy%|mag%|?]
                            case "qest": // some quest related skill?
                            case "heal": // heal [hp|hp%|mp|mp%]
                            case "pw": // player wall
                            case "summ": // summon bird
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 5 properties
                            case "bl": // bleed
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties5.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 6 properties
                            case "cshp": // panic
                            case "csmp": // combustion
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties5.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties6.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            case "reqi": // required item
                                int weapType1 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int weapType2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;

                                if (weapType1 == 4 && weapType2 == 1)
                                    sd.ReqItems.Add(s_data.ItemTypes.SHIELD);
                                else if (weapType1 == 4 && weapType2 == 2)
                                    sd.ReqItems.Add(s_data.ItemTypes.EUSHIELD);
                                else if (weapType1 == 6 && weapType2 == 6)
                                    sd.ReqItems.Add(s_data.ItemTypes.BOW);
                                else if (weapType1 == 6 && weapType2 == 7)
                                    sd.ReqItems.Add(s_data.ItemTypes.ONEHANDED);
                                else if (weapType1 == 6 && weapType2 == 8)
                                    sd.ReqItems.Add(s_data.ItemTypes.TWOHANDED);
                                else if (weapType1 == 6 && weapType2 == 9)
                                    sd.ReqItems.Add(s_data.ItemTypes.AXE);
                                else if (weapType1 == 6 && weapType2 == 10)
                                    sd.ReqItems.Add(s_data.ItemTypes.WARLOCKROD);
                                else if (weapType1 == 6 && weapType2 == 11)
                                    sd.ReqItems.Add(s_data.ItemTypes.STAFF);
                                else if (weapType1 == 6 && weapType2 == 12)
                                    sd.ReqItems.Add(s_data.ItemTypes.XBOW);
                                else if (weapType1 == 6 && weapType2 == 13)
                                    sd.ReqItems.Add(s_data.ItemTypes.DAGGER);
                                else if (weapType1 == 6 && weapType2 == 14)
                                    sd.ReqItems.Add(s_data.ItemTypes.BARD);
                                else if (weapType1 == 6 && weapType2 == 15)
                                    sd.ReqItems.Add(s_data.ItemTypes.CLERICROD);
                                else if (weapType1 == 10 && weapType2 == 0)
                                    sd.ReqItems.Add(s_data.ItemTypes.LIGHTARMOR);
                                else if (weapType1 == 14 && weapType2 == 1)
                                    sd.ReqItems.Add(s_data.ItemTypes.DEVILSPIRIT);
                                break;
                            case "ssou": // summon monster
                                s_data.summon_data summon;
                                while (Convert.ToInt32(TxtFile.commands[propIndex]) != 0)
                                {
                                    summon = new s_data.summon_data();
                                    summon.ID = Convert.ToInt32(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.Type = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.MinSummon = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.MaxSummon = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;

                                    sd.SummonList.Add(summon);
                                }
                                break;
                            case "att": // if attack skill
                                sd.isAttackSkill = true;
                                //sd.Time = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MagPer = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MinAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MaxAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.PhyPer = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                break;
                            case "efr":
                                sd.efrUnk1 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int type2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                if (type2 == 6)
                                    sd.RadiusType = s_data.RadiusTypes.TRANSFERRANGE;
                                else if (type2 == 2)
                                    sd.RadiusType = s_data.RadiusTypes.FRONTRANGERADIUS;
                                else if (type2 == 7)
                                    sd.RadiusType = s_data.RadiusTypes.MULTIPLETARGET;
                                else if (type2 == 4)
                                    sd.RadiusType = s_data.RadiusTypes.PENETRATION;
                                else if (type2 == 3)
                                    sd.RadiusType = s_data.RadiusTypes.PENETRATIONRANGED;
                                else if (type2 == 1)
                                    sd.RadiusType = s_data.RadiusTypes.SURROUNDRANGERADIUS;
                                propIndex++;

                                sd.Distance = Convert.ToInt32(TxtFile.commands[propIndex]); // in decimeters
                                propIndex++;
                                sd.SimultAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int unk2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex ++;
                                int unk3 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex ++;
                                break;
                            default:
                                //Console.WriteLine(" {0}  {1}  {2}", propIndex, nameString, sd.Name);
                                effectEnd = true;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }

                // this property only affects target when player target is set
                sd.canSelfTargeted = true;
                sd.needPVPstate = true;

                ////////////// set skill property's additional info for non attack skillz
                if (!sd.isAttackSkill)
                {
                    if (sd.Properties1.ContainsKey("heal") || // heal ;)
                        sd.Properties1.ContainsKey("curl"))   // bad status removal
                    {
                        sd.TargetType = s_data.TargetTypes.PLAYER;
                        sd.needPVPstate = false;
                    }
                    if (sd.Properties1.ContainsKey("resu")) // resurrection
                    {
                        sd.TargetType = s_data.TargetTypes.PLAYER;
                        sd.canSelfTargeted = false;
                        sd.needPVPstate = false;
                    }
                    if (sd.Properties1.ContainsKey("terd") ||  // parry reduce
                        sd.Properties1.ContainsKey("thrd") ||  // Attack ratio reduce
                        sd.Properties1.ContainsKey("cspd") ||  // decay
                        sd.Properties1.ContainsKey("csmd") ||  // weaken
                        sd.Properties1.ContainsKey("cssr") ||  // impotent
                        sd.Properties1.ContainsKey("st")   ||  // stun
                        sd.Properties1.ContainsKey("bu")   ||  // burn
                        sd.Properties1.ContainsKey("fb"))      // frostbite

                    {
                        sd.TargetType = s_data.TargetTypes.PLAYER | s_data.TargetTypes.MOB;
                        sd.canSelfTargeted = false;
                    }
                }
                Manager.SkillBase[sd.ID] = sd;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " skills");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Mastery data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadMasteryData()
        {
            TxtFile.ReadFromFile(@"\data\mastery.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int sp = Convert.ToInt16(TxtFile.commands[1]);
                byte level = Convert.ToByte(TxtFile.commands[0]);

                Manager.MasteryBase[level] = sp;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " mastery levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Gold data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadGoldData()
        {
            TxtFile.ReadFromFile(@"\data\levelgold.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                levelgold lg = new levelgold();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                lg.min = Convert.ToInt16(TxtFile.commands[1]);
                lg.max = Convert.ToInt16(TxtFile.commands[2]);

                Manager.LevelGold[level] = lg;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " gold stats");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Job Level Data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadJobLevels()
        {
            TxtFile.ReadFromFile(@"\data\tradeconflict_joblevel.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                JobLevel levelinfo = new JobLevel();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                levelinfo.level = Convert.ToByte(TxtFile.commands[0]);
                levelinfo.exp = Convert.ToUInt64(TxtFile.commands[1]);
                levelinfo.jobtitle = Convert.ToByte(TxtFile.commands[2]);

                Manager.Joblevelinfo.Add(levelinfo);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Job levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Level info
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadLevelData()
        {
            TxtFile.ReadFromFile(@"\data\leveldata.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                long exp = Convert.ToInt64(TxtFile.commands[1]);

                Manager.LevelData[level] = exp;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " player levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // magic options
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadMagicOptions()
        {
            TxtFile.ReadFromFile(@"\data\magicoption.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                MagicOption m = new MagicOption();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                m.ID = Convert.ToInt32(TxtFile.commands[1]);
                m.Name = TxtFile.commands[2];
                m.Type = TxtFile.commands[3];
                m.Level = Convert.ToInt32(TxtFile.commands[4]);
                m.OptionPercent = Convert.ToDouble(TxtFile.commands[5].Replace('.', ','));


                    if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9])) != 0)
                    {
                        m.MinValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9]));
                    }
                    else
                    {
                        m.MinValue = Convert.ToInt32(TxtFile.commands[9]);
                    }
                    if (Convert.ToInt32(TxtFile.commands[10]) != 0)
                    {
                        if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[10])) != 0)
                        {
                            m.MaxValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[10]));
                        }
                        else
                        {
                            m.MaxValue = Convert.ToInt32(TxtFile.commands[10]);
                        }
                    }
                    else
                    {
                        if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9])) != 0)
                        {
                            m.MinValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[8]));
                            m.MaxValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9]));
                        }

                    }
                
                m.ApplicableOn[0] = TxtFile.commands[31];
                if (TxtFile.commands[33] != "xxx") m.ApplicableOn[1] = TxtFile.commands[33];
                if (TxtFile.commands[35] != "xxx") m.ApplicableOn[2] = TxtFile.commands[35];
                if (TxtFile.commands[37] != "xxx") m.ApplicableOn[3] = TxtFile.commands[37];

                Manager.MagicOptions.Add(m);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " blue options");
        }
        public static int ConvertBlueValue(int num)
        {
            int number = num >>= 16;
            return number;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Shop tabs
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadShopTabData()
        {
            TxtFile.ReadFromFile(@"\data\shopdata.txt", '\t');
            string s = null;
            int count = TxtFile.amountLine;
            byte[] co = new byte[28000];
            Shop_Alexandria();
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[5]);

                if (ID > 0)
                    if (Manager.ObjectBase[ID] != null)
                    {
                        string name = TxtFile.commands[2];
                        Manager.ObjectBase[ID].StoreName = name;
                    }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Npc shop data");

            TxtFile.ReadFromFile(@"\data\refmappingshopwithtab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                int ID = GetNpcID(name);
                if (Manager.ObjectBase[ID] != null)
                {
                    Manager.ObjectBase[ID].Shop[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Shop tabs");
            co = null;
            co = new byte[25000];

            TxtFile.ReadFromFile(@"\data\refshoptab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[4];
                int ID = GetNpcID_(name);
                if (Manager.ObjectBase[ID] != null)
                {
                    shop_data sh = new shop_data();
                    sh.tab = TxtFile.commands[3];
                    Manager.ShopData.Add(sh);
                    Manager.ObjectBase[ID].Tab[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            co = null;

            TxtFile.ReadFromFile(@"\data\refshopgoods.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                shop_data ID = shop_data.GetShopIndex(name);
                if (ID != null)
                {
                    TxtFile.commands[3] = TxtFile.commands[3].Remove(0, 8);
                    ID.Item[Convert.ToInt16(TxtFile.commands[4])] = TxtFile.commands[3];

                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Shop items");
            SetShopData();
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Helper
        /////////////////////////////////////////////////////////////////////////////////
        public static string ASCIIIntToString(int skillInfo)
        {
            byte[] name;
            if (skillInfo <= 0xFF)
            {
                name = new byte[1];
                name[0] = (byte)(skillInfo);
            }
            else if (skillInfo <= 0xFFFF)
            {
                name = new byte[2];
                name[0] = (byte)(skillInfo >> 8);
                name[1] = (byte)(skillInfo);
            }
            else if (skillInfo <= 0xFFFFFF)
            {
                name = new byte[3];
                name[0] = (byte)(skillInfo >> 16);
                name[1] = (byte)(skillInfo >> 8);
                name[2] = (byte)(skillInfo);
            }
            else
            {
                name = new byte[4];
                name[0] = (byte)(skillInfo >> 24);
                name[1] = (byte)(skillInfo >> 16);
                name[2] = (byte)(skillInfo >> 8);
                name[3] = (byte)(skillInfo);
            }
            //Yeah but do you understand any of this what a skillinfo ? its the skill id ok i understand so show me here where i find the string
            return System.Text.Encoding.ASCII.GetString(name);
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get npc by string
        /////////////////////////////////////////////////////////////////////////////////
        public static int GetNpcID(string name)
        {
            for (int i = 0; i < Manager.ObjectBase.Length; i++)
            {
                if (Manager.ObjectBase[i] != null && Manager.ObjectBase[i].StoreName == name)
                    return Manager.ObjectBase[i].ID;
            }
            return 0;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get npc by string
        /////////////////////////////////////////////////////////////////////////////////
        public static int GetNpcID_(string name)
        {
            for (int i = 0; i <= Manager.ObjectBase.Length - 1; i++)
            {
                if (Manager.ObjectBase[i] != null)
                {
                    for (int b = 0; b <= 10 - 1; b++)
                        if (Manager.ObjectBase[i].Shop[b] == name)
                            return Manager.ObjectBase[i].ID;
                }
            }
            return 0;
        }
        public static void SetShopData()
        {
            #region Jangang
            Manager.ObjectBase[1915].Tab[0] = "STORE_CH_POTION_TAB1";

            Manager.ObjectBase[2008].Tab[0] = "STORE_CH_ACCESSORY_TAB1";
            Manager.ObjectBase[2008].Tab[1] = "STORE_CH_ACCESSORY_TAB2";
            Manager.ObjectBase[2008].Tab[2] = "STORE_CH_ACCESSORY_TAB3";

            Manager.ObjectBase[2010].Tab[0] = "STORE_CH_SPECIAL_TAB1";


            #endregion

            #region Donwhang

            #endregion

            #region Hotan
            Manager.ObjectBase[2021].Tab[0] = "STORE_CH_GUILD_TAB1";
            Manager.ObjectBase[2021].Tab[1] = "STORE_CH_GUILD_TAB3";

            Manager.ObjectBase[2072].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Manager.ObjectBase[2072].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Manager.ObjectBase[2072].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Manager.ObjectBase[2072].Tab[3] = "STORE_KT_SMITH_TAB1";
            Manager.ObjectBase[2072].Tab[4] = "STORE_KT_SMITH_TAB2";
            Manager.ObjectBase[2072].Tab[5] = "STORE_KT_SMITH_TAB3";


            Manager.ObjectBase[2073].Tab[0] = "STORE_KT_ARMOR_EU_TAB1";
            Manager.ObjectBase[2073].Tab[1] = "STORE_KT_ARMOR_EU_TAB2";
            Manager.ObjectBase[2073].Tab[2] = "STORE_KT_ARMOR_EU_TAB3";
            Manager.ObjectBase[2073].Tab[3] = "STORE_KT_ARMOR_EU_TAB4";
            Manager.ObjectBase[2073].Tab[4] = "STORE_KT_ARMOR_EU_TAB5";
            Manager.ObjectBase[2073].Tab[5] = "STORE_KT_ARMOR_EU_TAB6";
            Manager.ObjectBase[2073].Tab[6] = "STORE_KT_ARMOR_TAB1";
            Manager.ObjectBase[2073].Tab[7] = "STORE_KT_ARMOR_TAB2";
            Manager.ObjectBase[2073].Tab[8] = "STORE_KT_ARMOR_TAB3";
            Manager.ObjectBase[2073].Tab[9] = "STORE_KT_ARMOR_TAB4";
            Manager.ObjectBase[2073].Tab[10] = "STORE_KT_ARMOR_TAB5";
            Manager.ObjectBase[2073].Tab[11] = "STORE_KT_ARMOR_TAB6";

            Manager.ObjectBase[2075].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Manager.ObjectBase[2075].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Manager.ObjectBase[2075].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Manager.ObjectBase[2075].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Manager.ObjectBase[2075].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Manager.ObjectBase[2075].Tab[5] = "STORE_KT_ACCESSORY_TAB3";

            Manager.ObjectBase[9274].Tab[0] = "STORE_KT_GUILD_TAB1";
            Manager.ObjectBase[9274].Tab[1] = "STORE_KT_GUILD_TAB3";
            #endregion

            #region Europe sm
            Manager.ObjectBase[7531].Tab[0] = "STORE_CA_ARMOR_TAB1";
            Manager.ObjectBase[7531].Tab[1] = "STORE_CA_ARMOR_TAB2";
            Manager.ObjectBase[7531].Tab[2] = "STORE_CA_ARMOR_TAB3";
            Manager.ObjectBase[7531].Tab[3] = "STORE_CA_ARMOR_TAB4";
            Manager.ObjectBase[7531].Tab[4] = "STORE_CA_ARMOR_TAB5";
            Manager.ObjectBase[7531].Tab[5] = "STORE_CA_ARMOR_TAB6";

            Manager.ObjectBase[7530].Tab[0] = "STORE_CA_SMITH_TAB1";
            Manager.ObjectBase[7530].Tab[1] = "STORE_CA_SMITH_TAB2";
            Manager.ObjectBase[7530].Tab[2] = "STORE_CA_SMITH_TAB3";

            Manager.ObjectBase[7534].Tab[0] = "STORE_CA_STABLE_TAB1";
            Manager.ObjectBase[7534].Tab[1] = "STORE_CA_STABLE_TAB2";
            Manager.ObjectBase[7534].Tab[2] = "STORE_CA_STABLE_TAB3";
            Manager.ObjectBase[7534].Tab[3] = "STORE_CA_STABLE_TAB4";

            Manager.ObjectBase[7536].Tab[0] = "STORE_CA_TRADER_TAB1";
            Manager.ObjectBase[7536].Tab[1] = "STORE_CA_TRADER_TAB2";

            Manager.ObjectBase[7533].Tab[0] = "STORE_CA_ACCESSORY_TAB1";
            Manager.ObjectBase[7533].Tab[1] = "STORE_CA_ACCESSORY_TAB2";
            Manager.ObjectBase[7533].Tab[2] = "STORE_CA_ACCESSORY_TAB3";

            Manager.ObjectBase[7532].Tab[0] = "STORE_CA_POTION_TAB1";

            #endregion
        }
        public static void Shop_Alexandria()
        {
            #region Jangang
            Manager.ObjectBase[objectdata.GetItem("NPC_CH_ACCESSORY")].StoreName = "STORE_CA_ACCESSORY";
            Manager.ObjectBase[objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[0] = "STORE_CH_ACCESSORY_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[1] = "STORE_CH_ACCESSORY_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[2] = "STORE_CH_ACCESSORY_TAB3";

            #endregion

            #region Eu sm
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].StoreName = "STORE_CA_ARMOR";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[0] = "STORE_CA_ARMOR_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[1] = "STORE_CA_ARMOR_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[2] = "STORE_CA_ARMOR_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[3] = "STORE_CA_ARMOR_TAB4";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[4] = "STORE_CA_ARMOR_TAB5";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ARMOR")].Tab[5] = "STORE_CA_ARMOR_TAB6";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_SMITH")].StoreName = "STORE_CA_SMITH";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_SMITH")].Tab[0] = "STORE_CA_SMITH_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_SMITH")].Tab[1] = "STORE_CA_SMITH_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_SMITH")].Tab[2] = "STORE_CA_SMITH_TAB3";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HORSE")].StoreName = "STORE_CA_STABLE";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HORSE")].Tab[0] = "STORE_CA_STABLE_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HORSE")].Tab[1] = "STORE_CA_STABLE_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HORSE")].Tab[2] = "STORE_CA_STABLE_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HORSE")].Tab[3] = "STORE_CA_STABLE_TAB4";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_MERCHANT")].StoreName = "STORE_CA_TRADER";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_MERCHANT")].Tab[0] = "STORE_CA_TRADER_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_MERCHANT")].Tab[1] = "STORE_CA_TRADER_TAB2";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ACCESSORY")].StoreName = "STORE_CA_ACCESSORY";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[0] = "STORE_CA_ACCESSORY_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[1] = "STORE_CA_ACCESSORY_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[2] = "STORE_CA_ACCESSORY_TAB3";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_POTION")].StoreName = "STORE_CA_POTION";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_POTION")].Tab[0] = "STORE_CA_POTION_TAB1";

            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HUNTER")].StoreName = "STORE_CA_HUNTER";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HUNTER")].Tab[0] = "STORE_CA_HUNTER_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_CA_HUNTER")].Tab[0] = "STORE_CA_HUNTER_TAB2";
            #endregion

            #region Constantinople
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].StoreName = "STORE_EU_ARMOR";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[0] = "STORE_EU_ARMOR_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[1] = "STORE_EU_ARMOR_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[2] = "STORE_EU_ARMOR_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[3] = "STORE_EU_ARMOR_TAB4";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[4] = "STORE_EU_ARMOR_TAB5";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ARMOR")].Tab[5] = "STORE_EU_ARMOR_TAB6";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_SMITH")].StoreName = "STORE_EU_SMITH";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_SMITH")].Tab[0] = "STORE_EU_SMITH_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_SMITH")].Tab[1] = "STORE_EU_SMITH_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_SMITH")].Tab[2] = "STORE_EU_SMITH_TAB3";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HORSE")].StoreName = "STORE_EU_STABLE";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HORSE")].Tab[0] = "STORE_EU_STABLE_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HORSE")].Tab[1] = "STORE_EU_STABLE_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HORSE")].Tab[2] = "STORE_EU_STABLE_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HORSE")].Tab[3] = "STORE_EU_STABLE_TAB4";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_MERCHANT")].StoreName = "STORE_EU_TRADER";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_MERCHANT")].Tab[0] = "STORE_EU_TRADER_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_MERCHANT")].Tab[1] = "STORE_EU_TRADER_TAB2";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ACCESSORY")].StoreName = "STORE_EU_ACCESSORY";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[0] = "STORE_EU_ACCESSORY_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[1] = "STORE_EU_ACCESSORY_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[2] = "STORE_EU_ACCESSORY_TAB3";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_POTION")].StoreName = "STORE_EU_POTION";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_POTION")].Tab[0] = "STORE_EU_POTION_TAB1";

            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HUNTER")].StoreName = "STORE_EU_HUNTER";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HUNTER")].Tab[0] = "STORE_EU_HUNTER_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_EU_HUNTER")].Tab[0] = "STORE_EU_HUNTER_TAB2";
            #endregion
            #region GUILD
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_GUILD")].StoreName = "STORE_KT_GUILD";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_GUILD")].Tab[0] = "STORE_CH_GUILD_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_GUILD")].Tab[1] = "STORE_CH_GUILD_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_WC_GUILD_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_WC_GUILD_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_GUILD_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_GUILD_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_EU_GUILD_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_EU_GUILD_TAB3";

            #endregion
            #region SMITH
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";


            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";
            #endregion

            #region ACCESSORY
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3";

            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3"; //STORE_KT_POTION STORE_KT_POTION_TAB1
            #endregion

            #region POTION
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_M_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";

            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Manager.ObjectBase[objectdata.GetItem("NPC_SD_T_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";
            #endregion

        }
        // Region Informations:
        public static void LoadRegionCodes()
        {
            TxtFile.ReadFromFile(@"\data\regioncode.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                region r = new region();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                r.ID = Convert.ToInt32(TxtFile.commands[1]);
                r.Name = TxtFile.commands[2];
                if (r.Name == "xxx") r.Name = "";
                //r.SecX = Convert.ToInt32(TxtFile.commands[5]);
                //r.SecY = Convert.ToInt32(TxtFile.commands[6]);
                Manager.RegionBase.Add(r);
            }

            //get safe zones
            foreach (region r in Manager.RegionBase)
            {
                switch (r.ID)
                {
                    //the edges of cities:
                    case 25257:
                    case 24999:
                    case 25001:
                    case 26521:
                    case 26263:
                    case 26265:
                    case 23686:
                    case 23689:
                    case 23175:
                    case 27244:
                    case 27245:
                    case 27501:
                    case 27500:
                    case 27499:
                    case 26957:
                    case 27471:
                    //the area of cities:
                    case 26958:
                    case 26702:
                    case 26446:
                    case 26704:
                    case 26448:
                    case 26960:
                    case 27216:
                    case 27472:
                    case 27217:
                    case 27473:
                    case 27985:
                    case 27729:
                    case 27243:
                    case 23687:
                    case 23431:
                    case 23943:
                    case 24199:
                    case 23688:
                    case 23432:
                    case 26519:
                    case 25000:
                    case 25256:
                    case 25255:
                    case 24743:
                        Manager.safeZone.Add(r);
                        break;

                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Zones");
        }
        public static void ReverseData()
        {
            TxtFile.ReadFromFile(@"\data\refoptionalteleport.txt", '\t');
            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                int ID = Convert.ToInt32(TxtFile.commands[1]);
                short area = short.Parse(TxtFile.commands[4]);
                int x = Convert.ToInt32(TxtFile.commands[5]);
                int z = Convert.ToInt32(TxtFile.commands[6]);
                int y = Convert.ToInt32(TxtFile.commands[7]);
                reverse o = new reverse();
                o.ID = ID;
                o.xSec = Convert.ToByte((area).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((area).ToString("X4").Substring(0, 2), 16);

                o.x = x;
                o.z = z;
                o.y = y;

                Manager.ReverseData[ID] = o;

            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Reverse locations");
        }
        public static void LoadTeleportPrice()
        {
            TxtFile.ReadFromFile(@"\data\teleportlink.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                TeleportPrice t = new TeleportPrice();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int id = Convert.ToInt32(TxtFile.commands[2]);
                t.ID = id;
                t.price = Convert.ToInt32(TxtFile.commands[3]);
                t.level = Convert.ToInt32(TxtFile.commands[7]);
                Manager.TeleportPrice.Add(t);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Teleport locations");
        }
        

        public static void LoadCaves()
        {
            TxtFile.ReadFromFile(@"\data\caveteleportdata.txt", '\t');// made some changes here but still need to add some data to the text files
            string s = null;
            int count = TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                region r = new region();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                r.ID = l;
                short area = short.Parse(TxtFile.commands[5]);
                r.Name = TxtFile.commands[2];
                r.SecX = Convert.ToByte((area).ToString("X4").Substring(2, 2), 16);
                r.SecY = Convert.ToByte((area).ToString("X4").Substring(0, 2), 16);
                Manager.Cave.Add(r);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Cave teleport data");
        }
        
        
        
        public static bool CheckCave(byte xsec, byte ysec)
        {
            bool s = Manager.Cave.Exists(delegate(region r)
            {
                if (r.SecX == xsec && r.SecY == ysec)
                {
                    if (!(r.Name.ToUpper() == "GATE_DUNGEON_DH_OUT" || r.Name.ToUpper() == "GATE_JINSI_OUT"))// added this so when out of cave the server know we are back to basic movement not cave
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    }
                return false;
            });
            return s;
        }
        public static void LoadCaveTeleports()
        {
            TxtFile.ReadFromFile(@"\data\caveteleportok1.txt", '\t');// changed file name as this is still work in progress
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                CaveTeleports c = new CaveTeleports();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                c.name = TxtFile.commands[0];
                c.xsec = Convert.ToByte(TxtFile.commands[1]);
                c.ysec = Convert.ToByte(TxtFile.commands[2]);
                string[] x1x = new string[2];
                string[] z1z = new string[2];
                string[] y1y = new string[2];
                x1x = TxtFile.commands[3].Split(',');// the Convert ToDouble from a file read will sometimes put 10567,00234 as 105670.3 that is not good for us so i split the , out of the data to get true coord
                c.x = Convert.ToDouble(x1x[0]);
                z1z = TxtFile.commands[4].Split(',');
                c.z = Convert.ToDouble(z1z[0]);
                y1y = TxtFile.commands[5].Split(',');
                c.y = Convert.ToDouble(y1y[0]);
                Manager.CaveTeleports.Add(c);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Cave teleports");
        }
        public static void LoadQuestData(string file)
        {
            TxtFile.ReadFromFile(file, '\t');
            string line = null;
            quest_database Quest = new quest_database();

            for (int i = 0; i <= TxtFile.amountLine - 1; i++)
            {
                line = TxtFile.lines[i].ToString();
                TxtFile.commands = line.Split('\t');
                Quest.Questid = Convert.ToInt32(TxtFile.commands[1]);
                Quest.QuestNPC = Convert.ToString(TxtFile.commands[2]);
                Quest.QuestLevel = Convert.ToInt32(TxtFile.commands[3]);
                Manager.QuestData.Add(Quest);
            }
            TxtFile.ReadFromFile(@"\data\questcontentsdata.txt", '\t');
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                line = TxtFile.lines[l].ToString();
                TxtFile.commands = line.Split('\t');
                Quest.Questname = TxtFile.commands[5];
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Quests");
        }
        public static void TraderData()
        {
            //Trade scales 1 / 5 stars
            TxtFile.ReadFromFile(@"data\maxtradescaledata.txt", '\t');
            string line = null;
            for (int i = 0; i <= TxtFile.amountLine - 1; i++)
            {
                line = TxtFile.lines[i].ToString();
                TxtFile.commands = line.Split('\t');
                trader_data Data = new trader_data();
                Data.Amount = Convert.ToInt32(TxtFile.commands[0]);
                /* ################
                 * Definitions
                 * Extra check on trader amounts to ensure no exploiting
                 * Must add Max trade amount to 5 stars
                 * ################*/
                if (Data.Amount > 0 && Data.Amount < 510) Data.Details  = trader_data.stars.ONESTAR;
                else if (Data.Amount > 510   && Data.Amount < 918) Data.Details  = trader_data.stars.TWOSTARS;
                else if (Data.Amount > 918   && Data.Amount < 1428) Data.Details  = trader_data.stars.THREESTARS;
                else if (Data.Amount > 1428  && Data.Amount < 2142) Data.Details  = trader_data.stars.FOURSTARS;
                else if (Data.Amount > 2142  && Data.Amount < 2856) Data.Details  = trader_data.stars.FIVESTARS;
				else
				{
                    Data.Details = trader_data.stars.ZEROSTARS;
				}
                 
            }
        }

        public static void SetTimerItems(string listfile)
        {
            TxtFile.ReadFromFile(listfile, '\t');
            int count = TxtFile.amountLine;
            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Timer items");
        }
        public static void LoadMapObject(string filename)
        {
            Console.Clear();
            try
            {
                string path = Environment.CurrentDirectory + @"\data\" + filename;

                /* Get file content to memory */
                FileStream fs = File.OpenRead(path);
                MemoryStream ms = new MemoryStream();

                ms.SetLength(fs.Length);
                fs.Read(ms.GetBuffer(), 0, (int)fs.Length);

                ms.Flush();
                fs.Close();

                BinaryReader br = new BinaryReader(ms);

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    SectorObject obj = new SectorObject();

                    /* Read each sector's data */

                    byte xSector = br.ReadByte();
                    byte ySector = br.ReadByte();
                    short region = Formule.makeRegion(xSector, ySector);

                    for (int i = 0; i < 9409; ++i)
                        obj.heightmap[i] = br.ReadSingle();

                    obj.entityCount = br.ReadInt32();

                    for (int i = 0; i < obj.entityCount; ++i)
                    {

                        SectorObject.n7nEntity entity = new SectorObject.n7nEntity();
                        entity.Points = new List<SectorObject.n7nEntity.sPoint>();
                        entity.OutLines = new List<SectorObject.n7nEntity.sLine>();

                        entity.Position.x = br.ReadSingle();
                        entity.Position.z = br.ReadSingle();
                        entity.Position.y = br.ReadSingle();
                        entity.ObjectMapflag = br.ReadByte();

                        if (entity.ObjectMapflag == 1)
                        {
                            int PointCount = br.ReadInt32();
                            for (int j = 0; j < PointCount; ++j)
                            {
                                SectorObject.n7nEntity.sPoint p = new SectorObject.n7nEntity.sPoint();

                                p.x = br.ReadSingle();
                                p.z = br.ReadSingle();
                                p.y = br.ReadSingle();
                                entity.Points.Add(p);
                            }

                            int LineCount = br.ReadInt32();

                            for (int j = 0; j < LineCount; ++j)
                            {
                                SectorObject.n7nEntity.sLine l = new SectorObject.n7nEntity.sLine();

                                l.PointA = br.ReadInt16();
                                l.PointB = br.ReadInt16();
                                l.flag = br.ReadByte();

                                entity.OutLines.Add(l);
                            }
                        }
                        obj.entitys.Add(entity);
                    }

                    Manager.MapObject.Add(region, obj);
                }
                Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Map objects");
            }
            catch(Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}



