using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////
        // Union Apply
        /////////////////////////////////////////////////////////////////////////
        public void unionapply()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Get target id (Targeted).
                int Target = Reader.Int32();
                //Close reader
                Reader.Close();

                //Get target details
                PlayerMgr targetplayer = Helpers.GetInformation.GetPlayer(Target);
                //Make sure the target is still there
                if (targetplayer != null)
                {
                    //If allready in union
                    if (targetplayer.Character.Network.Guild.UnionActive) return;
                    //Set bools for both players
                    targetplayer.Character.State.UnionApply = true;
                    Character.State.UnionApply = true;
                    //Set target player to us
                    targetplayer.Character.Network.TargetID = Character.Information.UniqueID;
                    //Send request to targeted player
                    targetplayer.client.Send(Packet.PartyRequest(6, Character.Information.UniqueID, 0));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}