using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void PartyBan()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int TargetID = Reader.Int32();
                Reader.Close();
                //Get targeted player information
                PlayerMgr s = Helpers.GetInformation.GetPlayers(TargetID);
                //Remove id of the member
                Character.Network.Party.Members.Remove(s.Character.Information.UniqueID);
                //Remove the client of the member
                Character.Network.Party.MembersClient.Remove(s.client);
                //Repeat for each member the updated party information
                foreach (int partymember in Character.Network.Party.Members)
                {
                    //Get player information for the next member
                    PlayerMgr partym = Helpers.GetInformation.GetPlayer(partymember);
                    //Remove the kicked player
                    partym.Character.Network.Party.Members.Remove(s.Character.Information.UniqueID);
                    partym.Character.Network.Party.MembersClient.Remove(s.client);

                    //If we have one member remaining in the party we disband the party
                    if (partym.Character.Network.Party.Members.Count == 1)
                    {
                        //If its formed in the list remove the listening
                        if (partym.Character.Network.Party.IsFormed)
                            partym.DeleteFormedParty(Character.Network.Party.ptid);
                        //Send update packet to the party member                       
                        partym.client.Send(Packet.Party_Data(1, 0));
                        //Send update packet to the current player
                        client.Send(Packet.Party_Data(1, 0));
                        //Set party to null for the current player
                        Character.Network.Party = null;
                        //Set party to null for the remaining member
                        partym.Character.Network.Party = null;
                        //Set bool for current player
                        Character.Information.CheckParty = false;
                        //Set bool for the remaining party member
                        partym.Character.Information.CheckParty = false;
                    }
                    //If there are more members (Not autodisband party).
                    else
                    {
                        //Send the update packet to the party member
                        partym.client.Send(Packet.Party_Data(3, TargetID));
                    }
                }
                //Set the kicked player bool to false
                s.Character.Information.CheckParty = false;
                //Remove the party network for the kicked player
                s.Character.Network.Party = null;
                //Send update packet to the kicked player
                s.client.Send(Packet.Party_Data(1, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Party Ban Error {0}", ex);

            }
        }
    }
}
