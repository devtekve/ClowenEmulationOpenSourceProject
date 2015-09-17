using CLFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLGameServer.WorldMgr
{
    public partial class Monsters
    {
        public void SetExperience()
        {
            try
            {
                //Check death
                if (GetDie)
                {
                    //Set default parameters
                    int percent = 0;
                    //Exp information of the monster
                    long exp = ObjData.Manager.ObjectBase[ID].Exp;
                    //Sp information
                    int sp = 119;
                    //Level default false
                    bool level = false;
                    //Main perfect information
                    byte mainpercent = 100;
                    //Set quick info to player
                    PlayerMgr player;
                    //If Agro list isnt empty
                    if (AgressiveDMG != null)
                        //Check how many have agro state
                        for (byte b = 0; b < AgressiveDMG.Count; b++)
                        {
                            //Make sure the player isnt null
                            if (AgressiveDMG[b].playerID != 0)
                            {
                                //Get player information
                                player = Helpers.GetInformation.GetPlayer(AgressiveDMG[b].playerID);
                                //Again double check to make sure the called information isnt null
                                if (player != null)
                                {
                                    //Set definition for stat attributes
                                    short stat = player.Character.Information.Attributes;
                                    //Calculate the damage dealt of the player and divide it by the monster type and hp total.
                                    percent = AgressiveDMG[b].playerDMD * 100 / ObjData.Manager.ObjectBase[ID].HP * Kat;
                                    //If the % is higher or equals 100%, we reset our % to normal
                                    if (percent >= mainpercent)
                                        percent = mainpercent;
                                    //Set default bool info for level
                                    level = false;
                                    //Make sure our % isnt 0 , so we dont do none needed actions
                                    if (mainpercent > 0)
                                    {
                                        //If the player is currently in a party
                                        if (player.Character.Network.Party != null)
                                        {
                                            //Set definition for the party info
                                            party ept = player.Character.Network.Party;
                                            //Set party type information from our party
                                            PlayerMgr.PartyTypes ptType = (PlayerMgr.PartyTypes)ept.Type;
                                            //If its a non shared party
                                            if (ptType == PlayerMgr.PartyTypes.NONSHARE_NO_PERMISSION || ptType == PlayerMgr.PartyTypes.NONSHARE)
                                            {
                                                //Get gap information of the players mastery level
                                                int gap = Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel) * 10;
                                                if (gap >= 90) gap = 90;

                                                mainpercent -= (byte)percent;
                                                //Premium tickets should be added here to increase the exp
                                                exp *= Helpers.Settings.Rate.Experience;
                                                exp -= (exp * ((Math.Abs(ObjData.Manager.ObjectBase[ID].Level - player.Character.Information.Level) - Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel)) * 10)) / 100;
                                                // kat == the type of the mob. this is the multiplier of the mob's HP and the amount of exp what the mob gives.
                                                byte monstertype = Kat;

                                                if (monstertype == 20)
                                                    monstertype = 25;

                                                exp *= monstertype;
                                                exp = (exp * percent) / 100;
                                                exp = (exp * (100 - gap)) / 100;

                                                if (Math.Abs(ObjData.Manager.ObjectBase[ID].Level - player.Character.Information.Level) < 10)
                                                {
                                                    sp = (sp * (100 + gap)) / 100;
                                                    sp *= monstertype;
                                                    sp *= Helpers.Settings.Rate.SkillPoint;
                                                }
                                                else sp = 10;
                                                if (exp <= 1) exp = 1;
                                                if (sp <= 1) sp = 10;
                                                //simple non-share pt.
                                                if (ept.Members.Count > 1)
                                                {

                                                    if (Type == 0) //normal mob
                                                    {
                                                        exp *= 1;
                                                        sp *= 1;
                                                    }
                                                    else if (Type == 1) //champion
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (Type == 3)// Unique
                                                    {
                                                        exp *= 7;
                                                        sp *= 7;
                                                    }
                                                    else if (Type == 4) //giant
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (Type == 6) //elite
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (Type == 16) //normal ptmob
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (Type == 17) //champion ptmob
                                                    {
                                                        exp *= 4;
                                                        sp *= 4;
                                                    }
                                                    else if (Type == 20)//party giant
                                                    {
                                                        exp *= 6;
                                                        sp *= 6;
                                                    }

                                                    switch (ept.Members.Count)
                                                    {
                                                        case 2:
                                                            exp += (long)(exp * 0.05);
                                                            sp += (int)(sp * 0.05);
                                                            break;
                                                        case 3:
                                                            exp += (long)(exp * 0.05) * 2;
                                                            sp += (int)(sp * 0.05) * 2;
                                                            break;
                                                        case 4:
                                                            exp += (long)(exp * 0.05) * 3;
                                                            sp += (int)(sp * 0.05) * 3;
                                                            break;
                                                    }
                                                }
                                            }

                                            if (ptType == PlayerMgr.PartyTypes.EXPSHARE_NO_PERMISSION || ptType == PlayerMgr.PartyTypes.EXPSHARE || ptType == PlayerMgr.PartyTypes.FULLSHARE || ptType == PlayerMgr.PartyTypes.FULLSHARE_NO_PERMISSION)
                                            {
                                                if (ept.Members.Count > 1)
                                                {

                                                    if (Type == 0) //normal mob
                                                    {
                                                        exp *= 1;
                                                        sp *= 1;
                                                    }
                                                    else if (Type == 1) //champion
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (Type == 3)// Unique
                                                    {
                                                        exp *= 7;
                                                        sp *= 7;
                                                    }
                                                    else if (Type == 4) //giant
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (Type == 6) //elite
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (Type == 16) //normal ptmob
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (Type == 17) //champion ptmob
                                                    {
                                                        exp *= 4;
                                                        sp *= 4;
                                                    }
                                                    else if (Type == 20)//party giant
                                                    {
                                                        exp *= 6;
                                                        sp *= 6;
                                                    }
                                                    CalcSharedPartyExpSp(ObjData.Manager.ObjectBase[ID].Exp, ept, player, ref exp);
                                                }
                                            }
                                        }
                                        //Player not in party
                                        else
                                        {
                                            int gap = Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel) * 10;
                                            if (gap >= 90) gap = 90;

                                            mainpercent -= (byte)percent;

                                            if (player.Character.Information.Level != ObjData.Manager.ObjectBase[ID].Level)
                                                exp -= (exp * ((Math.Abs(ObjData.Manager.ObjectBase[ID].Level - player.Character.Information.Level) - Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel)) * 10)) / 100;

                                            if (Type == 0) //normal mob
                                            {
                                                exp *= 1;
                                                sp *= 1;
                                            }
                                            else if (Type == 1) //champion
                                            {
                                                exp *= 2;
                                                sp *= 2;
                                            }
                                            else if (Type == 3)// Unique
                                            {
                                                exp *= 7;
                                                sp *= 7;
                                            }
                                            else if (Type == 4) //giant
                                            {
                                                exp *= 3;
                                                sp *= 3;
                                            }
                                            else if (Type == 6) //elite
                                            {
                                                exp *= 2;
                                                sp *= 2;
                                            }
                                            else if (Type == 16) //normal ptmob
                                            {
                                                exp *= 3;
                                                sp *= 3;
                                            }
                                            else if (Type == 17) //champion ptmob
                                            {
                                                exp *= 4;
                                                sp *= 4;
                                            }
                                            else if (Type == 20)//party giant
                                            {
                                                exp *= 6;
                                                sp *= 6;
                                            }
                                            exp = (exp * percent) / 100;
                                            exp = (exp * (97 + gap)) / 100;

                                            if (player.Character.Information.Level == 140 && player.Character.Information.XP >= 130527554553)
                                            {
                                                exp = 0;
                                            }

                                            if (Math.Abs(ObjData.Manager.ObjectBase[ID].Level - player.Character.Information.Level) < 10)
                                            {
                                                int gaplevel = ObjData.Manager.ObjectBase[ID].Level - player.Character.Information.Level;
                                                sp = (sp * (100 + gaplevel)) / 100;
                                                sp *= Kat;
                                                sp *= Helpers.Settings.Rate.SkillPoint;

                                                exp = (exp * (100 + gaplevel)) / 100;
                                                exp *= Kat;
                                                exp *= Helpers.Settings.Rate.Experience;
                                            }
                                            else
                                            {
                                                exp = 10;
                                                sp = 10;
                                            }

                                            if (exp <= 1) exp = 1;
                                            if (sp <= 1) sp = 10;
                                        }
                                        player.Character.Information.XP += exp;

                                        while (player.Character.Information.XP >= ObjData.Manager.LevelData[player.Character.Information.Level])
                                        {
                                            player.Character.Information.XP -= ObjData.Manager.LevelData[player.Character.Information.Level];
                                            if (player.Character.Information.XP < 1) player.Character.Information.XP = 0;
                                            stat += 3;
                                            player.Character.Information.Attributes += 3;
                                            player.Character.Information.Level++;
                                            player.Character.Stat.Intelligence++;
                                            player.Character.Stat.Strength++;
                                            player.UpdateIntelligenceInfo(1);
                                            player.UpdateStrengthInfo(1);
                                            player.SetStat();
                                            level = true;
                                        }


                                        SetSp(player, sp);
                                        player.Character.Network.Guild.LastDonate += 2;
                                        if (player.Character.Network.Guild.Guildid != 0)
                                        {
                                            if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                            {
                                                int gpinformation = Rnd.Next(1, 9);//Need to make formula
                                                DB.query("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                //Reload information
                                                player.LoadPlayerGuildInfo(false);
                                                // set new amount to every guild members guild class
                                                foreach (int m in player.Character.Network.Guild.Members)
                                                {
                                                    if (m != 0)
                                                    {
                                                        PlayerMgr gmember = Helpers.GetInformation.GetPlayerMainid(m);
                                                        if (gmember != null)
                                                        {
                                                            gmember.client.Send(Client.Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                        }
                                                    }
                                                }
                                                player.Character.Network.Guild.LastDonate = 0;
                                            }
                                        }
                                        //Check pet level information
                                        if (player.Character.Attackpet.Active)
                                        {
                                            while (player.Character.Attackpet.Details.exp >= ObjData.Manager.LevelData[player.Character.Attackpet.Details.exp])
                                            {
                                                player.Character.Attackpet.Details.exp -= ObjData.Manager.LevelData[player.Character.Attackpet.Details.Level];
                                                if (player.Character.Attackpet.Details.exp < 1)
                                                    player.Character.Attackpet.Details.exp = 0;
                                                /*
                                                stat += 3;
                                                player.Character.Information.Attributes += 3;
                                                player.Character.Information.Level++;
                                                player.Character.Stat.Intelligence++;
                                                player.Character.Stat.Strength++;
                                                player.UpdateIntelligenceInfo(1);
                                                player.UpdateStrengthInfo(1);
                                                player.SetStat();
                                                 */
                                                level = true;
                                            }
                                        }

                                        if (level)
                                        {
                                            if (player.Character.Network.Guild.Guildid != 0)
                                            {
                                                if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                                {
                                                    int gpinformation = Rnd.Next(1, 9);//Need to make formula
                                                    DB.query("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                    player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                    //Reload information
                                                    player.LoadPlayerGuildInfo(false);
                                                    // set new amount to every guild members guild class
                                                    foreach (int m in player.Character.Network.Guild.Members)
                                                    {
                                                        if (m != 0)
                                                        {
                                                            PlayerMgr gmember = Helpers.GetInformation.GetPlayerMainid(m);
                                                            if (gmember != null)
                                                            {
                                                                gmember.client.Send(Client.Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                            }
                                                        }
                                                    }
                                                    player.Character.Network.Guild.LastDonate = 0;
                                                }
                                            }

                                            if (player.Character.Attackpet.Active)
                                            {
                                                //test
                                                player.Send(Client.Packet.Player_LevelUpEffect(player.Character.Attackpet.Details.UniqueID));
                                                player.client.Send(Client.Packet.Player_getExp(player.Character.Attackpet.Details.UniqueID, exp, sp, stat));

                                            }
                                            if (player.Character.Information.Level == 110 && player.Character.Information.XP >= 4000000000)
                                            {
                                                exp = 0;
                                            }
                                            player.Send(Client.Packet.Player_LevelUpEffect(player.Character.Information.UniqueID));
                                            player.client.Send(Client.Packet.Player_getExp(player.Character.Action.Target, exp, sp, stat));
                                            player.SavePlayerInfo();
                                        }
                                        else
                                        {
                                            if (player.Character.Network.Guild.Guildid != 0)
                                            {
                                                if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                                {
                                                    int gpinformation = Rnd.Next(1, 9);//Need to make formula
                                                    DB.query("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                    player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                    //Reload information
                                                    player.LoadPlayerGuildInfo(false);
                                                    // set new amount to every guild members guild class
                                                    foreach (int m in player.Character.Network.Guild.Members)
                                                    {
                                                        if (m != 0)
                                                        {
                                                            PlayerMgr gmember = Helpers.GetInformation.GetPlayerMainid(m);
                                                            if (gmember != null)
                                                            {
                                                                gmember.client.Send(Client.Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                            }
                                                        }
                                                    }
                                                    player.Character.Network.Guild.LastDonate = 0;
                                                }
                                            }
                                            if (player.Character.Information.Level == 110 && player.Character.Information.XP >= 4000000000)
                                            {
                                                exp = 0;
                                            }
                                            //Player experience
                                            player.client.Send(Client.Packet.Player_getExp(player.Character.Action.Target, exp, sp, 0));
                                            player.SavePlayerExperince();


                                            // Attack pet experience
                                            if (player.Character.Attackpet.Active)
                                            {
                                                bool petlevel;
                                                player.Character.Attackpet.Details.exp += exp * 2;

                                                while (player.Character.Attackpet.Details.exp >= ObjData.Manager.LevelData[player.Character.Attackpet.Details.exp])
                                                {
                                                    player.Character.Attackpet.Details.exp -= ObjData.Manager.LevelData[player.Character.Attackpet.Details.Level];
                                                    if (player.Character.Attackpet.Details.exp < 1)
                                                        player.Character.Attackpet.Details.exp = 0;
                                                    petlevel = true;
                                                }

                                                // Add exp
                                                player.client.Send(Client.Packet.PetSpawn(player.Character.Attackpet.Details.UniqueID, 3, player.Character.Attackpet.Details));
                                                // Save pet exp
                                                player.SaveAttackPetExp();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetExperience :error {0}", ex);
                Log.Exception(ex);
            }
        }
        public void CalcSharedPartyExpSp(int paramexp, party pt, PlayerMgr targetplayer, ref long outexp)
        {
            try
            {
                byte mlv = ObjData.Manager.ObjectBase[ID].Level;
                // party average
                int k = 0;
                List<int> NearbyMembers = new List<int>(9);
                if (pt.Members.Count != 0)
                {
                    double playerDist;
                    foreach (int memb in pt.Members)
                    {
                        PlayerMgr i = Helpers.GetInformation.GetPlayer(memb);
                        //playerDist = Formule.gamedistance(targetplayer.Character.Position.x, targetplayer.Character.Position.y, i.Character.Position.x, i.Character.Position.y);
                        playerDist = Formule.gamedistance(targetplayer.Character.Position, i.Character.Position);
                        if (playerDist >= 50)
                        {
                            NearbyMembers.Add(i.Character.Information.UniqueID);
                        }
                    }
                    foreach (int l in NearbyMembers)
                    {
                        k += Helpers.GetInformation.GetPlayer(l).Character.Information.Level;
                    }
                    k = (int)(k / pt.Members.Count);
                    //k = Ã¡tlag.
                    foreach (int mem in NearbyMembers)
                    {

                        PlayerMgr ch = Helpers.GetInformation.GetPlayer(mem);
                        int ptsp = 97;
                        //This isn't the right formula. TODO: We must find the right one!
                        int ptexp = (int)((((paramexp / mlv) + (mlv - k)) * mlv) / k) * ch.Character.Information.Level;
                        ptexp *= Helpers.Settings.Rate.Experience;
                        byte kat = Kat;
                        if (kat == 20) kat = 25;
                        ptexp *= kat; //we multiply the exp according to type of the mob.
                        //TODO: premium ticket
                        //gap:
                        ptexp -= (ptexp * (ch.Character.Information.Level) - Math.Abs(ch.Character.Information.Level - ch.MasteryGetBigLevel)) * 10 / 100;
                        if (ch.Character.Information.Level == 110 && ch.Character.Information.XP >= 4000000000) ptexp = 0;
                        //we calculate the amount of sp:
                        if (Math.Abs(ObjData.Manager.ObjectBase[ID].Level - k) < 10)
                        {
                            int gap = Math.Abs(ch.Character.Information.Level - ch.MasteryGetBigLevel) * 10;
                            if (gap >= 90) gap = 90;
                            ptsp = (ptsp * (100 + gap)) / k; //Instead of 100 I share with the avareage of the party, so we get a proportionate number.
                            ptsp *= kat;
                            ptsp *= Helpers.Settings.Rate.SkillPoint;
                        }
                        //Send total to all in party (Set exp from formula)
                        SetPartyMemberExp(ch, (long)ptexp, ch.Character.Information.Attributes, (long)ptsp);
                        SetSp(ch, ptsp);
                        if (ch.Character.Network.Guild.Guildid != 0)
                        {
                            if (Math.Abs(ch.Character.Network.Guild.LastDonate - ch.Character.Network.Guild.DonateGP) >= 10)
                            {
                                DB.query("UPDATE guild_members SET guild_points='" + ch.Character.Network.Guild.DonateGP + "' WHERE guild_member_id='" + ch.Character.Information.CharacterID + "'");
                                ch.Character.Network.Guild.LastDonate = ch.Character.Network.Guild.DonateGP;

                                // set new amount to every guild members guild class
                                foreach (int m in ch.Character.Network.Guild.Members)
                                {
                                    int targetindex = ch.Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                                    if (ch.Character.Network.Guild.MembersInfo[targetindex].Online)
                                    {
                                        PlayerMgr sys = Helpers.GetInformation.GetPlayer(m);

                                        // here comes the messy way 
                                        ObjData.guild_player mygp = new ObjData.guild_player();
                                        int myindex = 0;
                                        foreach (ObjData.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                                        {
                                            if (gp.MemberID == ch.Character.Information.CharacterID)
                                            {
                                                mygp = gp;
                                                mygp.DonateGP = ch.Character.Network.Guild.DonateGP;
                                                break;
                                            }
                                            myindex++;
                                        }
                                        sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                                    }
                                }

                                ch.Character.Network.Guild.Send(Client.Packet.GuildUpdate(ch.Character, 9, 0, 0, 0));
                            }
                        }
                        outexp = ptexp;
                    }
                }
                else return;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static void SetPartyMemberExp(PlayerMgr ch, long expamount, short stat, long sp)
        {
            try
            {
                ch.Character.Information.XP += expamount;
                bool level = false;
                while (ch.Character.Information.XP >= ObjData.Manager.LevelData[ch.Character.Information.Level])
                {
                    ch.Character.Information.XP -= ObjData.Manager.LevelData[ch.Character.Information.Level];
                    if (ch.Character.Information.XP < 1) ch.Character.Information.XP = 0;
                    stat += 3;
                    ch.Character.Information.Attributes += 3;
                    ch.Character.Information.Level++;
                    ch.Character.Stat.Intelligence++;
                    ch.Character.Stat.Strength++;
                    ch.UpdateIntelligenceInfo(1);
                    ch.UpdateStrengthInfo(1);
                    ch.SetStat();
                    level = true;
                }
                if (level)
                {
                    if (ch.Character.Network.Guild.Guildid != 0)
                    {
                        // 1 question again where we  set info to databse at lvlup? ah got it :)
                        ch.Character.Network.Guild.Send(Client.Packet.GuildUpdate(ch.Character, 8, 0, 0, 0));

                        // set new amount to every guild members guild class
                        foreach (int m in ch.Character.Network.Guild.Members)
                        {
                            int targetindex = ch.Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                            if (ch.Character.Network.Guild.MembersInfo[targetindex].Online)
                            {
                                PlayerMgr sys = Helpers.GetInformation.GetPlayer(m);

                                // here comes the messy way 
                                ObjData.guild_player mygp = new ObjData.guild_player();
                                int myindex = 0;
                                foreach (ObjData.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                                {
                                    if (gp.MemberID == ch.Character.Information.CharacterID)
                                    {
                                        mygp = gp;
                                        mygp.Level = ch.Character.Information.Level;
                                        break;
                                    }
                                    myindex++;
                                }
                                sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                            }
                        }
                    }
                    ch.Send(Client.Packet.Player_LevelUpEffect(ch.Character.Information.UniqueID));
                    ch.client.Send(Client.Packet.Player_getExp(ch.Character.Information.UniqueID, expamount, sp, stat));
                    ch.SavePlayerInfo();
                }
                else
                {
                    ch.client.Send(Client.Packet.Player_getExp(ch.Character.Information.UniqueID, expamount, sp, 0));
                    ch.SavePlayerExperince();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static void SetSp(PlayerMgr sys, int sp)
        {
            try
            {
                sys.Character.Information.SkillPoint += ((sp + sys.Character.Information.SpBar) / 400);
                sys.Character.Information.SpBar = ((sp + sys.Character.Information.SpBar) % 400);
                sys.client.Send(Client.Packet.InfoUpdate(2, sys.Character.Information.SkillPoint, 0));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
