using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void GuildStorage()
        {
            //Wrap our function inside a catcher
            try
            {
                //If guild level is to low send message
                if (Character.Network.Guild.Level == 1)
                {
                    //Need to sniff to check what opcode is sending for the message
                    client.Send(Packet.Message(OperationCode.SERVER_GUILD_STORAGE, Messages.UIIT_STT_GUILD_STORAGE_LEVEL_TO_LOW));
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
                                PlayerMgr getplayer = Helpers.GetInformation.GetPlayerMainid(member);
                                //Make sure player isnt null
                                if (getplayer != null)
                                {
                                    //Check if the player is using storage
                                    if (getplayer.Character.Network.Guild.UsingStorage)
                                    {
                                        //Send storage message error
                                        client.Send(Packet.Message(OperationCode.SERVER_GUILD_WAIT, Messages.UIIT_MSG_STRGERR_STORAGE_OPERATION_BLOCKED));
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
                    //If the player has no storage rights
                    else
                    {
                        //Send error message to user not allowed
                        client.Send(Packet.Message(OperationCode.SERVER_GUILD_STORAGE, Messages.UIIT_MSG_STRGERR_STORAGE_OPERATION_BLOCKED));
                    }
                }
            }
            //Catch any bad errors
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void GuildStorage2()
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
            //Catch any bad errors
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void GuildStorageClose()
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
            //Catch any bad errors
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
