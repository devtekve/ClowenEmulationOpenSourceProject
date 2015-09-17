using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections;
using CLGameServer.Client;
using CLFramework;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public static void LoadBlues(WorldMgr.character c)
        {
            try
            {
                DB ms = new DB("SELECT * FROM char_items WHERE owner='"+ c.Information.CharacterID +"'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        int a = 10;
                        int b = 11;

                        int id = reader.GetInt32(0);
                        ObjData.itemblue it = new ObjData.itemblue();
                        it.blue = new ArrayList();
                        it.blueamount = new ArrayList();
                        it.totalblue = reader.GetInt32(9);
                        for (int i = 0; i <= it.totalblue; i++)
                        {
                            it.blue.Add(reader.GetString(a));
                            it.blueamount.Add(reader.GetInt32(b));
                            a += 2;
                            b += 2;
                        }

                        ObjData.Manager.ItemBlue[id] = it;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blue error: {0}",ex);
            }
        }
        public static void LoadBluesid(int idinfo)
        {
            try
            {
                DB ms = new DB("SELECT * FROM char_items WHERE id='"+ idinfo +"'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        int a = 10;
                        int b = 11;

                        int id = idinfo;
                        ObjData.itemblue it = new ObjData.itemblue();
                        it.blue = new ArrayList();
                        it.blueamount = new ArrayList();
                        it.totalblue = reader.GetInt32(9);

                        for (int d = 0; d < it.totalblue; d++)
                        {
                            it.blue.Add(reader.GetString(a));
                            it.blueamount.Add(reader.GetInt32(b));
                            a += 2;
                            b += 2;
                        }

                        ObjData.Manager.ItemBlue[id] = it;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blue error: {0}", ex);
            }
        }
        public void AddRemoveBlues(PlayerMgr ch, ObjData.slotItem item, bool add)
        {
            try
            {
                LoadBluesid(item.dbID);
                string name;
                if (ObjData.Manager.ItemBlue.ContainsKey(item.dbID))
                {
                    for (int k = 0; k < ObjData.Manager.ItemBlue[item.dbID].totalblue; k++)
                    {
                        name = Convert.ToString(ObjData.Manager.ItemBlue[item.dbID].blue[k]);
                        switch (name)
                        {
                            case "MATTR_INT":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_INT"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_STR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_STR"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }

                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_LUCK":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_LUCK"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_HP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_HP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_MP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_MP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_REGENHPMP": // hp/mp regen inc(%)
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_REGENHPMP"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.mpregen += Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Blues.hpregen += Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.mpregen -= Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Blues.hpregen -= Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_RESIST_FROSTBITE":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_FROSTBITE"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Frostbite += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Frostbite -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_ESHOCK":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_ESHOCK"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Eshock += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Eshock -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_BURN":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_BURN"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Burn += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Burn -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_POISON":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_POISON"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Poison += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Poison -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_ZOMBIE":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_ZOMBIE"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Zombie += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Zombie -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_STR_3JOB":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_STR_3JOB"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_INT_3JOB":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_INT_3JOB"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_STR_AVATAR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_STR_AVATAR"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_INT_AVATAR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_INT_AVATAR"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_STR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_STR"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_INT":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_INT"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_HR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_HR"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UpdatedStats[item.dbID].Hit = (ch.Character.Stat.Hit / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.Hit += ch.Character.Blues.UpdatedStats[item.dbID].Hit;
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Hit -= ch.Character.Blues.UpdatedStats[item.dbID].Hit;
                                        ch.Character.Blues.UpdatedStats[item.dbID].Hit = 0;
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_ER":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_ER"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UpdatedStats[item.dbID].Parry = (ch.Character.Stat.Parry / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.Parry += ch.Character.Blues.UpdatedStats[item.dbID].Parry;
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Parry -= ch.Character.Blues.UpdatedStats[item.dbID].Parry;
                                        ch.Character.Blues.UpdatedStats[item.dbID].Parry = 0;
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_ER": //parry rate inc
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_ER"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UpdatedStats[item.dbID].Parry = (ch.Character.Stat.Parry / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.Parry += ch.Character.Blues.UpdatedStats[item.dbID].Parry;
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Parry -= ch.Character.Blues.UpdatedStats[item.dbID].Parry;
                                        ch.Character.Blues.UpdatedStats[item.dbID].Parry = 0;
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_HR": //attack rate inc (%)
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_HR"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UpdatedStats[item.dbID].Hit = (ch.Character.Stat.Hit / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.Hit += ch.Character.Blues.UpdatedStats[item.dbID].Hit;
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Hit -= ch.Character.Blues.UpdatedStats[item.dbID].Hit;
                                        ch.Character.Blues.UpdatedStats[item.dbID].Hit = 0;
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_HP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_HP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_MP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_DRUA": // damage increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_DRUA"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UpdatedStats[item.dbID].MinPhyAttack = (ch.Character.Stat.MinPhyAttack / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Blues.UpdatedStats[item.dbID].MaxPhyAttack = (ch.Character.Stat.MaxPhyAttack / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Blues.UpdatedStats[item.dbID].MinMagAttack = (ch.Character.Stat.MinMagAttack / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Blues.UpdatedStats[item.dbID].MaxMagAttack = (ch.Character.Stat.MaxMagAttack / 100) * Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                        ch.Character.Stat.MinPhyAttack += ch.Character.Blues.UpdatedStats[item.dbID].MinPhyAttack;
                                        ch.Character.Stat.MaxPhyAttack += ch.Character.Blues.UpdatedStats[item.dbID].MaxPhyAttack;
                                        ch.Character.Stat.MinMagAttack += ch.Character.Blues.UpdatedStats[item.dbID].MinMagAttack;
                                        ch.Character.Stat.MaxMagAttack += ch.Character.Blues.UpdatedStats[item.dbID].MaxMagAttack;
                                    }
                                    else
                                    {
                                        ch.Character.Stat.MinPhyAttack -= ch.Character.Blues.UpdatedStats[item.dbID].MinPhyAttack;
                                        ch.Character.Stat.MaxPhyAttack -= ch.Character.Blues.UpdatedStats[item.dbID].MaxPhyAttack;
                                        ch.Character.Stat.MinMagAttack -= ch.Character.Blues.UpdatedStats[item.dbID].MinMagAttack;
                                        ch.Character.Stat.MaxMagAttack -= ch.Character.Blues.UpdatedStats[item.dbID].MaxMagAttack;
                                        ch.Character.Blues.UpdatedStats[item.dbID].MinPhyAttack = 0;
                                        ch.Character.Blues.UpdatedStats[item.dbID].MaxPhyAttack = 0;
                                        ch.Character.Blues.UpdatedStats[item.dbID].MinMagAttack = 0;
                                        ch.Character.Blues.UpdatedStats[item.dbID].MaxMagAttack = 0;
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_DARA": // damage absorption
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_DARA"))
                                {
                                    if (add)
                                    {
                                        //ch.Character.Stat.phy_Absorb += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        //ch.Character.Stat.mag_Absorb += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        //ch.Character.Stat.phy_Absorb -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        //ch.Character.Stat.mag_Absorb -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_MDIA": // ignore monster defense
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MDIA"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.MonsterIgnorance += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.MonsterIgnorance -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_HPRG": //hp recovery increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_HPRG"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.hpregen += Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.hpregen -= Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_MPRG": //mp recovery increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MPRG"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.mpregen += Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.mpregen -= Convert.ToDouble(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_RESIST_STUN":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_STUN"))
                                {
                                    if (add)
                                        ch.Character.Blues.Resist_Stun += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Stun -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_CSMP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_CSMP"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_CSMP += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_CSMP -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_DISEASE":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_DISEASE"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Disease += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Disease -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_SLEEP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_SLEEP"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Sleep += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Sleep -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_FEAR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_FEAR"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_Fear += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_Fear -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_NASRUN_UMDU": //damage increase(only @ uniques)
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_NASRUN_UMDU"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.UniqueDMGInc += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.UniqueDMGInc += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_NASRUN_HPNA": //maximum hp increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_NASRUN_HPNA"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Hp += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.SecondHp += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Hp -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.SecondHp -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_NASRUN_MPNA": //maximum mp increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_NASRUN_MPNA"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Mp += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.SecondMP += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Mp -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.Character.Stat.SecondMP -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_NASRUN_BLOCKRATE": //blocking rate increase
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_NASRUN_BLOCKRATE"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.BlockRatio += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Stat.BlockRatio -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_STR_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_STR_SET"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_INT_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_INT_SET"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_ER_SET":
                                break;
                            case "MATTR_HP_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_HP_SET"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();

                                }
                                break;
                            case "MATTR_MP_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_MP_SET"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_LUCK_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_LUCK_SET"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_AVATAR_STR_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_STR_2"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_STR_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_STR_3"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_STR_4":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_STR_4"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_INT_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_INT_2"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_INT_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_INT_3"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_INT_4":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_INT_4"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_AVATAR_MDIA_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MDIA_2"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.MonsterIgnorance += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.MonsterIgnorance -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_MDIA_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MDIA_3"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.MonsterIgnorance += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.MonsterIgnorance -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_MDIA_4":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_MDIA_4"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Blues.MonsterIgnorance += Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                    else
                                    {
                                        ch.Character.Blues.MonsterIgnorance -= Convert.ToInt32(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    }
                                }
                                break;
                            case "MATTR_AVATAR_LUCK":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_LUCK"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_AVATAR_LUCK_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_LUCK_2"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_AVATAR_LUCK_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_LUCK_4"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_AVATAR_LUCK_4":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_AVATAR_LUCK_4"))
                                {

                                    if (add)
                                        ch.Character.Blues.Luck += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Luck -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_RESIST_ALL_SET":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_RESIST_ALL_SET"))
                                {

                                    if (add)
                                        ch.Character.Blues.Resist_All += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Blues.Resist_All -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                }
                                break;
                            case "MATTR_TRADE_STR":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_STR"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_STR_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_STR_2"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_STR_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_STR_3"))
                                {

                                    if (add)
                                    {
                                        ch.Character.Stat.Strength += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Strength -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateStrengthMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateHp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_INT":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_INT"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_INT_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_INT_2"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_INT_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_INT_3"))
                                {
                                    if (add)
                                    {
                                        ch.Character.Stat.Intelligence += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceInfo(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    else
                                    {
                                        ch.Character.Stat.Intelligence -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                        ch.UpdateIntelligenceMinus(Convert.ToSByte(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]));
                                        ch.SetStat();
                                        ch.UpdateMp();
                                    }
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_HP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_HP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_HP_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_HP_2"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_HP_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_HP_3"))
                                {

                                    if (add)
                                        ch.Character.Stat.Hp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Hp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateHp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_MP":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_MP"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_MP_2":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_MP_2"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                            case "MATTR_TRADE_MP_3":
                                if (ObjData.Manager.ItemBlue[item.dbID].blue.Contains("MATTR_TRADE_MP_3"))
                                {

                                    if (add)
                                        ch.Character.Stat.Mp += Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);
                                    else
                                        ch.Character.Stat.Mp -= Convert.ToInt16(ObjData.Manager.ItemBlue[item.dbID].blueamount[k]);

                                    ch.client.Send(Client.Packet.PlayerStat(ch.Character));
                                    ch.UpdateMp();
                                    ch.SavePlayerInfo();
                                }
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[BlueSystem] ItemBlue for ID {0} not found", item.dbID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blue add/remove error {0}", ex);
                Log.Exception(ex);
            }
        }
    }
    
}
