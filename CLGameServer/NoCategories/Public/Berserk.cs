using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void Player_Berserk_Up()
        {
            try
            {
                if (!Character.Information.Berserking && !Character.Information.Invisible)
                {
                    Character.Information.BerserkBar = 0;
                    client.Send(Packet.InfoUpdate(4, 0, Character.Information.BerserkBar));
                    Character.Information.Berserking = true;

                    if (Character.Information.Title != 0) Character.Information.BerserkOran = 230;
                    else Character.Information.BerserkOran = 200;
                    Send(Packet.StatePack(Character.Information.UniqueID, 4, 1, false));
                    
                    Character.Speed.INC = 100;
                    Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed + (Character.Speed.WalkSpeed * Character.Speed.INC / 100), Character.Speed.RunSpeed + (Character.Speed.RunSpeed * Character.Speed.INC / 100)));

                    DB.query("update character set berserkbar='" + Character.Information.BerserkBar + "' where id='" + Character.Information.CharacterID + "'");
                    StartBerserkerTimer(60000);
                }
                else
                {
                    client.Send(Packet.BerserkNotRun());
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Player_Berserk_Down()
        {
            try
            {
                Character.Information.Berserking    = false;
                Character.Information.BerserkBar    = 0;
                Character.Speed.INC -= 100;

                Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                Send(Packet.StatePack(Character.Information.UniqueID, 4, 0, false));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void GetBerserkOrb()
        {
            try
            {
                
                if (Character.Information.BerserkBar < 5 && !Character.Information.Berserking)
                {
                    if (Rnd.Next(0, 100) > 70)
                    {
                        Character.Information.BerserkBar++;
                        client.Send(Packet.InfoUpdate(4, 0, Character.Information.BerserkBar));

                        DB.query("update character set berserkbar='" + Character.Information.BerserkBar + "' where id='" + Character.Information.CharacterID + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
