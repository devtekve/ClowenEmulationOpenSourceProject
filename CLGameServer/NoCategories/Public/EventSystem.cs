using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {

        public void Gameguide()//Will need to read this byte by byte to get the id for the server to record for the chardata
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                bool Guideok = false;
                int[] b1 = new int[8];
                for (int b = 0; b < 8; ++b)//Reads Guide Data
                {
                    b1[b] = Reader.Byte();//Puts into a int Array
                }

                for (int gc = 0; gc < 8; ++gc)//This Checks The Last Send Guide Paket To Make Sure The Same Packet Is Not Read Twice
                {
                    if (b1[gc] == Character.Guideinfo.Gchk[gc])
                    {
                        Guideok = false;//If Guide Packet Has Been Sent Will Return False
                    }
                    else
                    {
                        Guideok = true;//If Guide Packet Is New Will Retun True And Break
                        break;
                    }
                }

                if (Guideok)
                {
                    for (int gc = 0; gc < 8; ++gc)// Guide Packet Check
                    {
                        Character.Guideinfo.Gchk[gc] = b1[gc];//Adds Packet To Int Array
                    }

                    for (int gi = 0; gi < 8; ++gi)//Guide Packet Update For Save And Teleport,Return,Etc
                    {
                        Character.Guideinfo.G1[gi] = Character.Guideinfo.G1[gi] + b1[gi];//Adds The Packet And Updates The Data
                    }
                    PacketWriter Writer = new PacketWriter();//Writes the Packet Responce For Guide Window
                    Writer.Create(OperationCode.SERVER_SEND_GUIDE);
                    Writer.Byte(1);
                    for (int b = 0; b < 8; ++b)
                    {
                        Writer.Byte(b1[b]);
                    }
                    client.Send(Writer.GetBytes());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}