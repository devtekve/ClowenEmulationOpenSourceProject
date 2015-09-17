using CLGameServer.Client;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public static void RemoveOnDisconnect(WorldMgr.party p, PlayerMgr c)
        {
            //Remove client and member if it contains our removing character
            if (p.Members.Contains(c.Character.Information.UniqueID))
            {
                p.Members.Remove(c.Character.Information.UniqueID);
                p.MembersClient.Remove(c.client);
            }
            //Send packet to each player
            foreach (int member in p.Members)
            {
                PlayerMgr playerdetail = Helpers.GetInformation.GetPlayer(member);

                if (p.Members.Count > 1)
                {
                    playerdetail.client.Send(Packet.Party_Data(1, 0));
                }
                else
                {
                    //Send removal of party
                    playerdetail.client.Send(Packet.Party_Data(3, playerdetail.Character.Information.UniqueID));
                    //Remove party state
                    playerdetail.Character.Network.Party = null;
                }
            }
        }
    }
}
