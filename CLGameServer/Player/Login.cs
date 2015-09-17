using CLGameServer.Client;
using CLFramework;
using System;

namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void Connect()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte type = Reader.Byte();
                Reader.Skip(3);
                string ID = Reader.Text();
                string PW = Reader.Text();
                Reader.Close();
                //Set login result information
                int LoginResult = LoginUser(ID, ref PW, ref Player, true);
                //If the login is succesfull
                if (LoginResult == 4)
                {
                    //Send succes packet
                    client.Send(Packet.ConnectSuccess());
                }
                //If the login is wrong
                else
                {
                    //Disconnect the user
                    client.Disconnect(PacketInformation.Client);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static ushort LoginUser(string aID, ref string aPass, ref WorldMgr.player aPlayer, bool localConnect)
        {
            if (Helpers.Manager.clients.Count >= Helpers.Manager.maxSlots)
            {
                return 0; // crowded
            }
            DB ms = new DB("SELECT * FROM users WHERE id = '" + aID + "'");
            if (ms.Count() == 0)
            {
                ms.Close();
                return 1;
            }
            ms = new DB("SELECT * FROM users WHERE id = '" + aID + "' AND password='" + cMD5.ConvertStringToMD5(aPass) + "'");
            if (ms.Count() == 0)
            {
                ms.Close();
                return 5;
            }
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1).ToLower() == aID.ToLower()) // id
                    {
                        if (reader.GetByte(3) == 1) // online
                        {
                            ms.Close();
                            return 2; // already online
                        }

                        if (reader.GetInt32(5) == 1) // banned
                        {
                            aPass = reader.GetString(4);
                            ms.Close();
                            return 3; // banned
                        }

                        if (aPlayer == null && localConnect) DB.query("UPDATE users SET online=1 WHERE userid='" + reader.GetInt32(0) + "'");
                        aPlayer = new WorldMgr.player();
                        aPlayer.AccountName = aID;
                        aPlayer.Password = aPass; // Nukei: ?? whats the reason for saving password in memory ?
                        aPlayer.ID = reader.GetInt32(0);
                        aPlayer.pGold = reader.GetInt64(7);
                        aPlayer.Silk = reader.GetInt32(6);
                        aPlayer.SilkPrem = reader.GetInt32(9);
                        aPlayer.wSlots = reader.GetByte(11);
                        ms.Close();
                        return 4;
                    }
                }
            }
            ms.Close();
            return 6; // Bilinmeyen geri dönüş:Özel bir durum oluşmadı. (Mecburi Gönderim)
        }
    }
}
