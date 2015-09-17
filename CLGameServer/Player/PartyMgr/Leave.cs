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
        public void LeaveParty()
        {
            try
            {
                //Make sure the party isnt null to prevent errors.
                if (Character.Network.Party != null)
                {
                    //Remove the character member info
                    Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                    //Remove client
                    Character.Network.Party.MembersClient.Remove(client);
                    //If the owner removes the party set new leader
                    if (Character.Information.UniqueID == Character.Network.Party.LeaderID)
                    {
                        //Repeat for each member in the party
                        foreach (int partymembers in Character.Network.Party.Members)
                        {
                            PlayerMgr partymember = Helpers.GetInformation.GetPlayer(partymembers);
                            //Send party update data to player
                            client.Send(Packet.Party_Data(1, 0));
                            //If the count is 1, we remove the party information
                            if (partymember != null)
                            {
                                if (Character.Network.Party.Members.Count == 1)
                                {
                                    //If its a formed party remove the entry. (check).
                                    if (Character.Network.Party.IsFormed)
                                        DeleteFormedParty(Character.Network.Party.ptid);
                                    //Remove party member from member list
                                    partymember.Character.Network.Party.Members.Remove(partymember.Character.Information.UniqueID);
                                    //Remove the party member client from the list
                                    partymember.Character.Network.Party.MembersClient.Remove(partymember.client);
                                    //Set party to null
                                    partymember.Character.Network.Party = null;
                                    //Set bool to false so the player can join another party
                                    partymember.Character.Information.CheckParty = false;
                                    //Send packet to member
                                    partymember.client.Send(Packet.Party_Data(1, 0));
                                }
                                //If more party members are in the party we dont remove the party.
                                else
                                {
                                    //Get first available member for new leader
                                    partymember.Character.Network.Party.LeaderID = Character.Network.Party.Members[0];
                                    //Send update packet to member
                                    partymember.client.Send(Packet.Party_Data(9, Character.Network.Party.Members[0]));
                                    //Send removal of the user
                                    partymember.client.Send(Packet.Party_Data(3, Character.Information.UniqueID));
                                }
                            }
                        }
                        //Set player information
                        Character.Network.Party = null;
                        Character.Information.CheckParty = false;
                    }

                    else
                    {
                        //Send party update data to player
                        client.Send(Packet.Party_Data(1, 0));
                        //For each member in the party
                        foreach (int partymember in Character.Network.Party.Members)
                        {
                            //Get player information
                            PlayerMgr partym = Helpers.GetInformation.GetPlayer(partymember);
                            //If auto disband party
                            if (Character.Network.Party.Members.Count == 1)
                            {
                                //If its a formed party remove the entry. (check).
                                if (partym.Character.Network.Party.IsFormed) partym.DeleteFormedParty(Character.Network.Party.ptid);
                                //Remove the owner member
                                partym.Character.Network.Party.Members.Remove(this.Character.Information.UniqueID);
                                //Remove the client
                                partym.Character.Network.Party.MembersClient.Remove(this.client);
                                //Set party to null
                                partym.Character.Network.Party = null;
                                //Bool to false so can be invited again
                                partym.Character.Information.CheckParty = false;
                                //Visual update packet
                                partym.client.Send(Packet.Party_Data(1, 0));
                            }
                            //If the player has enough players (Not auto disband).
                            else
                            {
                                //Remove information for all party members
                                partym.Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                                //Remove the client
                                partym.Character.Network.Party.MembersClient.Remove(client);
                                //Remove the id
                                partym.client.Send(Packet.Party_Data(3, Character.Information.UniqueID));
                                //Set null party info
                                partym.Character.Network.Party = null;
                                //Set bool
                                partym.Character.Information.CheckParty = false;
                            }
                        }
                        //Set party network to null
                        Character.Network.Party = null;
                        //Set bool to false so player can go in new party.
                        Character.Information.CheckParty = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Leave party error {0}", ex);

            }
        }
    }
}
