using System;
using System.IO;
using System.Text;

namespace CLFramework
{
    public enum IPCCommand { IPC_REQUEST_SERVERINFO, IPC_REQUEST_LOGIN, IPC_INFO_SERVER, IPC_INFO_LOGIN }

    public class IPCPacket : IDisposable
    {
        MemoryStream ms;
        BinaryWriter bw;

        public IPCPacket()
        {
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
        }

        ~IPCPacket()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (ms != null)
            {
                bw.Close();
                ms.Close();
                bw = null;
                ms = null;
            }
        }

        public void WriteByte(byte aByte)
        {
            bw.Write(aByte);
        }

        public void WriteWord(ushort aWord)
        {
            bw.Write(aWord);
        }

        public void WriteString(string aString)
        {
            WriteByte((byte)aString.Length);
            bw.Write(Encoding.ASCII.GetBytes(aString));
        }

        public void WriteBytes(byte[] aBytes)
        {
            bw.Write(aBytes);
        }

        public void AddCRC()
        {
            long oPos = ms.Position;
            ms.Position = 0;
            bw.Flush();
            byte crc = Servers.BCRC(ms.ToArray());
            ms.Position = oPos;
            WriteByte(crc);
        }

        public byte[] GetBytes()
        {
            long oPos = ms.Position;
            ms.Position = 0;
            bw.Flush();
            byte[] arr = ms.ToArray();
            ms.Position = oPos;
            return arr;
        }
    }
}
