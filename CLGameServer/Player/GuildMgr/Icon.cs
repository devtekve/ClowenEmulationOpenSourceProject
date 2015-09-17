using System;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        void HandleRegisterIcon()
        {
            try
            {
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                int iconlenght = reader.Int32();
                string icon = reader.Text();
                reader.Close();

                string convertedicon = ConvertToHex(icon);
                //Save output to .dat file in hex formatting.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild icon register error {0}", ex);
            }
        }

        string ConvertToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
    }
}
