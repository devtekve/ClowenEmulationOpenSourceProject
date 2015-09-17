using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void PartyAddmembers()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read target id
                int targetid = Reader.Int32();
                //Close packet reader
                Reader.Close();
                //Get detailed information from target
                PlayerMgr InvitedPlayer = Helpers.GetInformation.GetPlayer(targetid);
                //Check if the targeted player allready is in a party.
                if (InvitedPlayer.Character.Network.Party == null)
                {
                    //Set target id of target player to our id
                    InvitedPlayer.Character.Network.TargetID = this.Character.Information.UniqueID;
                    //Send request
                    InvitedPlayer.client.Send(Packet.PartyRequest(2, Character.Information.UniqueID, Character.Network.Party.Type));
                }
            }
            //Write bad exception errors
            catch (Exception ex)
            {
                //Write error to the console.
                Console.WriteLine(ex);
                //Write error to the debug log

            }
        }
    }
}
