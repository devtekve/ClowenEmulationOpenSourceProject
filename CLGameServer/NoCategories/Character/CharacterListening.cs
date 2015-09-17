using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        #region Listing
        void CharacterListing()
        {
            //Wrap the function in catcher
            try
            {
                //Send packet information character listening
                client.Send(Packet.CharacterListing(Player.AccountName));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Delete & Restore
        void CharacterDelete()
        {
            //Wrap our function in a catcher
            try
            {
                //Open our reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte unused = Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                //Update character deletion information
                DB.query("UPDATE character SET deletedtime=dateadd(dd,7,getdate()) WHERE name='" + name + "'");
                //Update character visual screen
                client.Send(Packet.ScreenSuccess(3));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Character restoring
        void CharacterRestore()
        {
            //Wrap our function in a catcher
            try
            {
                //Open packet readers
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte unused = Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                //Update database information set delete time to 0
                DB.query("UPDATE character SET deletedtime=0 WHERE name='" + name + "'");
                //Update visually screen with packet
                client.Send(Packet.ScreenSuccess(5));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }   
        }
        #endregion
        #region Character job info
        void CharacterJobInfo()
        {
            //Wrap our function into catcher
            try
            {
                //Set defaul int's to 0
                int huntercount = 0;
                int thiefcount = 0;
                //Update hunter and thief count.
                huntercount = DB.GetRowsCount("SELECT * FROM users WHERE jobtype='1'");
                thiefcount = DB.GetRowsCount("SELECT * FROM users WHERE jobtype='2'");
                //Set total count
                int jobplayercount = thiefcount + huntercount;
                //Set our null to 1 if this would happen
                if (jobplayercount == 0) jobplayercount = 1;
                //Calculate our % for the jobs
                double thiefpercentage = (double)thiefcount / (double)jobplayercount * 100.0;
                double hunterpercentage = (double)huntercount / (double)jobplayercount * 100.0;
                //Send visual packet for job %
                client.Send(Packet.CharacterScreenJobs(hunterpercentage, thiefpercentage));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        #region Character job selection
        void CharacterJobPick(byte[] buff)
        {
            //Wrap our function inside a catcher
            try
            {
                //Open packet reader
                PacketReader Reader = new PacketReader(buff);
                Reader.Skip(1);
                short namel = Reader.Int16();
                string name = Reader.String(namel);
                byte job = Reader.Byte();
                Reader.Close();

                //Anti hack check
                string namecheck = DB.GetData("SELECT name FROM character WHERE account='" + Player.AccountName + "'", "name");
                int jobcheck = Convert.ToInt32(DB.GetData("SELECT jobtype FROM users WHERE id='" + Player.AccountName + "'", "jobtype"));

                //If the name check is succesfull and account has no job set.
                if (jobcheck == 0 && namecheck.Length > 0)
                {
                    //Write new job info 
                    DB.query("UPDATE users SET jobtype='" + job + "' WHERE id='" + Player.AccountName + "'");
                }
                //Send visual confirmation
                client.Send(Packet.CharacterJobSelection());
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
    }
}