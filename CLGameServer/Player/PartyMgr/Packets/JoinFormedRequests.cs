using System;
using CLFramework;

namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] PartyRequest(byte Type, int id, byte type)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(OperationCode.SERVER_PARTY_REQUEST);
            //Write type byte
            Writer.Byte(Type);
            //Create switch on type
            switch (Type)
            {
                case 6:
                    //Union invite
                    Writer.DWord(id);
                    break;
                case 5:
                    //Guild invitation
                    Writer.DWord(id);
                    PlayerMgr InvitedPlayer = Helpers.GetInformation.GetPlayer(id);
                    Writer.Word(InvitedPlayer.Character.Information.Name.Length);
                    Writer.String(InvitedPlayer.Character.Information.Name);
                    Writer.Word(InvitedPlayer.Character.Network.Guild.Name.Length);
                    Writer.String(InvitedPlayer.Character.Network.Guild.Name);
                    break;
                case 2:
                    //Party invite
                    Writer.DWord(id);
                    Writer.Byte(type);
                    break;
                case 1:
                    //Exchange invite
                    Writer.DWord(id);
                    break;
            }

            return Writer.GetBytes();
        }
        public static byte[] JoinFormedRequest(WorldMgr.character requesting, WorldMgr.character owner)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode for packet
            Writer.Create(OperationCode.SERVER_PARTY_JOIN_FORMED);
            //Character model information (Req).
            Writer.DWord(requesting.Information.Model);
            //Leader id
            Writer.DWord(requesting.Information.UniqueID);
            //Party id
            Writer.DWord(owner.Network.Party.ptid);
            //Static
            Writer.DWord(0);
            Writer.DWord(0);
            Writer.Byte(0);
            Writer.Byte(0xFF);
            //Write character unique id
            Writer.DWord(requesting.Information.UniqueID);
            //Write character name
            Writer.Text(requesting.Information.Name);
            //Write model information
            Writer.DWord(requesting.Information.Model);
            //Write level information
            Writer.Byte(requesting.Information.Level);
            //Static
            Writer.Byte(0xAA);
            //X and Y Sector
            Writer.Byte(requesting.Position.xSec);
            Writer.Byte(requesting.Position.ySec);
            //Static
            Writer.Word(0);
            Writer.Word(0);
            Writer.Word(0);
            Writer.Word(1);
            Writer.Word(1);
            //If character is in a guild
            if (requesting.Network.Guild != null)
                //Write guild name
                Writer.Text(requesting.Network.Guild.Name);
            //If character is not in a guild
            else
                //Write word value 0
                Writer.Word(0);
            //Static
            Writer.Byte(0);
            Writer.DWord(0);
            Writer.DWord(0);
            //Return all bytes to send
            return Writer.GetBytes();
        }
    }
}
