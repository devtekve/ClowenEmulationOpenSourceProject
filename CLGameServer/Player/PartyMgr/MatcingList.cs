using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void ListPartyMatching(List<WorldMgr.party> pt)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(OperationCode.SERVER_SEND_PARTYLIST);
            //Write static bytes
            Writer.Byte(1);
            Writer.Byte(4);
            Writer.Byte(0);
            //Write total count of partys
            Writer.Byte(pt.Count);
            //If party count higher is then zero
            if (pt.Count > 0)
            {
                //Repeat for each party in list of party's
                foreach (WorldMgr.party currpt in pt)
                {
                    //Get player information using leaderid
                    PlayerMgr s = Helpers.GetInformation.GetPlayer(currpt.LeaderID);
                    //Write party id
                    Writer.DWord(currpt.ptid);
                    //Write leader id
                    Writer.DWord(currpt.LeaderID);
                    //Write charactername
                    Writer.Text(s.Character.Information.Name);
                    //Write static byte 1
                    Writer.Byte(currpt.Race);
                    //Write current party players count
                    Writer.Byte(currpt.Members.Count);
                    //Write party type
                    Writer.Byte(currpt.Type);
                    //Write party purpose
                    Writer.Byte(currpt.ptpurpose);
                    //Write min level required
                    Writer.Byte(currpt.minlevel);
                    //Write max level to join the party
                    Writer.Byte(currpt.maxlevel);
                    //Write party name
                    Writer.Text3(currpt.partyname);
                }
            }
            //Send bytes to the client
            client.Send(Writer.GetBytes());
        }
    }
}
