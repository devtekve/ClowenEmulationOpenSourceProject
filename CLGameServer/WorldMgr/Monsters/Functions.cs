using CLFramework;
using System;
using System.Collections.Generic;

namespace CLGameServer.WorldMgr
{
    public partial class Monsters
    {
        public void GotoPlayer(character Player, double distance)
        {
            try
            {
                StopMovement();

                float farkx = Player.Position.x + 1; // later have to modify
                float farky = Player.Position.y + 1;
                //Systems.aRound(ref Player.aRound, ref farkx, ref farky);

                xSec = (byte)((farkx / 192) + 135);
                ySec = (byte)((farky / 192) + 92);

                //don't follow the player into the town.
                bool inTown = ObjData.Manager.safeZone.Exists(
                    delegate (ObjData.region r)
                    {
                        if (r.SecX == xSec && r.SecY == ySec)
                        {
                            return true;
                        }
                        return false;
                    });

                Send(Client.Packet.Movement(new ObjData.vektor(UniqueID,
                    (float)Formule.packetx((float)farkx, Player.Position.xSec),
                    (float)Player.Position.z,
                    (float)Formule.packety((float)farky, Player.Position.ySec),
                    xSec,
                    ySec)));

                if (inTown)
                {
                    AttackStop();
                    Attacking = false;

                    GetDie = true;
                    Die = true;
                    DeSpawnMe();

                    return;
                }

                // Keep track of the current position of the mob.
                wx = farkx - x;
                wy = farky - y;
                // Calc for time if mob follows initalise speed info Run
                WalkingTime = (distance / (RunningSpeed * 0.0768)) * 1000.0;
                RecordedTime = WalkingTime;

                StartMovement((int)(WalkingTime / 10));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public byte DeBuffGetFreeSlot()
        {
            try
            {
                for (byte b = 0; b <= DeBuff.Effect.EffectID.Length - 1; b++)
                    if (DeBuff.Effect.EffectID[b] == 0) return b;

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 255;
        }
        public void ChangeState(byte type, byte type2)
        {
            Send(Client.Packet.StatePack(UniqueID, type, type2, false));
            LastState = type;
            LastState2 = type2;
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        if (Helpers.Manager.clients[i] != null && Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                        {
                            if (!Helpers.Manager.clients[i].Character.Spawning)
                            {
                                Helpers.Manager.clients[i].client.Send(buff);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void SpawnMe()
        {
            try
            {
                if (Die) return;
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                            PlayerMgr sys = Helpers.Manager.clients[i];
                        if (x >= (sys.Character.Position.x - 50) && x <= ((sys.Character.Position.x - 50) + 100) && y >= (sys.Character.Position.y - 50) && y <= ((sys.Character.Position.y - 50) + 100) && Spawned(sys.Character.Information.UniqueID) == false)
                        {
                            Spawn.Add(sys.Character.Information.UniqueID);
                            sys.client.Send(Client.Packet.ObjectSpawn(this));
                        }
                    }
                }

                if (AutoMovement && AutoRun == null)
                {
                    StartRunTimer(Rnd.Next(1, 5) * 1000);
                }

                if (Agresif == 1)
                {
                    StopAgressiveTimer(); StartAgressiveTimer(1000);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                //StopAutoRunTimer();
                StopAgressiveTimer();
                if (Die)
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            try
                            {
                                PlayerMgr sys = Helpers.Manager.clients[i];
                                if (Spawned(sys.Character.Information.UniqueID))
                                {
                                    Spawn.Remove(sys.Character.Information.UniqueID);
                                    sys.client.Send(Client.Packet.ObjectDeSpawn(UniqueID));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Exception(ex);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public List<PlayerMgr> GetRangePlayers(int dist)
        {
            try
            {
                List<PlayerMgr> Players = new List<PlayerMgr>();
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {

                        PlayerMgr s = Helpers.Manager.clients[i];
                        //double distance = Formule.gamedistance((float)x, (float)y, s.Character.Position.x, s.Character.Position.y);
                        double distance = Formule.gamedistance(this, s.Character.Position);
                        if (distance <= dist)
                            Players.Add(s);
                    }
                    return Players;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
        public void reSpawn()
        {
            try
            {
                AgressiveDMG.Clear();
                HP = ObjData.Manager.ObjectBase[ID].HP;
                Agresif = oldAgresif;
                Attacking = false;
                Helpers.Functions.aRound(ref x, ref y, 2);
                OriginalX = x;
                OriginalY = y;
                List<PlayerMgr> Players = new List<PlayerMgr>();
                Players = GetRangePlayers(50);

                bool isSharePartyInRange = Players.Exists(
                delegate (PlayerMgr s)
                {
                    bool retValue = false;

                    // player has exp/item share pt?
                    if (s.Character.Network.Party != null)
                        if (
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.EXPSHARE ||
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.EXPSHARE_NO_PERMISSION ||
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.FULLSHARE ||
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.FULLSHARE_NO_PERMISSION ||
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.ITEMSHARE ||
                        s.Character.Network.Party.Type == (byte)PlayerMgr.PartyTypes.ITEMSHARE_NO_PERMISSION
                        )
                        {
                            // at least 2 players in range
                            bool isInRange = s.Character.Network.Party.Members.Exists(
                            delegate (int m)
                            {
                                // if me -> false
                                if (m == s.Character.Information.UniqueID) return false;

                                PlayerMgr ptmate = Helpers.GetInformation.GetPlayer(m);

                                if (Players.Exists(sys => (sys.Character.Information.UniqueID == ptmate.Character.Information.UniqueID)))
                                    return true;
                                else
                                    return false;
                            }
                            );
                            if (isInRange) retValue = true;
                        }

                    return retValue;
                }
                );

                if (!isSharePartyInRange)
                    Type = Helpers.Functions.RandomType(ObjData.Manager.ObjectBase[ID].Level, ref Kat, false, ref Agresif);
                else
                    Type = Helpers.Functions.RandomType(ObjData.Manager.ObjectBase[ID].Level, ref Kat, true, ref Agresif);

                switch (Type)
                {
                    case 1:
                        Agresif = 1;
                        HP *= 2;
                        break;
                    case 4:
                        HP *= 20;
                        Agresif = 1;
                        break;
                    case 5:
                        HP *= 100;
                        Agresif = 1;
                        break;
                    case 16:
                        HP *= 10;
                        break;
                    case 17:
                        HP *= 20;
                        Agresif = 1;
                        break;
                    case 20:
                        HP *= 210;
                        Agresif = 1;
                        break;
                }

                GetDie = false;
                Die = false;
                Busy = false;

                SpawnMe();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void RandomMonster(int sID, byte randomTYPE)
        {
            try
            {
                if (ObjData.Manager.ObjectBase[sID].SpeedWalk == 0 && ObjData.Manager.ObjectBase[sID].SpeedRun == 0)
                {
                    return;
                }
                Monsters o = new Monsters();
                o.ID = sID;
                o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = x;
                o.z = z;
                o.y = y;
                Helpers.Functions.aRound(ref o.x, ref o.y, 2);
                o.UniqueGuardMode = true;
                o.OriginalY = OriginalY;
                o.OriginalX = OriginalX;


                o.xSec = xSec;
                o.ySec = ySec;

                o.AutoMovement = AutoMovement;
                o.AutoSpawn = true;
                o.Move = 1;

                o.HP = ObjData.Manager.ObjectBase[o.ID].HP;
                o.WalkingSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedWalk;
                o.RunningSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedRun;
                o.BerserkerSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedZerk;
                o.Agresif = 0;
                o.LocalType = 1;
                o.State = 2;
                o.Kat = 1;
                o.spawnOran = 0;

                if (randomTYPE == 0) // Standart
                {
                    o.Type = Helpers.Functions.RandomType(ObjData.Manager.ObjectBase[o.ID].Level, ref Kat, false, ref o.Agresif);
                    if (o.Type == 1) o.Agresif = 1;
                    if (ObjData.Manager.ObjectBase[o.ID].Agresif == 1)
                    {
                        o.Agresif = 1;
                    }
                    o.HP *= Kat;
                }
                else
                {
                    if (randomTYPE == 6)
                        o.HP *= 4;
                    else if (randomTYPE == 4)
                        o.HP *= 20;
                    else if (randomTYPE == 1)
                        o.HP *= 2;
                    else if (randomTYPE == 16)
                        o.HP *= 10;
                    else if (randomTYPE == 17)
                        o.HP *= 20;
                    else if (randomTYPE == 20)
                        o.HP *= 210;
                    o.AutoSpawn = false;
                    o.Type = randomTYPE;
                    o.Agresif = 1;
                }
                Helpers.Manager.Objects.Add(o);
                o.SpawnMe();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void CheckUnique()
        {
            try
            {
                if (Type == 3)
                    if (ID == 1954 || ID == 5871 || ID == 1982 || ID == 2002 || ID == 3810 || ID == 3875 || ID == 14538)
                    {
                        int yuzde = ((HP * 100) / ObjData.Manager.ObjectBase[ID].HP);
                        int[] bs = Helpers.Functions.GetEliteIds(ID);
                        if (yuzde > 99)
                        {
                            if (!guard[0])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 1);
                                    guard[0] = true;
                                }
                        }
                        else if (yuzde < 80 && yuzde > 70)
                        {
                            if (!guard[1])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 6);
                                    guard[1] = true;
                                }
                        }
                        else if (yuzde < 60 && yuzde > 50)
                        {
                            if (!guard[2])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 4);
                                    guard[2] = true;
                                }
                        }
                    }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void CheckUnique(string CharacterName)
        {
            try
            {
                if (Type == 3)
                {
                    Helpers.SendToClient.SendAll(Client.Packet.Unique_Data(6, ID, CharacterName));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
