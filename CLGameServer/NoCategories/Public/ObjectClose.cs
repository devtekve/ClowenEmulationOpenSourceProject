using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void SelectObject()
        {
            try
            {

                if (Character.Information.Scroll) return;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int objectid = Reader.Int32();
                if (objectid == 0) return;
                //Character.Action.Target = objectid;
                //if (Character.Position.Walking) StopPlayerMovementO(objectid);
                if (objectid == Character.Information.UniqueID) return;
                WorldMgr.Monsters o = Helpers.GetInformation.GetObject(objectid);
                if (o != null)
                {
                    byte[] bb = Packet.SelectObject(objectid, o.ID, o.LocalType, o.HP);
                    if (bb == null) return;
                    client.Send(bb);
                    //Character.Action.Object = o;
                    return;
                }
                PlayerMgr sys = Helpers.GetInformation.GetPlayers(objectid);
                if (o == null && sys != null)
                {
                    client.Send(Packet.SelectObject(objectid, 0, 5, sys.Character.Stat.Hp));
                    Character.Action.Object = sys;
                    return;
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Selectobject error: {0}", ex);
            }
        }
        public void HandleClosePet()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int petid = Reader.Int32();

                if (petid == Character.Transport.Horse.UniqueID)
                {
                    Send(Packet.Player_UpToHorse(Character.Information.UniqueID, false, petid));
                    client.Send(Packet.PetSpawn(petid, 1, Character.Transport.Horse));
                    Character.Transport.Horse.DeSpawnMe();
                    Character.Transport.Right = false;
                    if (Character.Position.Walking) Timer.Movement.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Close Pets
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ClosePet(int petid, WorldMgr.pet_obj o)
        {
            try
            {
                if (petid == o.UniqueID && o != null)
                {
                    Send(Packet.PetSpawn(petid, 1, o));
                    //Below is for icon change from flashing to none flashing in inventory
                    //client.Send(Packet.ChangeStatus(0,6, //Slot here //));
                    o.DeSpawnMe();
                }
            }
            catch (Exception ex) 
            {
                Log.Exception(ex);
            }
        }
    }
}
