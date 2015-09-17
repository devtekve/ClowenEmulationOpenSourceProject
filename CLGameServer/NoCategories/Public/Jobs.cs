using System;
using System.Linq;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void JoinMerc()
        {
            try
            {
                DB ms = new DB("SELECT * FROM character_jobs WHERE character_name='" + Character.Information.Name + "'");
                int checkjob = ms.Count();

                if (checkjob == 0)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    int id = Reader.Int32();
                    byte type = Reader.Byte();
                    /////////////////////////////////////////////////////////////////////////////////////
                    client.Send(Packet.InfoUpdate(1, Character.Information.CharacterID, 0));
                    client.Send(Packet.JoinMerchant(id, type));
                    /////////////////////////////////////////////////////////////////////////////////////
                    DB.query("INSERT INTO character_jobs (character_name, job_type) VALUES ('" + Character.Information.Name + "','2')");
                    ms.Close();
                }
                else
                {
                    // Not needed cant join job because excist
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void PrevJob()
        {
            client.Send(Packet.PrevJobInfo(Character.Information.UniqueID, PacketInformation.buffer[0]));
        }
        public void LeaveJob()
        {
            try
            {
                DB.query("delete FROM character_jobs WHERE character_name='" + Character.Information.Name + "'");
                client.Send(Packet.LeaveJob());
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void MakeAlias()
        {
            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int id = Reader.Int32();
                byte type = Reader.Byte();
                short nLenght = Reader.Int16();
                string name = Reader.String(nLenght);
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine(name);
                DB ms = new DB("SELECT * FROM character_jobs WHERE job_alias='" + name + "'");
                int checkjob = ms.Count();
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                client.Send(Packet.MakeAlias(name, type));
                if (checkjob == 0)
                {
                    client.Send(Packet.MakeAlias(name, type));
                    DB.query("UPDATE character_jobs SET job_alias='" + name + "' WHERE character_name='" + Character.Information.Name + "'");
                }
                else if (checkjob >= 0)
                {
                    client.Send(Packet.MakeAliasError(name, type));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
