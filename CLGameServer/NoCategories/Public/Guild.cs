using System;
using System.Linq;
using System.Collections.Generic;

namespace CLGameServer
{
    public partial class Systems
    {
        void HandleRegisterIcon()
        {
            try
            {
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                int iconlenght = reader.Int32();
                string icon = reader.Text();
                reader.Close();
                
                string convertedicon = ConvertToHex(icon);
                //Save output to .dat file in hex formatting.


            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild icon register error {0}", ex);
            }
        }
        public string ConvertToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        /////////////////////////////////////////////////////////////////////////
        //Guild War
        /////////////////////////////////////////////////////////////////////////
        void GuildWarGold()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int guildid = Reader.Int32();
                Reader.Close();

                if (Character.Guild.GuildWarGold == 0)
                {
                    //Send Packet Message No War Gold Received
                    client.Send(Packet.GuildWarMsg(2));
                }
                else
                {
                    //Sniff packet for war gold
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////
        //Load guild members
        /////////////////////////////////////////////////////////////////////////
        #region Load guild members
        public void LoadGuildMembers()
        {
            try
            {
                LoadGuildMemberIds(Character.Network.Guild.Guildid, ref Character.Network.Guild.Members);

                foreach (int m in Character.Network.Guild.Members)
                {
                    Global.guild_player PlayerGuild = new Global.guild_player();
                    PlayerGuild.MemberID = m;

                    DB ms = new DB("SELECT * FROM character WHERE id='" + m + "'");
                    using (System.ObjData.Manager.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            PlayerGuild.Model = reader.GetInt32(3);
                            PlayerGuild.Xsector = reader.GetByte(16);
                            PlayerGuild.Ysector = reader.GetByte(17);
                            PlayerGuild.Level = reader.GetByte(5);
                            PlayerGuild.Name = reader.GetString(2);
                            PlayerGuild.Online = (reader.GetInt32(47) == 1);
                            if (PlayerGuild.Online)
                            {
                                Systems sys = GetPlayerMainid(m);
                                if (sys != null)
                                    Character.Network.Guild.MembersClient.Add(sys.client);
                            }
                        }
                    }
                    ms.Close();

                    DB ms2 = new DB("SELECT * FROM guild_members WHERE guild_member_id='" + m + "'");
                    using (System.ObjData.Manager.SqlClient.SqlDataReader reader2 = ms2.Read())
                    {
                        while (reader2.Read())
                        {
                            PlayerGuild.joinRight = (reader2.GetByte(7) == 1);
                            PlayerGuild.withdrawRight = (reader2.GetByte(8) == 1);
                            PlayerGuild.unionRight = (reader2.GetByte(9) == 1);
                            PlayerGuild.guildstorageRight = (reader2.GetByte(10) == 1);
                            PlayerGuild.noticeeditRight = (reader2.GetByte(11) == 1);
                            PlayerGuild.FWrank = reader2.GetByte(6);
                            PlayerGuild.DonateGP = reader2.GetInt32(4);
                            PlayerGuild.Rank = reader2.GetByte(3);
                        }
                    }

                    ms2.Close();
                    Character.Network.Guild.MembersInfo.Add(PlayerGuild);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Load guild information
        /////////////////////////////////////////////////////////////////////////
        #region Load guild info
        public void LoadPlayerGuildInfo(bool logon)
        {
            try
            {
                DB ms = new DB("SELECT * FROM guild_members WHERE guild_member_id='" + Character.Information.CharacterID + "'");
                using (System.ObjData.Manager.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Network.Guild.Guildid = reader.GetInt32(1);
                        Character.Network.Guild.GrantName = reader.GetString(5);
                        Character.Network.Guild.FWrank = reader.GetByte(6);
                        Character.Network.Guild.DonateGP = reader.GetInt32(4);
                        Character.Network.Guild.LastDonate = Character.Network.Guild.DonateGP;
                        Character.Network.Guild.joinRight = (reader.GetByte(7) == 1);
                        Character.Network.Guild.withdrawRight = (reader.GetByte(8) == 1);
                        Character.Network.Guild.unionRight = (reader.GetByte(9) == 1);
                        Character.Network.Guild.guildstorageRight = (reader.GetByte(10) == 1);
                        Character.Network.Guild.noticeeditRight = (reader.GetByte(11) == 1);
                    }
                }
                ms.Close();

                DB ms2 = new DB("SELECT * FROM guild WHERE id='" + Character.Network.Guild.Guildid + "'");
                using (System.ObjData.Manager.SqlClient.SqlDataReader reader2 = ms2.Read())
                {
                    while (reader2.Read())
                    {
                        Character.Network.Guild.Name = reader2.GetString(1);
                        Character.Network.Guild.Level = reader2.GetByte(2);
                        Character.Network.Guild.PointsTotal = reader2.GetInt32(3);
                        Character.Network.Guild.NewsTitle = reader2.GetString(4);
                        Character.Network.Guild.NewsMessage = reader2.GetString(5);
                        Character.Network.Guild.StorageSlots = reader2.GetInt32(7);
                        Character.Network.Guild.Wargold = reader2.GetInt32(8);
                        Character.Network.Guild.StorageGold = reader2.GetInt64(11);
                        //Character.Network.Guild.MasterID = reader.GetInt32(9);
                    }
                }
                ms2.Close();

                DB ms3 = new DB("SELECT * FROM guild_members WHERE guild_id='" + Character.Network.Guild.Guildid + "'");
                Character.Network.Guild.TotalMembers = Convert.ToByte(ms3.Count());

                //Only load on player login
                if (logon)
                {
                    LoadGuildMembers();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Console.WriteLine("LoadPlayerGuildInfo error {0}", ex);
            }

        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Member id's
        /////////////////////////////////////////////////////////////////////////
        #region Guild member id's
        public void LoadGuildMemberIds(int guildid, ref List<int> MemberIDs)
        {
            try
            {
                DB ms = new DB("SELECT * FROM guild_members WHERE guild_id='" + guildid + "'");
                using (System.ObjData.Manager.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        int memberid = reader.GetInt32(2);
                        MemberIDs.Add(memberid);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Create New Guild
        /////////////////////////////////////////////////////////////////////////
        #region Guild creation
        void GuildCreate()
        {
            try
            {
                //Extra check if user is allready in guild.
                if (Character.Network.Guild.Guildid != 0) return;

                //Read client packet ObjData.Manager.
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int infoid = Reader.Int32();
                short guildname = Reader.Int16();
                string guildnameinfo = Reader.String(guildname);
                string charactername = Character.Information.Name;
                Reader.Close();
                //Will write global class for special chars later.
                if (guildnameinfo.Contains("[")) return;
                if (guildnameinfo.Contains("]")) return;
                if (guildnameinfo.Contains("(")) return;
                if (guildnameinfo.Contains(")")) return;
                if (guildnameinfo.Contains("@")) return;
                if (guildnameinfo.Contains("#")) return;
                if (guildnameinfo.Contains("$")) return;
                if (guildnameinfo.Contains("^")) return;
                if (guildnameinfo.Contains("&")) return;
                if (guildnameinfo.Contains("*")) return;
                if (guildnameinfo.Contains("+")) return;
                if (guildnameinfo.Contains("=")) return;
                if (guildnameinfo.Contains("~")) return;
                if (guildnameinfo.Contains("`")) return;
                //Check length lower are 4 return
                if (guildnameinfo.Length < 4) return;
                //Check if guild name is taken or not.
                int guildcheckname = DB.GetRowsCount("SELECT * FROM guild WHERE guild_name='" + guildnameinfo + "'");
                //If name excists
                if (guildcheckname == 1)
                {
                    //Need to sniff packet
                    return;
                }
                //Set the gold requirements 500.000 retail info
                int goldrequired = 500000;

                //If character level is to low
                if (Character.Information.Level < 20)
                {
                    client.Send(Packet.GuildCreateLow());
                    return;
                }

                //If gold is lower then price of creating a guild
                else if (Character.Information.Gold < goldrequired)
                {
                    client.Send(Packet.ErrorMsg(SERVER_GUILD, ErrorMsg.UIIT_MSG_GUILDERR_NOT_ENOUGH_GOLD));
                    return;
                }

                //All checks ok, continue creating new guild.
                else
                {
                    //Reduct the gold required from player gold
                    Character.Information.Gold -= goldrequired;

                    //Save player information
                    SavePlayerInfo();

                    //Insert guild into database
                    DB.query("INSERT INTO guild (guild_name, guild_level, guild_points, guild_news_t, guild_news_m, guild_members_t, guild_master_id) VALUES ('" + guildnameinfo + "','1','0','" + "" + "','" + "" + "','1','" + Character.Information.CharacterID + "')");

                    //Get guild id
                    string guildid = DB.GetData("SELECT id FROM guild WHERE guild_name='" + guildnameinfo + "'", "id");
                    int docount = Convert.ToInt32(guildid);

                    //Insert member into database
                    DB.query("INSERT INTO guild_members (guild_id, guild_member_id, guild_rank, guild_points, guild_fortress, guild_grant, guild_perm_join, guild_perm_withdraw, guild_perm_union, guild_perm_storage, guild_perm_notice) VALUES ('" + docount + "','" + Character.Information.CharacterID + "','0','0','1','','1','1','1','1','1')");

                    //Load our new guild
                    LoadPlayerGuildInfo(false);

                    //Private packet
                    client.Send(Packet.Guild_Create(Character.Network.Guild));

                    //Public spawn packet
                    Send(Packet.SendGuildInfo2(Character));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild invite
        /////////////////////////////////////////////////////////////////////////
        #region Guild invite
        void GuildInvite()
        {
            //Wrap our code into a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Int16 dLen = Reader.Int16();
                string invitename = Reader.String(dLen);
                Reader.Close();
                //Get information for target
                Systems sys = GetPlayerName(invitename);
                //Set targetid information
                Character.Network.TargetID = sys.Character.Information.UniqueID;
                //If player allready has a guild
                if (sys.Character.Network.Guild.Guildid != 0)
                {
                    //Add message here
                    return;
                }
                //Set targetid to the invited player
                sys.Character.Network.TargetID = Character.Information.UniqueID;
                //Send guild request packet
                sys.client.Send(Packet.P_Request(5, Character.Information.UniqueID, 0));
                //Set bools active
                Character.State.GuildInvite = true;
                sys.Character.State.GuildInvite = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild invite error {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Permissions
        /////////////////////////////////////////////////////////////////////////
        void GuildPermissions()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                int memberid = Reader.Int32();

                byte permissions = (byte)Reader.Int32();

                char[] bits = new char[8];

                for (int i = 0; i < 8; ++i) bits[i] = (char)0;
                bits = Convert.ToString(permissions, 2).ToCharArray();

                Reader.Close();

                // set the amount to the target player :)
                int targetindex = Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == memberid);
                if (Character.Network.Guild.MembersInfo[targetindex].Online)
                {
                    Systems member = GetPlayerMainid(memberid);

                    // so here we can set chars right
                    member.Character.Network.Guild.joinRight = bits[4] == '1' ? true : false;
                    member.Character.Network.Guild.withdrawRight = bits[3] == '1' ? true : false;
                    member.Character.Network.Guild.unionRight = bits[2] == '1' ? true : false;
                    member.Character.Network.Guild.guildstorageRight = bits[0] == '1' ? true : false;
                    member.Character.Network.Guild.noticeeditRight = bits[1] == '1' ? true : false;
                }

                // set new amount to every guild members guild class
                foreach (int m in Character.Network.Guild.Members)
                {
                    int index = Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                    if (Character.Network.Guild.MembersInfo[index].Online)
                    {
                        Systems sys = Helpers.GetInformation.GetPlayerMainid(m);

                        // here comes the messy way 
                        Global.guild_player mygp = new Global.guild_player();
                        int myindex = 0;
                        foreach (Global.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                        {
                            if (gp.MemberID == memberid)
                            {
                                mygp = gp; // 
                                mygp.joinRight = bits[4] == '1' ? true : false;
                                mygp.withdrawRight = bits[3] == '1' ? true : false;
                                mygp.unionRight = bits[2] == '1' ? true : false;
                                mygp.guildstorageRight = bits[0] == '1' ? true : false;
                                mygp.noticeeditRight = bits[1] == '1' ? true : false;
                                break;
                            }
                            myindex++;
                        }
                        sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                    }
                }

                DB.query("UPDATE guild_members SET guild_perm_join='" + bits[4] + "',guild_perm_withdraw='" + bits[3] + "',guild_perm_union='" + bits[2] + "',guild_perm_storage='" + bits[0] + "',guild_perm_notice='" + bits[1] + "' WHERE guild_member_id='" + memberid + "'");
                Character.Network.Guild.Send(Packet.GuildUpdate(Character, 4, 0, permissions, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild permission error: {0}", ex);
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////
        //Guild Message
        /////////////////////////////////////////////////////////////////////////
        #region Guild Message
        void GuildMessage()
        {
            //Wrap our function inside a catcher
            try
            {
                //Start reading packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                short TitleL = Reader.Int16();
                string Title = Reader.String(TitleL);
                short MessageL = Reader.Int16();
                string Message = Reader.String(MessageL);
                Reader.Close();

                //Update database guild message title
                DB.query("UPDATE guild SET guild_news_t='" + Title + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");

                //Update database guild message
                DB.query("UPDATE guild SET guild_news_m='" + Message + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                //Set new message info to current member for sending packet update.
                Character.Network.Guild.NewsTitle = Title;
                Character.Network.Guild.NewsMessage = Message;
                //Send news update to all members currently online
                Character.Network.Guild.Send(Packet.GuildUpdate(Character, 11, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Message Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Transfer leadership
        /////////////////////////////////////////////////////////////////////////
        #region Transfer leadership
        void GuildTransferLeaderShip()
        {
            try
            {
                //Read packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int Guildid = Reader.Int32();
                int Memberid = Reader.Int32();
                Reader.Close();

                //Grab the information of the player
                Systems sys = GetPlayerid(Memberid);

                //Update database
                DB.query("UPDATE guild_members SET guild_rank='10',guild_perm_join='0',guild_perm_withdraw='0',guild_perm_union='0',guild_perm_storage='0',guild_perm_notice='0' WHERE guild_member_id='" + Character.Information.CharacterID + "'");
                DB.query("UPDATE guild_members SET guild_rank='0',guild_perm_join='1',guild_perm_withdraw='1',guild_perm_union='1',guild_perm_storage='1',guild_perm_notice='1' WHERE guild_member_id='" + Memberid + "'");

                //Send required packets to network
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member s not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Bool check single send packet
                            if (!guildmember.Character.Network.Guild.SingleSend)
                            {
                                //Send update packet
                                guildmember.client.Send(Packet.GuildUpdate(Character, 3, Memberid, 0 , 0));
                                guildmember.LoadPlayerGuildInfo(false);
                            }
                        }
                    }
                }
                //Disable bool
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems guildmember = GetPlayerMainid(member);
                        if (guildmember != null)
                        {
                            guildmember.Character.Network.Guild.SingleSend = false;
                        }
                    }
                }
                //Send message to old owner
                client.Send(Packet.GuildTransferMessage());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Transfer Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Title
        /////////////////////////////////////////////////////////////////////////
        #region Guild Title
        void GuildTitle()
        {
            //Wrap our function inside a catcher
            try
            {
                //Extra hack check
                if (Character.Network.Guild.Level < 4) return;
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int Selecteduser = Reader.Int32();
                short TitleL = Reader.Int16();
                string Title = Reader.String(TitleL);
                Reader.Close();

                //Get character information
                Systems playerinfo = GetPlayerMainid(Selecteduser);
                //Make sure the user is still there
                if (playerinfo.Character != null)
                {
                    //Update database set new title
                    DB.query("UPDATE guild_members SET guild_grant='" + Title + "' WHERE guild_member_id='" + playerinfo.Character.Information.CharacterID + "'");

                    //Send title update to send list
                    Send(Packet.GuildSetTitle(Character.Guild.GuildID, playerinfo.Character.Information.Name, Title));

                    //Send Final packet to client
                    client.Send(Packet.GuildSetTitle2(Character.Guild.GuildID, Selecteduser, Title));
                }
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems getplayer = GetPlayerMainid(member);
                        if (getplayer != null)
                        {
                            getplayer.LoadPlayerGuildInfo(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Title Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Disbanding
        /////////////////////////////////////////////////////////////////////////
        #region Guild Disband
        void GuildDisband()
        {
            try
            {
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems guildplayer = GetPlayerMainid(member);
                        if (guildplayer != null)
                        {
                            if (guildplayer.Character.Information.CharacterID != Character.Information.CharacterID)
                            {
                                //Guild disband message packet
                                guildplayer.client.Send(Packet.GuildUpdate(Character, 2, 0, 0, 0));
                                //Remove guild name and details from player
                                Send(Packet.GuildKick(guildplayer.Character.Information.UniqueID));
                                //State packet
                                guildplayer.client.Send(Packet.StatePack(guildplayer.Character.Information.UniqueID, 4, 0, false));
                                //Set all values to null.
                                guildplayer.Character.Network.Guild.Members.Remove(guildplayer.Character.Information.CharacterID);
                                guildplayer.Character.Network.Guild.MembersClient.Remove(guildplayer.client);
                                guildplayer.Character.Network.Guild.Guildid = 0;

                                if (guildplayer.Character.Network.Guild.UniqueUnion != 0)
                                {
                                    guildplayer.Character.Network.Guild.UnionActive = false;
                                    guildplayer.Character.Network.Guild.UnionMembers.Remove(guildplayer.Character.Information.CharacterID);
                                }
                            }
                        }
                    }
                }
                //Guild disband message packet
                client.Send(Packet.GuildUpdate(Character, 2, 0, 0, 0));
                //Remove guild name and details from player
                Send(Packet.GuildKick(Character.Information.UniqueID));
                //State packet
                client.Send(Packet.StatePack(Character.Information.UniqueID, 4, 0, false));
                //Set all values to null.

                //Remove all rows that contains guildname
                DB.query("DELETE FROM guild_members WHERE guild_id=" + Character.Network.Guild.Guildid + "");
                //Remove guild from guild table
                DB.query("DELETE FROM guild WHERE id=" + Character.Network.Guild.Guildid + "");
                //Remove ourself
                if (Character.Network.Guild.UniqueUnion != 0)
                {
                    Character.Network.Guild.UnionActive = false;
                    Character.Network.Guild.UnionMembers.Remove(Character.Information.CharacterID);
                }

                Character.Network.Guild.Members.Remove(Character.Information.UniqueID);
                Character.Network.Guild.MembersClient.Remove(client);
                Character.Network.Guild.Guildid = 0;

                //Packet Final message
                client.Send(Packet.DisbandGuildMsgEnd());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Disband Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Promote
        /////////////////////////////////////////////////////////////////////////
        #region Guild Promote
        void GuildPromote()
        {
            try
            {
                //Read client information int32 id
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int guildid = reader.Int32();
                reader.Close();

                //Incase of hacking attempt :)
                if (Character.Network.Guild.Level == 5) return;
                //Define int's to use
                int goldneeded;
                int guildpoints;
                bool promoting = false;
                if (!promoting)
                {
                    LoadPlayerGuildInfo(false);
                    promoting = true;
                    switch (Character.Network.Guild.Level)
                    {
                        case 1:
                            goldneeded = 3000000;
                            guildpoints = 5400;
                            break;
                        case 2:
                            goldneeded = 9000000;
                            guildpoints = 50400;
                            break;
                        case 3:
                            goldneeded = 15000000;
                            guildpoints = 135000;
                            break;
                        case 4:
                            goldneeded = 21000000;
                            guildpoints = 378000;
                            break;
                        default:
                            return;
                    }
                    int changelevel = Character.Network.Guild.Level + 1;
                    int storageslots = Character.Network.Guild.StorageSlots + 30;

                    if (Character.Information.Gold < goldneeded)
                    {
                        client.Send(Packet.ErrorMsg(SERVER_GUILD_PROMOTE_MSG, ErrorMsg.UIIT_MSG_ERROR_GUILD_LEVEL_UP_GOLD_DEFICIT));
                        return;
                    }
                    //Not enough guildpoints
                    if (Character.Network.Guild.PointsTotal < guildpoints)
                    {
                        client.Send(Packet.ErrorMsg(SERVER_GUILD_PROMOTE_MSG, ErrorMsg.UIIT_MSG_ERROR_GUILD_LEVEL_UP_GP_DEFICIT));
                    }
                    //Max level
                    if (Character.Network.Guild.Level == 5)
                    {
                        client.Send(Packet.ErrorMsg(SERVER_GUILD_PROMOTE_MSG, ErrorMsg.UIIT_MSG_ERROR_GUILD_LEVEL_UP_FULL));
                        return;
                    }
                    else
                    //Upgrade guild initiate
                    {
                        //If max level return just incase.
                        if (Character.Network.Guild.Level == 5) return;
                        //Reduct guildpoints
                        Character.Network.Guild.PointsTotal -= guildpoints;
                        //If the upgrade is final upgrade set points to 0
                        if (Character.Network.Guild.Level == 4) Character.Network.Guild.PointsTotal = 0;
                        //Reduct gold
                        Character.Information.Gold -= goldneeded;

                        client.Send(Packet.InfoUpdate(1, Character.Network.Guild.PointsTotal, 0));
                        client.Send(Packet.GuildPromoteMsgS());

                        //Update guild in database
                        DB.query("UPDATE guild SET guild_level='" + changelevel + "',guild_points='" + Character.Network.Guild.PointsTotal + "',guild_storage_slots='" + storageslots + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");

                        //Send Guild Packets
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            if (member != 0)
                            {
                                Systems memberinfo = GetPlayerMainid(member);
                                if (memberinfo != null)
                                {
                                    memberinfo.LoadPlayerGuildInfo(false);
                                    memberinfo.client.Send(Packet.GuildUpdate(Character, 5, 0, 0, 0));
                                }
                            }
                        }
                        // save
                        SaveGold();
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                    }
                }
                promoting = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Promote Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Kick from guild
        /////////////////////////////////////////////////////////////////////////
        #region Guild remove
        void GuildRemove()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                short UserL = Reader.Int16();
                string User = Reader.String(UserL);
                Reader.Close();

                //Define new count for database check

                //Get player information
                Systems sys = GetPlayerName(User);

                //Send required packets to network
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member s not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Make sure the kicked member does not receive the packet
                            if (guildmember.Character.Information.CharacterID != sys.Character.Information.CharacterID)
                            {
                                //Bool check single send packet
                                if (!guildmember.Character.Network.Guild.SingleSend)
                                {
                                    //Send update packet
                                    guildmember.client.Send(Packet.GuildUpdate(sys.Character, 7, 0, 0, 0));
                                    guildmember.Character.Network.Guild.SingleSend = true;
                                    
                                }
                            }
                        }
                    }
                }
                //Send update packet to the kicked player
                sys.client.Send(Packet.GuildUpdate(sys.Character, 7, 0, 0, 0));
                sys.client.Send(Packet.GuildKickMsg());
                sys.Send(Packet.GuildKick(sys.Character.Information.UniqueID));

                //Minus 1 count for new database info
                int getmembercount = Convert.ToInt32(DB.GetData("SELECT guild_members_t FROM guild WHERE guild_name='" + Character.Network.Guild.Name + "'", "guild_members_t"));
                int newmembercount = getmembercount - 1;

                //Update database
                DB.query("UPDATE guild SET guild_members_t='" + newmembercount + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");

                //Remove the player from database
                DB.query("DELETE from guild_members where guild_member_id='" + sys.Character.Information.CharacterID + "'");

                //Disable bool
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems guildmember = GetPlayerMainid(member);
                        if (guildmember != null)
                        {
                            guildmember.LoadPlayerGuildInfo(false);
                            guildmember.Character.Network.Guild.SingleSend = false;
                        }
                    }
                }

                //Final updates
                if (Character.Network.Guild.UniqueUnion != 0)
                {
                    Character.Network.Guild.UnionActive = false;
                    Character.Network.Guild.UnionMembers.Remove(Character.Information.CharacterID);
                }

                Character.Network.Guild.Members.Remove(sys.Character.Information.UniqueID);
                Character.Network.Guild.MembersClient.Remove(sys.client);

                sys.Character.Network.Guild.Guildid = 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Kick Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Leave guild
        /////////////////////////////////////////////////////////////////////////
        #region Leave Guild
        void GuildLeave()
        {
            //Write our function inside a catcher
            try
            {
                //Send required packets to network
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member s not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Bool check single send packet
                            if (!guildmember.Character.Network.Guild.SingleSend)
                            {
                                //Send update packet
                                guildmember.client.Send(Packet.GuildUpdate(Character, 12, 0, 0, 0));
                                guildmember.Character.Network.Guild.SingleSend = true;
                            }
                        }
                    }
                }
                //Disable bool
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems guildmember = GetPlayerMainid(member);
                        if (guildmember != null)
                        {
                            guildmember.Character.Network.Guild.SingleSend = false;
                        }
                    }
                }
                //Send remaining packets
                Send(Packet.GuildKick(Character.Information.UniqueID));
                client.Send(Packet.GuildLeave());
                client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x00, false));
                //Minus 1 count for new database info
                int getmembercount = Convert.ToInt32(DB.GetData("SELECT guild_members_t FROM guild WHERE guild_name='" + Character.Network.Guild.Name + "'", "guild_members_t"));
                int newmembercount = getmembercount - 1;

                //Update database
                DB.query("UPDATE guild SET guild_members_t='" + newmembercount + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");

                //Remove the player from database
                DB.query("DELETE from guild_members where guild_member_id='" + Character.Information.CharacterID + "'");

                //Final updates
                if (Character.Network.Guild.UniqueUnion != 0)
                {
                    Character.Network.Guild.UnionActive = false;
                    Character.Network.Guild.UnionMembers.Remove(Character.Information.CharacterID);
                }
                Character.Network.Guild.Members.Remove(Character.Information.UniqueID);
                Character.Network.Guild.MembersClient.Remove(client);
                Character.Network.Guild.Guildid = 0;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Storage
        /////////////////////////////////////////////////////////////////////////
        #region Guild Storage
        void GuildStorage()
        {
            //Wrap our function inside a catcher
            try
            {
                //If guild level is to low send message
                if (Character.Network.Guild.Level == 1)
                {
                    byte type = 2;
                    client.Send(Packet.GuildStorageStart(type));
                }
                //If guild level is 2 meaning it has storage option
                else
                {
                    //Make sure the user has guild storage rights
                    if (Character.Network.Guild.guildstorageRight)
                    {
                        //Check if other guild members are currently in storage
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure member isnt 0
                            if (member != 0)
                            {
                                //Get player details
                                Systems getplayer = GetPlayerMainid(member);
                                //Make sure player isnt null
                                if (getplayer != null)
                                {
                                    //Check if the player is using storage
                                    if (getplayer.Character.Network.Guild.UsingStorage)
                                    {
                                        //Add message here other user is using storage.
                                        return;
                                    }
                                }
                            }
                        }
                        //Make sure that the user isnt using storage allready
                        if (!Character.Network.Guild.UsingStorage)
                        {
                            byte type = 1;
                            //Set user as active storage user
                            Character.Network.Guild.UsingStorage = true;
                            //Send storage begin packet
                            client.Send(Packet.GuildStorageStart(type));
                        }
                    }
                    else
                    {
                        //Send error message to user not allowed
                        client.Send(Packet.ErrorMsg(SERVER_GUILD_STORAGE, ErrorMsg.UIIT_MSG_STRGERR_STORAGE_OPERATION_BLOCKED));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GuildStorage Open Error: {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
        #region Storage Data
        void GuildStorage2()
        {
            //Make sure the player is still using storage.
            try
            {
                if (Character.Network.Guild.UsingStorage)
                {
                    LoadPlayerGuildInfo(false);
                    //Send storage data load information
                    client.Send(Packet.GuildStorageGold(Character));
                    client.Send(Packet.GuildStorageData(Character));
                    client.Send(Packet.GuildStorageDataEnd());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Close storage
        void GuildStorageClose()
        {
            try
            {
                //Set bool to false
                Character.Network.Guild.UsingStorage = false;
                //Send close packet for storage window
                client.Send(Packet.GuildStorageClose());
                //Send close packet for npc id window
                Close_NPC();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////
        //Guild Dontate GP
        /////////////////////////////////////////////////////////////////////////
        #region Donated gp
        void DonateGP()
        {
            //First we write our function inside a catcher
            try
            {
                //Max level of guild wont allow new gp donations.
                if (Character.Network.Guild.Level == 5)
                {
                    client.Send(Packet.ErrorMsg(SERVER_GUILD_PROMOTE_MSG, ErrorMsg.UIIT_MSG_GUILD_LACK_GP));
                    return;
                }
                //Open our packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int donatedgp = reader.Int32();
                reader.Close();
                //Anti hack checking (If donated gp higher is then the player skillpoints.
                if (donatedgp > Character.Information.SkillPoint) return;
                //Calculate total
                int totalgp = Character.Network.Guild.PointsTotal + donatedgp;
                //First we write our base information to use
                Character.Network.Guild.PointsTotal += donatedgp;
                Character.Information.SkillPoint -= donatedgp;
                //Save our information (Skill points).
                SavePlayerInfo();
                //Update database information
                DB.query("UPDATE guild SET guild_points='" + totalgp + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                //Update packets for each member in the guild
                //Send packets to donator.
                client.Send(Packet.InfoUpdate(1, totalgp, 0));
                client.Send(Packet.GuildDonateGP(donatedgp));

                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure that the member isnt null
                    if (member != 0)
                    {
                        //Now we get the detailed information for each member
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember is still there
                        if (guildmember != null)
                        {
                            //Send packet only once
                            if (!guildmember.Character.Network.Guild.SingleSend)
                            {
                                //Set bool true so it only sends once
                                guildmember.Character.Network.Guild.SingleSend = true;
                                //Send packet data to the player
                                guildmember.client.Send(Packet.GuildUpdate(Character, 13, 0, 0, totalgp));
                                guildmember.client.Send(Packet.GuildUpdate(Character, 9, 0, 0, totalgp));
                                guildmember.LoadPlayerGuildInfo(false);
                            }
                        }
                    }
                }
                //Disable the bool again
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member isnt null
                    if (member != 0)
                    {
                        //Get guildmember details
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure guildmember isnt null
                        if (guildmember != null)
                        {
                            //Disable bool to allow resend new packets.
                            guildmember.Character.Network.Guild.SingleSend = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Donate GP Error {0}", ex);
                Log.Exception(ex);
            }
        }
        #endregion
    }
}