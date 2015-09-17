using CLFramework;

namespace Clowen.Core.Packets
{
    class News
    {
        public static byte[] NewsPacket()
        {
            PacketWriter Writer = new PacketWriter();

            Writer.Create(Opcodes.SERVER.SERVER_MAIN);
            Writer.Byte(0);
            Writer.Byte((byte)Definitions.Serverdef.News_List.Count);

            foreach (Definitions.Serverdef.NewsList n in Definitions.Serverdef.News_List)
            {
                Writer.Text(n.Title);
                Writer.Text(n.Article);
                Writer.Word(n.Year);
                Writer.Word(n.Month);
                Writer.Word(n.Day);
                Writer.Word(0); // Hour
                Writer.Word(0); // Minute
                Writer.Word(0); // Second
                Writer.Word(0); // MiliSecond
            }

            Writer.Word(0);

            return Writer.GetBytes();
        }
    }
}
