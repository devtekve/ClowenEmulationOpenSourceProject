using CLGameServer.Client;
using CLFramework;
using System;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        #region WorldMgr.character creation
        void CharacterCreate()
        {
            //Start wrapper for catching errors
            try
            {
                //Check the amount of characters created (If 4 then we return). Todo: Check if any message shows up.
                if (DB.GetRowsCount("SELECT * FROM character WHERE account='" + Player.AccountName + "'") == 4)
                    return;
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte tocheck = Reader.Byte();
                string name = Reader.Text();
                int model = Reader.Int32();
                byte volume = Reader.Byte();
                int[] Item = new int[4];
                Item[0] = Reader.Int32();
                Item[1] = Reader.Int32();
                Item[2] = Reader.Int32();
                Item[3] = Reader.Int32();
                Reader.Close();
                //Check player name
                #region Check Name
                //Check the character name before creation
                if (CharacterCheck(name))
                {
                    //If bad send informational packet
                    client.Send(Packet.CharacterName(4));
                    //Finally return.
                    return;
                }
                //Set and disable special characters
                if (name.Contains("[")) return;
                else if (name.Contains("GM")) return;
                else if (name.Contains("]")) return;
                else if (name.Contains("-")) return;
                #endregion
                //Begin creation for EU characters
                #region European characters
                //Check by model type to see what race we have.
                if (model >= 14715 && model <= 14745)
                {
                    //Insert the basic information into the database
                    DB.query("INSERT INTO character (account, name, chartype, volume, xsect, ysect, xpos, zpos, ypos, savearea,GuideData) VALUES ('" + Player.AccountName + "','" + name + "', '" + model + "', '" + volume + "','79','105','387','1000','1279','20','0000000000000000')");
                    //Insert reverse scroll data into the database
                    DB.query("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + name + "','79','105','1000','22','83')");
                    //Set definition for the character id information
                    Player.CreatingCharID = Convert.ToInt32(DB.GetData("Select * from character Where name='" + name + "'", "id"));
                    //Set default information (TODO: Check if all chars have same base).
                    double MagDef = 3;
                    double PhyDef = 6;
                    double Parry = 11;
                    double Hit = 11;
                    double MinPhyA = 6;
                    double MaxPhyA = 9;
                    double MinMagA = 6;
                    double MaxMagA = 10;

                    //Open new framework ini reading
                    Ini ini;
                    //Open settings information for start items
                    ini = new Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                    //5 Items max for starting characters
                    string Item1 = ini.GetValue("European.Start", "Item1", "").ToString();
                    string Item2 = ini.GetValue("European.Start", "Item2", "").ToString();
                    string Item3 = ini.GetValue("European.Start", "Item3", "").ToString();
                    string Item4 = ini.GetValue("European.Start", "Item4", "").ToString();
                    string Item5 = ini.GetValue("European.Start", "Item5", "").ToString();
                    //The amount related information
                    string Amount1 = ini.GetValue("European.Start", "Amount1", "").ToString();
                    string Amount2 = ini.GetValue("European.Start", "Amount2", "").ToString();
                    string Amount3 = ini.GetValue("European.Start", "Amount3", "").ToString();
                    string Amount4 = ini.GetValue("European.Start", "Amount4", "").ToString();
                    string Amount5 = ini.GetValue("European.Start", "Amount5", "").ToString();
                    //Add the custom items
                    try
                    {
                        AddItem(Convert.ToInt32(Item1), Convert.ToByte(Amount1), 13, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item2), Convert.ToByte(Amount2), 14, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item3), Convert.ToByte(Amount3), 15, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item4), Convert.ToByte(Amount4), 16, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item5), Convert.ToByte(Amount5), 17, Player.CreatingCharID, 0);
                    }
                    catch (Exception)
                    {
                    }
                    //Add the base items
                    AddItem(Item[0], 0, 1, Player.CreatingCharID, 0);
                    AddItem(Item[1], 0, 4, Player.CreatingCharID, 0);
                    AddItem(Item[2], 0, 5, Player.CreatingCharID, 0);
                    AddItem(Item[3], 0, 6, Player.CreatingCharID, 0);
                    //Set mag defense
                    MagDef += ObjData.Manager.ItemBase[Item[0]].Defans.MinMagDef;
                    MagDef += ObjData.Manager.ItemBase[Item[1]].Defans.MinMagDef;
                    MagDef += ObjData.Manager.ItemBase[Item[2]].Defans.MinMagDef;
                    //Set phy defence
                    PhyDef += ObjData.Manager.ItemBase[Item[0]].Defans.MinPhyDef;
                    PhyDef += ObjData.Manager.ItemBase[Item[1]].Defans.MinPhyDef;
                    PhyDef += ObjData.Manager.ItemBase[Item[2]].Defans.MinPhyDef;
                    //Set parry information
                    Parry += ObjData.Manager.ItemBase[Item[0]].Defans.Parry;
                    Parry += ObjData.Manager.ItemBase[Item[1]].Defans.Parry;
                    Parry += ObjData.Manager.ItemBase[Item[2]].Defans.Parry;
                    //Set hit ratio
                    Hit += ObjData.Manager.ItemBase[Item[3]].Attack.MinAttackRating;
                    //Set phy attack
                    MinPhyA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                    MaxPhyA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                    //Set mag attack
                    MinMagA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_LMagAttack;
                    MaxMagA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_HMagAttack;
                    //If the 3rd item is a sword or a dark staff
                    if (Item[3] == 10730 || Item[3] == 10734)
                    {
                        //Add the mag def information
                        MagDef += ObjData.Manager.ItemBase[251].Defans.MinMagDef;
                        //Add the phy def information
                        PhyDef += ObjData.Manager.ItemBase[251].Defans.MinPhyDef;
                        //Add parry ration
                        Parry += ObjData.Manager.ItemBase[251].Defans.Parry;
                        //Add shield item
                        AddItem(10738, 0, 7, Player.CreatingCharID, 0);
                    }
                    //If the 3rd item is a crossbow
                    if (Item[3] == 10733)
                    {
                        //We add our base bolts 250
                        AddItem(10376, 250, 7, Player.CreatingCharID, 0);
                    }
                    //Update database information for stats
                    DB.query("update character set min_phyatk='" + (int)Math.Round(MinPhyA) +
                            "', max_phyatk='" + Math.Round(MaxPhyA) +
                            "', min_magatk='" + Math.Round(MinMagA) +
                            "', max_magatk='" + Math.Round(MaxMagA) +
                            "', phydef='" + Math.Round(PhyDef) +
                            "', magdef='" + Math.Round(PhyDef) +
                            "', parry='" + Math.Round(Parry) +
                            "', hit='" + Math.Round(Hit) +
                            "' where name='" + name + "'");
                    //Add base mastery's for europe characters
                    AddMastery(513, Player.CreatingCharID); //Warrior
                    AddMastery(515, Player.CreatingCharID); //Rogue
                    AddMastery(514, Player.CreatingCharID); //Wizard
                    AddMastery(516, Player.CreatingCharID); //Warlock
                    AddMastery(517, Player.CreatingCharID); //Bard
                    AddMastery(518, Player.CreatingCharID); //Cleric
                    client.Send(Packet.ScreenSuccess(1));
                #endregion
                }
                //If the character model is an chinese character
                else
                {
                #region Chinese characters
                    DB.query("INSERT INTO character (account, name, chartype, volume,GuideData) VALUES ('" + Player.AccountName + "','" + name + "', '" + model + "', '" + volume + "','0000000000000000')");
                    DB.query("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + name + "','168','79','911','1083','-32')");
                    Player.CreatingCharID = Convert.ToInt32(DB.GetData("Select * from character Where name='" + name + "'", "id"));

                    #region Item
                    double MagDef = 3;
                    double PhyDef = 6;
                    double Parry = 11;
                    double Hit = 11;
                    double MinPhyA = 6;
                    double MaxPhyA = 9;
                    double MinMagA = 6;
                    double MaxMagA = 10;

                    CLFramework.Ini ini;
                    ini = new CLFramework.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");

                    string Item1 = ini.GetValue("Chinese.Start", "Item1", "").ToString();
                    string Item2 = ini.GetValue("Chinese.Start", "Item2", "").ToString();
                    string Item3 = ini.GetValue("Chinese.Start", "Item3", "").ToString();
                    string Item4 = ini.GetValue("Chinese.Start", "Item4", "").ToString();
                    string Item5 = ini.GetValue("Chinese.Start", "Item5", "").ToString();

                    string Amount1 = ini.GetValue("Chinese.Start", "Amount1", "").ToString();
                    string Amount2 = ini.GetValue("Chinese.Start", "Amount2", "").ToString();
                    string Amount3 = ini.GetValue("Chinese.Start", "Amount3", "").ToString();
                    string Amount4 = ini.GetValue("Chinese.Start", "Amount4", "").ToString();
                    string Amount5 = ini.GetValue("Chinese.Start", "Amount5", "").ToString();

                    try
                    {
                        AddItem(Convert.ToInt32(Item1), Convert.ToByte(Amount1), 13, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item2), Convert.ToByte(Amount2), 14, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item3), Convert.ToByte(Amount3), 15, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item4), Convert.ToByte(Amount4), 16, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item5), Convert.ToByte(Amount5), 17, Player.CreatingCharID, 0);
                    }
                    catch (Exception)
                    {

                    }

                    AddItem(Item[0], 0, 1, Player.CreatingCharID, 0);
                    AddItem(Item[1], 0, 4, Player.CreatingCharID, 0);
                    AddItem(Item[2], 0, 5, Player.CreatingCharID, 0);
                    AddItem(Item[3], 0, 6, Player.CreatingCharID, 0);

                    MagDef += ObjData.Manager.ItemBase[Item[0]].Defans.MinMagDef;
                    MagDef += ObjData.Manager.ItemBase[Item[1]].Defans.MinMagDef;
                    MagDef += ObjData.Manager.ItemBase[Item[2]].Defans.MinMagDef;
                    PhyDef += ObjData.Manager.ItemBase[Item[0]].Defans.MinPhyDef;
                    PhyDef += ObjData.Manager.ItemBase[Item[1]].Defans.MinPhyDef;
                    PhyDef += ObjData.Manager.ItemBase[Item[2]].Defans.MinPhyDef;
                    Parry += ObjData.Manager.ItemBase[Item[0]].Defans.Parry;
                    Parry += ObjData.Manager.ItemBase[Item[1]].Defans.Parry;
                    Parry += ObjData.Manager.ItemBase[Item[2]].Defans.Parry;
                    Hit += ObjData.Manager.ItemBase[Item[3]].Attack.MinAttackRating;
                    MinPhyA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                    MaxPhyA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                    MinMagA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_LMagAttack;
                    MaxMagA += ObjData.Manager.ItemBase[Item[3]].Attack.Min_HMagAttack;

                    if (Item[3] == 3632 || Item[3] == 3633)
                    {
                        MagDef += ObjData.Manager.ItemBase[251].Defans.MinMagDef;
                        PhyDef += ObjData.Manager.ItemBase[251].Defans.MinPhyDef;
                        Parry += ObjData.Manager.ItemBase[251].Defans.Parry;
                        AddItem(251, 0, 7, Player.CreatingCharID, 0);
                    }
                    if (Item[3] == 3636)
                    {
                        AddItem(62, 250, 7, Player.CreatingCharID, 0);
                    }
                    #endregion

                    DB.query("update character set min_phyatk='" + (int)Math.Round(MinPhyA) +
                            "', max_phyatk='" + Math.Round(MaxPhyA) +
                            "', min_magatk='" + Math.Round(MinMagA) +
                            "', max_magatk='" + Math.Round(MaxMagA) +
                            "', phydef='" + Math.Round(PhyDef) +
                            "', magdef='" + Math.Round(PhyDef) +
                            "', parry='" + Math.Round(Parry) +
                            "', hit='" + Math.Round(Hit) +
                            "' where name='" + name + "'");

                    AddMastery(257, Player.CreatingCharID); //blade
                    AddMastery(258, Player.CreatingCharID); //heuksal
                    AddMastery(259, Player.CreatingCharID); //bow
                    AddMastery(273, Player.CreatingCharID); //cold
                    AddMastery(274, Player.CreatingCharID); //light
                    AddMastery(275, Player.CreatingCharID); //fire
                    AddMastery(276, Player.CreatingCharID); //force
                    client.Send(Packet.ScreenSuccess(1));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region WorldMgr.character check
        public void CharacterCheck(byte[] buff)
        {
            //Wrap our function into a catcher
            try
            {
                //Open our reader
                PacketReader Reader = new PacketReader(buff);
                byte ignored = Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                
                if (CharacterCheck(name))
                    client.Send(Packet.CharacterName(4));
                else 
                    client.Send(Packet.ScreenSuccess(4));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Character Name Check
        public bool CharacterCheck(string name)
        {
            //Set new bool taken
            bool Taken = false;
            //Wrap with catcher
            try
            {
                //If name lenght higher is then 3 characters and lower then 12 characters
                if (name.Length > 3 && name.Length < 12)
                {
                    //Check name in database
                    string dbname = DB.GetData("SELECT name FROM character WHERE name='" + name + "'", "name");
                    //If the name does not excist
                    if (dbname == null)
                    {
                        //Set bool to false
                        Taken = false;
                    }
                    //If name excists, bool is true
                    else Taken = true;
                }
                //Finally if name lenght is wrong taken is true
                else
                {
                    Taken = true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            //Return the bool value
            return Taken;
        }
        #endregion
    }
}
