using CLGameServer.Client;
using CLFramework;
using System;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void NormalRequest()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Get invited member
                int Target = Reader.Int32();
                //Get party type
                byte PartyType = Reader.Byte();
                //Close reader
                Reader.Close();

                //Get target player information
                PlayerMgr InvitedPlayer = Helpers.GetInformation.GetPlayer(Target);
                //First we check the our own player level
                if (Character.Information.Level < 5)
                {
                    //Send message

                    //Return
                    return;
                }
                //Check target level
                if (InvitedPlayer.Character.Information.Level < 5)
                {
                    //Send message

                    //Return
                    return;
                }
                //Set target information for invited player
                InvitedPlayer.Character.Network.TargetID = Character.Information.UniqueID;
                //If the player inviting, has no party yet.
                if (Character.Network.Party == null)
                {
                    //Create new party
                    WorldMgr.party Party = new WorldMgr.party();
                    //Set leader of party
                    Party.LeaderID = Character.Information.UniqueID;
                    //Set party type
                    Party.Type = PartyType;
                    //Add to party net info
                    Character.Network.Party = Party;
                }
                //If the target player has no party yet.
                if (InvitedPlayer.Character.Network.Party == null)
                {
                    //Send invitation packet
                    InvitedPlayer.client.Send(Packet.PartyRequest(2, this.Character.Information.UniqueID, PartyType));
                    //Set invite bools
                    InvitedPlayer.Character.Information.CheckParty = true;
                    Character.Information.CheckParty = true;
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine(ex);
                //Write information to the debug log

            }
        }
    }
}
