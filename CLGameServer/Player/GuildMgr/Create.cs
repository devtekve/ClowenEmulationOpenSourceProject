using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void GuildCreate()
        {
            try
            {
                //Extra check if user is allready in guild.
                if (Character.Network.Guild.Guildid != 0) return;
                //If player has recently been in a guild
                if (Character.Information.GuildPenalty)
                {
                    //Need to sniff the retail packet (Tmp used guild war error).
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD_WAIT, Messages.UIIT_MSG_GUILDWARERR_GUILD_CREATE_PENALTY));
                    return;
                }
                //Create new packet reader for reading information
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //None needed integer
                int notneeded = Reader.Int32();
                //Get guild name lenght
                short GuildNameLen = Reader.Int16();
                //Get guild name
                string GuildName = Reader.String(GuildNameLen);
                //Close packet reader
                Reader.Close();
                //Check length lower are 4 return
                if (GuildName.Length < 4)
                {
                    //Send incorrect lenght
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD_WAIT, Messages.UIIT_MSG_GUILDERR_INVALID_GUILDNAME_LEN));
                    return;
                }
                //Check if guild name is taken or not.
                int guildcheckname = DB.GetRowsCount("SELECT * FROM guild WHERE guild_name='" + GuildName + "'");
                //If name excists
                if (guildcheckname == 1)
                {
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD_WAIT, Messages.UIIT_MSG_GUILDERR_SAME_GUILDNAME_EXIST));
                    return;
                }
                //If character level is to low
                if (Character.Information.Level < 20)
                {
                    //Send packet level to low message
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD, Messages.UIIT_MSG_GUILDERR_TOO_LOW_CREATOR_LEVEL));
                    return;
                }
                //Set the gold requirements 500.000 retail info
                int goldrequired = 500000;
                //If gold is lower then price of creating a guild
                if (Character.Information.Gold < goldrequired)
                {
                    //Send message not enough gold
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD, Messages.UIIT_MSG_GUILDERR_NOT_ENOUGH_GOLD));
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
                    DB.query("INSERT INTO guild (guild_name, guild_level, guild_points, guild_news_t, guild_news_m, guild_members_t, guild_master_id) VALUES ('" + GuildName + "','1','0','" + "" + "','" + "" + "','1','" + Character.Information.CharacterID + "')");
                    //Get guild id
                    string guildid = DB.GetData("SELECT id FROM guild WHERE guild_name='" + GuildName + "'", "id");
                    int docount = Convert.ToInt32(guildid);
                    //Insert member into database
                    DB.query("INSERT INTO guild_members (guild_id, guild_member_id, guild_rank, guild_points, guild_fortress, guild_grant, guild_perm_join, guild_perm_withdraw, guild_perm_union, guild_perm_storage, guild_perm_notice) VALUES ('" + docount + "','" + Character.Information.CharacterID + "','0','0','1','','1','1','1','1','1')");
                    //Load our new guild
                    LoadPlayerGuildInfo(true);
                    //Private packet
                    client.Send(Packet.Guild_Create(Character.Network.Guild));
                    //Public spawn packet
                    Send(Packet.SendGuildInfo2(this.Character));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild creation error: " + ex);

            }
        }
    }
}
