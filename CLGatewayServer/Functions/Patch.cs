namespace Clowen.Functions
{
    class Patch
    {
        /*public static System.Collections.Generic.List<Patchs> qFilesLists = new System.Collections.Generic.List<Patchs>();
        public static void GetPatches(int Version)
		{
            // Load All patch Filesiles
		}
		class Patchs
		{
			public int FileID,Size;
			public string Name,Path;
			public bool Pk2Compressed;
		}
		public static byte[] SendPatchFiles()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x6100);
            //Writer.Byte(0); // --
            Writer.Byte(2);
            Writer.Byte(2);
            string ip = "127.0.0.1";
            //Writer.Byte((byte)ip.Length); // ip lenght     --       
            Writer.Text(ip); // DownloadServer IP
            Writer.Word(16002); // DownloadServer port
            Writer.DWord(Definitions.Serverdef.SilkroadClientVersion);
            Writer.Byte(0x01); // 1 patch have 0 not have
            GetPatches(Definitions.Serverdef.SilkroadClientVersion);

            for (int i = 0; i < qFilesLists.Count; i++)
            {
                 // new file;
                Writer.DWord(qFilesLists[i].FileID);
                Writer.Text(qFilesLists[i].Name);
                Writer.Text(qFilesLists[i].Path);
                Writer.DWord(qFilesLists[i].Size);
                Writer.Bool(qFilesLists[i].Pk2Compressed);
                if(i == qFilesLists.Count - 1)
                    Writer.Bool(true);
                else
                    Writer.Bool(false);
            }
            return Writer.GetBytes();
        }*/
    }
}
