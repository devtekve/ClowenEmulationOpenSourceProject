using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {

        int GetMovementTime(double Distance)
        {

            if (Character.Information.Berserking)
            {
                Character.Position.Time = (Distance / Character.Speed.BerserkSpeed + (Character.Speed.BerserkSpeed * Character.Speed.INC / 100) * 0.0768 ) * 1000.0;
            }
            else if (Character.Position.WalkRun)
            {
                Character.Position.Time = (Distance / Character.Speed.WalkSpeed + (Character.Speed.WalkSpeed * Character.Speed.INC / 100) * 0.0768) * 1000.0;
            }
            else
            {
                Character.Position.Time = (Distance / Character.Speed.RunSpeed + (Character.Speed.RunSpeed * Character.Speed.INC / 100) * 0.0768) * 1000.0;
            }
            Character.Position.RecordedTime = Character.Position.Time;
            return Convert.ToInt32(Character.Position.RecordedTime * 0.1095);

        }
        /////////////////////////////////////////////////////////////////////////////////
        // Movement
        /////////////////////////////////////////////////////////////////////////////////
        public void Movement()
        {
            try
            {
                //foreach (var item in ObjData.Manager.MapObject)
                //{
                //    for (int i2 = 0; i2 < ObjData.Manager.MapObject[item.Key].entitys.Count; i2++)
                //    {
                //        if (ObjData.Manager.MapObject[item.Key].entitys[i2].OutLines.Exists(te => te.PointA == Formule.packetx(Character.Position.x, Character.Position.xSec) && te.PointB == Formule.packety(Character.Position.y, Character.Position.ySec)))
                //        {
                //            Console.WriteLine("Çarpışma Bulundu");
                //        }
                //        else if (ObjData.Manager.MapObject[item.Key].entitys[i2].Points.Exists(te => te.x == Formule.packetx(Character.Position.x, Character.Position.xSec) && te.y == Formule.packety(Character.Position.y, Character.Position.ySec)))
                //        {
                //            Console.WriteLine("Çarpışma bulundu2");
                //        }
                //    }
                //}
                #region Check
                Character.Action.Object = null;
                Character.Action.nAttack = false;
                Character.Action.Target = -1;
                if (Timer.Pickup != null && Character.Action.PickUping)
                {
                    StopPickupTimer();
                }
                if (Character.Information.SkyDroming)
                {
                    StopSkyDromeTimer();
                }
                if (Character.Action.nAttack)
                {
                    StopAttackTimer();
                    Character.Action.nAttack = false;
                }
                if (Character.Action.sAttack)
                {
                    StopAttackTimer();
                    Character.Action.sAttack = false;
                }
                if (Character.Action.sCasting)
                {
                    StopAttackTimer();
                    Character.Action.sCasting = false;
                }
                if (Character.Information.PvpWait)
                {
                    Send(Packet.PvpInterupt(Character.Information.UniqueID));
                    Character.Information.PvpWait = false;
                    Character.Information.Pvptype = 0;
                    StopPvpTimer();

                }
                // returned
                if (Character.Stall.Stallactive || 
                    Character.Information.SkyDroming || 
                    Character.State.Die || 
                    Character.State.Sitting || 
                    Character.Information.Scroll ||
                    Character.Action.sCasting ||
                    Character.Action.sAttack ||
                    Character.Action.nAttack)
                {
                    return;
                }
                #endregion
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte MovementFlag = Reader.Byte();
                if (MovementFlag == 0)
                {
                    MovementSkyClicking(Reader);
                }
                if (MovementFlag == 1)
                {
                    MovementNormalClicking(Reader);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Movement error: {0}", exception);
            }
        }
        void MovementSkyClicking(PacketReader Reader)
        {
            Character.Position.Walking = true;
            byte Type = Reader.Byte();
            ushort Angle = Reader.UInt16();
            Character.Information.Angle = (Angle) / 182.04166666666666;
            Character.Position.packetxSec = Character.Position.xSec;
            Character.Position.packetySec = Character.Position.ySec;
            Character.Position.packetX = (ushort)Formule.gamex(Character.Position.x, Character.Position.xSec);
            Character.Position.packetY = (ushort)Formule.gamex(Character.Position.y, Character.Position.ySec);
            double Distance = Formule.gamedistance(Character.Position.x, Character.Position.y, Formule.gamex((float)Character.Position.packetX, Character.Position.xSec), Formule.gamey((float)Character.Position.packetY, Character.Position.ySec));
            PacketWriter writer = new PacketWriter();
            writer.Create(0xb021);
            writer.DWord(Character.Information.UniqueID);
            writer.Byte(0);
            writer.Byte(Type);
            writer.Word(Angle);
            writer.Byte(1);
            writer.Byte(Character.Position.xSec);
            writer.Byte(Character.Position.ySec);
            writer.Word(Character.Position.packetX);
            writer.DWord(Character.Position.z);
            writer.Word(Character.Position.packetY);
            Send(writer.GetBytes());
            Reader.Close();
            StartSkyDromeTimer(1000);
        }
        void MovementNormalClicking(PacketReader Reader)
        {
            float XPosition = 0;
            float YPosition = 0;
            float ZPosition = 0;
            byte xsec = Reader.Byte();
            byte ysec = Reader.Byte();
            double Distance = 0;
            Character.Position.Walking = true;

            if (!FileDB.CheckCave(xsec, ysec))
            {
                XPosition = Reader.Int16();
                ZPosition = Reader.Int16();
                YPosition = Reader.Int16();
                Distance = Formule.gamedistance(Character.Position.x, Character.Position.y, Formule.gamex(XPosition, xsec), Formule.gamey(YPosition, ysec));
                Character.Position.xSec = xsec;
                Character.Position.ySec = ysec;
                Character.Position.wX = Formule.gamex(XPosition, xsec) - Character.Position.x;
                Character.Position.wZ = ZPosition;
                Character.Position.wY = Formule.gamey(YPosition, ysec) - Character.Position.y;
                Character.Position.packetxSec = xsec;
                Character.Position.packetySec = ysec;
                Character.Position.packetX = (ushort)XPosition;
                Character.Position.packetZ = (ushort)ZPosition;
                Character.Position.packetY = (ushort)YPosition;
                if ((xsec != 0) && (ysec != 0))
                {
                    Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID, XPosition, ZPosition, YPosition, xsec, ysec)));
                }
                StartMovementTimer(GetMovementTime(Distance));
            }
            else
            {
                XPosition = Formule.cavegamex((float)Reader.Int16(), (float)Reader.Int16());
                ZPosition = Formule.cavegamez((float)Reader.Int16(), (float)Reader.Int16());
                YPosition = Formule.cavegamey((float)Reader.Int16(), (float)Reader.Int16());
                Distance = Formule.gamedistance(Character.Position.x, Character.Position.y, Formule.cavegamex(XPosition), Formule.cavegamey(YPosition));
                Character.Position.xSec = xsec;
                Character.Position.ySec = ysec;
                Character.Position.wX = Formule.cavegamex(XPosition) - Character.Position.x;
                Character.Position.wZ = ZPosition;
                Character.Position.wY = Formule.cavegamey(YPosition) - Character.Position.y;
                Character.Position.packetxSec = xsec;
                Character.Position.packetySec = ysec;
                Character.Position.packetX = (ushort)XPosition;
                Character.Position.packetZ = (ushort)ZPosition;
                Character.Position.packetY = (ushort)YPosition;
                if ((xsec != 0) && (ysec != 0))
                {
                    Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID, XPosition, ZPosition, YPosition, xsec, ysec)));
                }
                StartMovementTimer(GetMovementTime(Distance));
            }
            Reader.Close();
            if (Character.Grabpet.Active)
            {
                Send(Packet.Movement(new ObjData.vektor(Character.Grabpet.Details.UniqueID, XPosition + Rnd.Next(10, 15), ZPosition, YPosition + Rnd.Next(10, 15), xsec, ysec)));
            }
            if (Character.Attackpet.Active)
            {
                Send(Packet.Movement(new ObjData.vektor(Character.Attackpet.Details.UniqueID, XPosition + Rnd.Next(10, 15), ZPosition, YPosition + Rnd.Next(10, 15), xsec, ysec)));
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Pet movement
        /////////////////////////////////////////////////////////////////////////////////
        public void MovementPet()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //This one happens to all pets.
                int petid = Reader.Int32();
                //We switch on type (2 = attack pet, 1 = horse).
                byte type = Reader.Byte();

                switch (type)
                {
                    //Horse pet movement
                    case 1:
                        byte movetype = Reader.Byte();
                        byte xsec = Reader.Byte();
                        byte ysec = Reader.Byte();
                        float x = Reader.Int16();
                        float z = Reader.Int16();
                        float y = Reader.Int16();
                        Reader.Close();
                        //Make sure attack timer is gone
                        StopAttackTimer();
                        //Set pickup to false
                        Character.Action.PickUping = false;
                        //Set movement active
                        Character.Position.Walking = true;

                        double distance = Formule.gamedistance(Character.Position.x,
                            Character.Position.y,
                            Formule.gamex(x, xsec),
                            Formule.gamey(y, ysec));

                        Character.Position.xSec = xsec;
                        Character.Position.ySec = ysec;
                        Character.Position.wX = Formule.gamex(x, xsec) - Character.Position.x;
                        Character.Position.wZ = z;
                        Character.Position.wY = Formule.gamey(y, ysec) - Character.Position.y;

                        Character.Position.packetxSec = xsec;
                        Character.Position.packetySec = ysec;
                        Character.Position.packetX = (ushort)x;
                        Character.Position.packetZ = (ushort)z;
                        Character.Position.packetY = (ushort)y;

                        Send(Packet.Movement(new ObjData.vektor(petid, x, z, y, xsec, ysec)));
                        
                        break;
                    //Attack pet movement
                    case 2:
                        //Set pet info
                        Character.Attackpet.Details.x = Character.Position.x;
                        Character.Attackpet.Details.y = Character.Position.y;
                        Character.Attackpet.Details.z = Character.Position.z;
                        Character.Attackpet.Details.xSec = Character.Position.xSec;
                        Character.Attackpet.Details.ySec = Character.Position.ySec;
                        //Target id information
                        int targetid = Reader.Int32();
                        Reader.Close();
                        //Set pet speed information
                        Send(Packet.SetSpeed(petid, 50, 100));//Need to make correct speed info later
                        //Check distances / target detailed.

                        //Send attack packet (new void pet attack to be created).
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Movement angle
        /////////////////////////////////////////////////////////////////////////////////
        public void Angle()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            ushort angle = Reader.UInt16();
            client.Send(Packet.Angle(Character.Information.UniqueID, angle));
            client.Send(Packet.ChatPacket(7, 0, string.Format("ANGLE:{0}", angle), ""));
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Cave movement teleport
        /////////////////////////////////////////////////////////////////////////////////
        public void Movement_CaveTeleport()// This was changed due to going in and out of caves to change the movment patten
        {
            try
            {
                // if our destination is caveteleport
                foreach (ObjData.CaveTeleports r in ObjData.Manager.CaveTeleports)
                {
                    if (!FileDB.CheckCave(Character.Position.xSec, Character.Position.ySec))
                    {
                        if (Formule.gamedistance(Formule.packetx(Character.Position.x, Character.Position.xSec), Formule.packety(Character.Position.y, Character.Position.ySec), (float)r.x, (float)r.y) <= 10)
                        {
                            foreach (ObjData.cavepoint p in ObjData.Manager.cavePointBase)
                            {
                                if (p != null)
                                    if (p.Name == r.name)
                                    {
                                        TeleportCave(p.Number);
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (Formule.gamedistance(Formule.cavepacketx(Character.Position.x), Formule.cavepackety(Character.Position.y), (float)r.x, (float)r.y) <= 10)
                        {
                            foreach (ObjData.cavepoint p in ObjData.Manager.cavePointBase)
                            {
                                if (p != null)
                                    if (p.Name == r.name)
                                    {
                                        TeleportCave(p.Number);
                                        break;
                                    }
                            }
                            break;
                        }
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
