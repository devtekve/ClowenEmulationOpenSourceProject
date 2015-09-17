using System;
using System.Linq;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void CreateFormedParty()
        {
            try
            {
                //Get packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //First and second dword are static
                int masteryid1 = Reader.Int32(); // MasteryID
                int masteryid2 = Reader.Int32(); // MasteryID2
                //First byte is our party type
                byte PartyType = Reader.Byte();
                //Second byte is purpose information
                byte PartyPurpose = Reader.Byte();
                //3rd byte is minimal level required to join
                byte PartyMinLevelReq = Reader.Byte();
                //4th byte is max level to join
                byte PartyMaxLevelReq = Reader.Byte();
                //5th is short, party name lenght
                //6th is party name
                string PartyName = Reader.Text3();
                //Close our reader
                Reader.Close();

                //Make sure the user isnt in a party yet.
                if (Character.Network.Party != null)
                {
                    //If party is formed allready return
                    if (Character.Network.Party.IsFormed)
                    {
                        return;
                    }
                    //If party is not formed
                    else
                    {
                        //Get current party information
                        WorldMgr.party CurrentParty = Character.Network.Party;
                        //Set formed state
                        CurrentParty.IsFormed = true;
                        //Party type
                        CurrentParty.Type = PartyType;
                        //Party purpose
                        CurrentParty.ptpurpose = PartyPurpose;
                        //Party minimal level
                        CurrentParty.minlevel = PartyMinLevelReq;
                        //Party maximum level
                        CurrentParty.maxlevel = PartyMaxLevelReq;
                        //Party name
                        CurrentParty.partyname = PartyName;
                        //Party owner
                        CurrentParty.LeaderID = Character.Information.UniqueID;
                        //Add party eu / ch information by model
                        CurrentParty.Race = Character.Information.Race;
                        //Send packet information to user
                        client.Send(Packet.CreateFormedParty(CurrentParty));
                        //Add party to list
                        Helpers.Manager.Party.Add(CurrentParty);
                    }
                }
                //If a new formed party is created from 0
                else
                {
                    //New party for details
                    WorldMgr.party newparty = new WorldMgr.party();
                    //Set formed state
                    newparty.IsFormed = true;
                    //Party type
                    newparty.Type = PartyType;
                    //Party purpose
                    newparty.ptpurpose = PartyPurpose;
                    //Party minimal level
                    newparty.minlevel = PartyMinLevelReq;
                    //Party maximum level
                    newparty.maxlevel = PartyMaxLevelReq;
                    //Party name
                    newparty.partyname = PartyName;
                    //Party owner
                    newparty.LeaderID = Character.Information.UniqueID;
                    //Add party eu / ch information by model
                    newparty.Race = Character.Information.Race;
                    //Add our player to the member list
                    newparty.Members.Add(Character.Information.UniqueID);
                    //Add player client to party list information
                    newparty.MembersClient.Add(client);
                    //Party id , is current count of party's + 1
                    newparty.ptid = Helpers.Manager.Party.Count + 1;
                    //Add the new party list
                    Helpers.Manager.Party.Add(newparty);
                    //Set party to player
                    Character.Network.Party = newparty;
                    //bool set player in party
                    Character.Network.Party.InParty = true;
                    //Send packet information to user
                    client.Send(Packet.CreateFormedParty(newparty));
                }

            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
