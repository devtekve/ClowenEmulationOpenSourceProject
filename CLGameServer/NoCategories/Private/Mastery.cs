using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void Mastery_Up()
        {
            try
            {
                List<byte> Masteries = new List<byte>();
                DB mastery = new DB("SELECT * FROM mastery WHERE owner='" + Character.Information.CharacterID + "'");
                using (SqlDataReader reader = mastery.Read())
                {
                    while (reader.Read())
                    {
                        Masteries.Add(reader.GetByte(2));
                    }

                }
                int totalmastery = 0;
                int masterylimit = Character.Information.Level * 3;
                if (Character.Information.Race == 1)
                {
                    masterylimit = Character.Information.Level * 2;
                }
                if (Character.Information.GM > 0)
                {
                    masterylimit = Character.Information.Level * 8;
                }
                for (int i = 0; i < Masteries.Count; i++)
                {
                    totalmastery += Masteries[i];
                }
                if (totalmastery < masterylimit)
                {
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    int masteryid = Reader.Int32();
                    byte level = Reader.Byte();
                    byte m_index = MasteryGet(masteryid);

                    if (m_index == 255)
                    {
                        return;
                    }

                    if (Character.Information.SkillPoint > ObjData.Manager.MasteryBase[Character.Stat.Skill.Mastery_Level[m_index]])
                    {
                        if (Character.Stat.Skill.Mastery_Level[m_index] < Character.Information.Level)
                        {
                            if (!(Character.Stat.Skill.Mastery_Level[m_index] == 120))
                            {
                                Character.Stat.Skill.Mastery_Level[m_index]++;
                                Character.Information.SkillPoint -= ObjData.Manager.MasteryBase[Character.Stat.Skill.Mastery_Level[m_index]];

                                client.Send(Packet.InfoUpdate(2, Character.Information.SkillPoint, 0));
                                client.Send(Packet.MasteryUpPacket(masteryid, Character.Stat.Skill.Mastery_Level[m_index]));
                                client.Send(Packet.PlayerStat(Character));
                                SaveMaster();
                            }
                        }
                    }
                }
                else
                {
                    client.Send(Packet.Message(OperationCode.SERVER_MASTERY_UP, Messages.UIIT_STT_SKILL_LEARN_MASTERY_TOTAL_LIMIT));
                    return;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        byte MasteryGet(int id)
        {
            for (byte b = 1; b <= (Character.Information.Race == 1 ? 6 : 7); b++)
            {
                if (Character.Stat.Skill.Mastery[b] == id) 
                {
                    return b;
                }
            }
            Console.WriteLine("MASTERY ID NOT FOUND:{0} Log Out.", Character.Information.Name);
            Disconnect("normal");
            return 255;
        }
        void SaveMaster()
        {
            for (byte Index = 1; Index <= (Character.Information.Race == 1 ? 6 : 7); Index++)
            {
                if (Character.Stat.Skill.Mastery[Index] != 0)
                {
                    DB.query("update mastery set level='" + Character.Stat.Skill.Mastery_Level[Index] + "' where owner='" + Character.Information.CharacterID + "' AND mastery='" + Character.Stat.Skill.Mastery[Index] + "'");
                }
            }
            DB.query("update character set sp='" + Character.Information.SkillPoint + "' where id='" + Character.Information.CharacterID + "'");
        }
        void SaveSkill(int skillid)
        {
            int info = DB.GetRowsCount("SELECT * from saved_skills WHERE owner='" + Character.Information.CharacterID + "' AND Skillid='" + skillid + "'");
            if (info != 0) return;
            DB.query("INSERT INTO saved_skills (skillid, owner, level) VALUES ('" + skillid + "','" + Character.Information.CharacterID + "','1') ");

        }

        public void Mastery_Skill_Up()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int SkillID = Reader.Int32();
            /*if (Array.Exists(ObjData.Manager.SkillBase, GetSkill => GetSkill.ID == SkillID) == false)
            {
                Console.WriteLine("SkillID bulunamadı:{0}", Character.Information.Name);
            }
            */
            if (Character.Information.SkillPoint < ObjData.Manager.SkillBase[SkillID].SkillPoint) { Console.WriteLine("SP LAZIM"); return; }
            int check = DB.GetRowsCount("SELECT * FROM saved_skills WHERE skillid = " + SkillID + " AND owner=" + Character.Information.CharacterID);
            if (check == 1)
            {
                client.Send(Packet.Message(OperationCode.SERVER_SKILL_UPDATE, Messages.UIIT_STT_SKILL_LEARN_MASTERY_LIMIT));
                return;
            }
            else
            {
                Character.Stat.Skill.AmountSkill++;
                Character.Stat.Skill.Skill[Character.Stat.Skill.AmountSkill] = SkillID;
            }
            if (SkillGetOpened(SkillID))
            {
                Character.Information.SkillPoint -= ObjData.Manager.SkillBase[SkillID].SkillPoint;
                client.Send(Packet.InfoUpdate(2, Character.Information.SkillPoint, 0));
                client.Send(Packet.SkillUpdate(SkillID));
                client.Send(Packet.PlayerStat(Character));

                SaveSkill(SkillID);
            }
            else
            {
               return;
            }
        }
        public void AddMastery(short masteryid, int newCharName)
        {
            DB.query("INSERT INTO mastery (owner, mastery) VALUES ('" + newCharName + "','" + masteryid + "')");
        }
        byte MasteryGetPower(int SkillID)
        {
            if (MasteryGet(ObjData.Manager.SkillBase[SkillID].Mastery) != 255)
	        {
                return Character.Stat.Skill.Mastery_Level[MasteryGet(ObjData.Manager.SkillBase[SkillID].Mastery)];
	        }
            Console.WriteLine("MASTERY ID NOT FOUND:{0} Log Out.",Character.Information.Name);
            Disconnect("normal");
            return 255;
        }
        public byte MasteryGetBigLevel
        {
            get
            {
                return Character.Stat.Skill.Mastery_Level.Max();
            }
        }
    }
}
