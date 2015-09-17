using CLGameServer.Client;
using CLFramework;
using System;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        #region Save guide
        public void SaveGuideInfo()
        {
            //Wrap our function inside a catcher
            try
            {
                //Set default value for guidehex
                string GuideHex = "";
                //For 8 repeating
                for (int i = 0; i < 8; ++i)
                {
                    //Num lenght for guideinfo
                    string Numlen = String.Format("{0:X}", Character.Guideinfo.G1[i]);
                    //If lenght is 1, we set the string to 0 + lenght
                    if (Numlen.Length == 1) Numlen = "0" + Numlen;
                    //Set total
                    GuideHex = GuideHex + Numlen;
                }
                //Update database information
                DB.query("update character set GuideData='" + GuideHex + "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }   
        }
        #endregion
        #region Save player hp, mp
        public void SavePlayerHPMP()
        {
                DB.query(string.Format("update character set s_hp='{0}', s_mp='{1}' where id='{2}'",
                      Character.Stat.SecondHp, Character.Stat.SecondMP, Character.Information.CharacterID));
        } 
        #endregion
        #region Save player information
        public void SavePlayerInfo()
        {
            //Wrap our function inside a catcher
            try
            {
                DB.query("update character set min_phyatk='" + Convert.ToInt32(Math.Round(Character.Stat.MinPhyAttack)) +
                    "', max_phyatk='" + Convert.ToInt32(Math.Round(Character.Stat.MaxPhyAttack)) +
                    "', min_magatk='" + Convert.ToInt32(Math.Round(Character.Stat.MinMagAttack)) +
                    "', max_magatk='" + Convert.ToInt32(Math.Round(Character.Stat.MaxMagAttack)) +
                    "', phydef='" + Convert.ToInt32(Math.Round(Character.Stat.PhyDef - Character.Stat.uPhyDef)) +
                    "', magdef='" + Convert.ToInt32(Math.Round(Character.Stat.MagDef - Character.Stat.uMagDef)) +
                    "', hit='" + Convert.ToInt16(Math.Round(Character.Stat.Hit)) +
                    "', parry='" + Convert.ToInt16(Math.Round(Character.Stat.Parry)) +
                    "', hp='" + Character.Stat.Hp +
                    "', mp='" + Character.Stat.Mp +
                    "', s_hp='" + Character.Stat.SecondHp +
                    "', s_mp='" + Character.Stat.SecondMP +
                    "', attribute='" + Character.Information.Attributes +
                    "', strength='" + Character.Stat.Strength +
                    "', intelligence='" + Character.Stat.Intelligence +
                    "', experience='" + Convert.ToInt64(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "', level='" + Character.Information.Level +
                    "', mag_absord='" + Character.Stat.mag_Absorb +
                    "', phy_absord='" + Character.Stat.phy_Absorb +
                    "' where id='" + Character.Information.CharacterID + "'");
                //Save guid information
                SaveGuideInfo();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            } 
        }
        #endregion
        #region Save exp
        public void SavePlayerExperince()
        {
                //Update database information
                DB.query("update character set  experience='" + Convert.ToUInt64(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "' where id='" + Character.Information.CharacterID + "'");
        }
        #endregion
        #region Save Attack Pet Exp
        public void SaveAttackPetExp()
        {
                DB.query("update pets set pet_experience='" + Convert.ToUInt64(Character.Attackpet.Details.exp) +"' where id='" + Character.Attackpet.Details.UniqueID + "'");
        }
        #endregion
        #region Save position
        protected void SavePlayerPosition()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database
                if (!FileDB.CheckCave(Character.Position.xSec, Character.Position.ySec))
                {
                    DB.query("update character set xsect='" + Character.Position.xSec +
                        "', ysect='" + Character.Position.ySec +
                        "', xpos='" + Formule.packetx(Character.Position.x, Character.Position.xSec) +
                        "', ypos='" + Formule.packety(Character.Position.y, Character.Position.ySec) +
                        "', zpos='" + Math.Round(Character.Position.z) +
                        "' where id='" + Character.Information.CharacterID + "'");
                }
                else
                {
                    DB.query("update character set xsect='" + Character.Position.xSec +
                       "', ysect='" + Character.Position.ySec +
                       "', xpos='" + Formule.cavepacketx(Character.Position.x) +
                       "', ypos='" + Formule.cavepackety(Character.Position.y) +
                       "', zpos='" + Math.Round(Character.Position.z) +
                       "' where id='" + Character.Information.CharacterID + "'");
                }
               
               }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Save last point of death or return scroll usage
        protected void SavePlayerReturn()
        {
                //Update database
                DB.query("update character_rev set revxsec='" + Character.Position.xSec +
                    "', revysec='" + Character.Position.ySec +
                    "', revx='" + Formule.packetx(Character.Position.x, Character.Position.xSec) +
                    "', revy='" +Formule.packety(Character.Position.y, Character.Position.ySec) +
                    "', revz='" + Math.Round(Character.Position.z) +
                    "' where charname='" + Character.Information.Name + "'");
        } 
        #endregion
        #region Set Balance
        protected void SetBalance()
        {
                //Set maxstats info
                int maxstat = 28 + Character.Information.Level * 4;
                //Set physical and magical balance
                Character.Information.Phy_Balance = (byte)(99 - (100 * 2 / 3 * (maxstat - Character.Stat.Strength) / maxstat));
                Character.Information.Mag_Balance = (byte)(100 * Character.Stat.Intelligence / maxstat);
        }
        #endregion
        #region Save gold
        protected void SaveGold()
        {
                //Update database with inventory gold
                DB.query("update character set gold='" + Character.Information.Gold +
                        "' where id='" + Character.Information.CharacterID + "'");
                //Update database with storage gold
                DB.query("update users set gold='" + Character.Account.StorageGold +
                    "' where id='" + Player.AccountName + "'");
        }
        protected void SaveGuildGold()
        {
                //Update database with inventory gold
                DB.query("update guild set guild_storage_gold='" + Character.Network.Guild.StorageGold +
                        "' where id='" + Character.Network.Guild.Guildid + "'");
        }
        #endregion
        #region Save quick bar & autopotion
        public void Save()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                string player_path;
                byte[] file;
                //Switch on byte
                switch (Reader.Byte())
                {
                    case 1:
                        //Save quickbar information
                        player_path = Environment.CurrentDirectory + @"\PlayerData\Hotkey\" + Character.Information.CharacterID + ".ClientSettings";
                        file = System.IO.File.ReadAllBytes(player_path);

                        byte Slot = Reader.Byte();
                        byte sType = Reader.Byte();

                        Slot *= 5;

                        file[Slot] = sType;
                        file[Slot + 1] = Reader.Byte();
                        file[Slot + 2] = Reader.Byte();
                        file[Slot + 3] = Reader.Byte();
                        file[Slot + 4] = Reader.Byte();
                        System.IO.File.WriteAllBytes(player_path, file);
                        break;
                    case 2:
                        //Save autopotion information
                        player_path = Environment.CurrentDirectory + @"\PlayerData\AutoPotion\" + Character.Information.CharacterID + ".ClientSettings";
                        file = System.IO.File.ReadAllBytes(player_path);
                        file[0] = Reader.Byte();
                        file[1] = Reader.Byte();
                        file[2] = Reader.Byte();
                        file[3] = Reader.Byte();
                        file[4] = Reader.Byte();
                        file[5] = Reader.Byte();
                        file[6] = Reader.Byte();
                        System.IO.File.WriteAllBytes(player_path, file);
                        UpdateHp();
                        UpdateMp();
                        break;
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }        
        }
        #endregion
        #region Save return point
        public void SavePlace()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int ObjectID = reader.Int32();
                //Get object information
                WorldMgr.Monsters o = Helpers.GetInformation.GetObject(ObjectID);
                //Defaul value for type
                byte type = 0;
                //Switch by object name
                switch (ObjData.Manager.ObjectBase[o.ID].Name)
                {
                    case "STORE_CH_GATE":
                        type = 1;
                        break;
                    case "STORE_WC_GATE":
                        type = 2;
                        break;
                    case "STORE_KT_GATE":
                        type = 5;
                        break;
                    case "STORE_EU_GATE":
                        type = 20;
                        break;
                    case "STORE_CA_GATE":
                        type = 25;
                        break;
                    case "STORE_SD_GATE1":
                        type = 175;
                        break;
                    case "STORE_SD_GATE2":
                        type = 176;
                        break;
                }
                //Set new return global information
                Character.Information.Place = type;
                //Update database
                DB.query("update character set savearea='" + Character.Information.Place + "' where id='" + Character.Information.CharacterID + "'");
                //Send confirmation packet
                client.Send(Packet.UpdatePlace());
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
    }
}
