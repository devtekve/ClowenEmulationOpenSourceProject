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
        public void DeleteFormedParty(int partynetid)
        {
            try
            {
                //If the party is beeing deleted manually from listening
                if (partynetid == 0)
                {
                    //Read our packet data
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    //Read party id integer
                    int partyid = Reader.Int32();
                    //Close packet reader
                    Reader.Close();
                    //Find the related party id
                    Helpers.Manager.Party.Remove(Helpers.Manager.Party.Find(delegate (WorldMgr.party pt)
                    {
                        //If found return the information
                        return pt.IsFormed && (pt.ptid == partyid);
                    }));
                    //Send removal packet for listening
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(OperationCode.SERVER_DELETE_FORMED_PARTY);
                    Writer.Byte(1);
                    Writer.DWord(partyid);
                    client.Send(Writer.GetBytes());
                    //Set party state
                    Character.Network.Party.IsFormed = false;
                }
                //If listening is deleted due to auto disband
                else
                {
                    //Find the related party given from partynetid
                    Helpers.Manager.Party.Remove(Helpers.Manager.Party.Find(delegate (WorldMgr.party pt)
                    {
                        //Return information
                        return pt.IsFormed && (pt.ptid == partynetid);
                    }));
                    //Remove from listening
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(OperationCode.SERVER_DELETE_FORMED_PARTY);
                    Writer.Byte(1);
                    Writer.DWord(partynetid);
                    client.Send(Writer.GetBytes());
                    //Set party state
                    Character.Network.Party.IsFormed = false;
                }
                //If theres only one member leave and remove party data
                if (Character.Network.Party.Members.Count == 1)
                {
                    Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                    Character.Network.Party.MembersClient.Remove(client);

                    Character.Network.Party = null;
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
