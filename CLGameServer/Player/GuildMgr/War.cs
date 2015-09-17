using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void GuildWarGold()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            int guildid = Reader.Int32();
            Reader.Close();

            if (Character.Guild.GuildWarGold == 0)
            {
                //Send Packet Message No War Gold Received
                client.Send(Packet.GuildWarMsg(2));
            }
            else
            {
                //Sniff packet for war gold
            }

        }
    }
}
