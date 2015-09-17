using CLFramework;
using System;
using System.Diagnostics;
using System.Threading;

namespace CLGameServer.WorldMgr
{
    public partial class Monsters
    {
        public Thread AttackHandle;
        public void CheckAgro()
        {
            try
            {
                if (AgressiveDMG.Count > 0)
                {
                    if (Attacking == false)
                    {
                        if (AttackHandle != null)
                            if (AttackHandle.IsAlive)
                                return;

                        AttackHandle = new Thread(new ThreadStart(AttackMain));
                        AttackHandle.Start();
                    }
                }
                else
                {
                    if (AutoRun == null && AutoMovement)
                    {
                        StartRunTimer(Rnd.Next(1, 5) * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void CheckEveryOne(object e = null)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int b = 0; b < Helpers.Manager.clients.Count; b++)
                    {
                        if (Spawned(Helpers.Manager.clients[b].Character.Information.UniqueID))
                            FollowHim(Helpers.Manager.clients[b]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void AttackMain() // have to add this to thread pool
        {
            try
            {
                PlayerMgr sys;
                double distance;

                if (AgressiveDMG != null) sys = (PlayerMgr)GetTarget();
                else { return; }
                //Print.Format("AttackMain()");
                if (bSleep) return;

                if (sys == null || Die || GetDie) { Attacking = false; return; }
                if (sys != null && !Spawned(sys.Character.Information.UniqueID)) { Attacking = false; return; }
                if (!sys.MonsterCheck(UniqueID)) sys.Character.Action.MonsterID.Add(UniqueID);
                if (!sys.Character.InGame) { Attacking = false; return; }

                if (AutoMovement) StopAutoRunTimer();
                else StopAgressiveTimer();

                Busy = true;
                Attacking = true;
                bool Hit = false;

                int AttackType = 0;
                int AttackDistance = 0;
                int AttackTime = 1000;

                int acount = UniqueID;

                //mob starts running
                ChangeState(1, 3);

                Stopwatch PursuitWatch = new Stopwatch();
                PursuitWatch.Start();

                // mob brain loop
                while (Attacking)
                {
                    if (sys == null || Die || GetDie) { AttackStop(); break; }
                    if (sys != null && !Spawned(sys.Character.Information.UniqueID)) { AttackStop(); break; }
                    if (!sys.Character.InGame) { AttackStop(); break; }

                    // make every new skill random
                    AttackType = ObjData.Manager.ObjectBase[ID].Skill[Rnd.Next(0, ObjData.Manager.ObjectBase[ID].amountSkill)];
                    AttackDistance = ObjData.Manager.SkillBase[AttackType].Distance;
                    AttackTime = ObjData.Manager.SkillBase[AttackType].Action_CastingTime;

                    distance = Formule.gamedistance((float)x, (float)y, sys.Character.Position.x, sys.Character.Position.y);

                    // mob's attack ranged
                    if (AttackDistance != 0)
                    {
                        distance -= AttackDistance;
                        if (distance < 0) distance = 0;
                    }

                    #region Monster action switch
                    // stop pursuit coz player too far
                    if (distance > 19 /*&& Attack != null*/)
                    {
                        ChangeState(8, 1);
                        AttackStop();
                        break;
                    }
                    // player try to escape from mob's agro => go and pursuit the player
                    else if ((distance > 3 && distance < 19) && Runing == false /*&& PursuitWatch.ElapsedMilliseconds >= 500*/)
                    {
                        //restrict the loop to call this branch so frequent
                        //PursuitWatch.Restart();

                        Runing = true;
                        GotoPlayer(sys.Character, distance);
                    }
                    else if (distance <= 3 && Runing == false)
                    {
                        Hit = true;
                        AttackHim(AttackType);
                    }
                    #endregion

                    if (Hit) // wait for the attack animation
                    {
                        Thread.Sleep(AttackTime);
                        Hit = false;
                    }
                    else
                    {
                        int SleepTime = 300;
                        //magic
                        /*if (Walking)
                        {
                            if (RecordedTime < SleepTime)
                            {
                                SleepTime = (int)RecordedTime-50;
                                if (RecordedTime < 50) SleepTime = 50;
                            }
                        }*/
                        Thread.Sleep(SleepTime);
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void FollowHim(PlayerMgr sys)
        {
            try
            {
                if (sys != null && LocalType == 1 && !sys.Character.State.Die/* && !sys.Character.Action.Check() */&& !sys.Character.Transport.Right)
                {
                    if (!Busy && Agresif == 1 && Attacking == false)
                    {
                        if (x >= (sys.Character.Position.x - 10) && x <= ((sys.Character.Position.x - 10) + 20) && y >= (sys.Character.Position.y - 10) && y <= ((sys.Character.Position.y - 10) + 20))
                        {
                            if (Runing)
                            {       //Notes / bugs :
                                    //- Monster skill attacks does not count amount of attacks for example if an attack strikes 2 times, onlye does 1 time damage. since its static. AttackHim()
                                    //- Player attack distance is really bad, needs timer to check monster location so attacking distance is more accurate. rly complex so navmesh here myb .. coz mob coordinate not apropriate need to check it
                                    //- Monsters agro needs max agro count, because having 100 on top of you is rediculous
                                    //- when player has to die (when mob attacks him) dont die at 0hp player needs to get one more hit to die :P

                                Runing = false;
                                StopMovement();

                                Target = sys;

                                if (AttackHandle != null)
                                    if (AttackHandle.IsAlive)
                                        return;

                                AttackHandle = new Thread(new ThreadStart(AttackMain));
                                AttackHandle.Start();

                                return;
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
        public void AttackHim(int AttackType)
        {
            try
            {
                if (!Runing && Attacking && !bSleep)
                {

                    PlayerMgr sys = (PlayerMgr)GetTarget();

                    if (sys == null || Die || GetDie)
                    {
                        AttackStop();
                        return;
                    }
                    if (sys != null && !Spawned(sys.Character.Information.UniqueID))
                    {
                        AttackStop();
                        return;
                    }

                    if (!sys.Character.InGame)
                    {
                        AttackStop();
                        return;
                    }

                    byte NumberAttack = 1;

                    int p_dmg = 0;
                    byte status = 0, crit = 1;

                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Client.OperationCode.SERVER_ACTION_DATA);
                    Writer.Byte(1);
                    Writer.Byte(2);
                    Writer.Byte(0x30);

                    Writer.DWord(AttackType);
                    Writer.DWord(UniqueID);

                    LastCasting = Ids.GetCastingID();

                    Writer.DWord(LastCasting);
                    Writer.DWord(sys.Character.Information.UniqueID);

                    Writer.Bool(true);
                    Writer.Byte(NumberAttack);
                    Writer.Byte(1);

                    Writer.DWord(sys.Character.Information.UniqueID);

                    for (byte n = 1; n <= NumberAttack; n++)
                    {
                        bool block = false;

                        if (sys.Character.Information.Item.sID != 0 && ObjData.Manager.ItemBase[sys.Character.Information.Item.sID].TypeID2 == 1)
                        {
                            if (Rnd.Next(25) < 10) block = true;
                        }
                        if (!block)
                        {
                            status = 0;
                            crit = 1;

                            p_dmg = (int)Formule.gamedamage(ObjData.Manager.SkillBase[AttackType].MaxAttack, 0, sys.Character.Stat.phy_Absorb, sys.Character.Stat.PhyDef, 50, ObjData.Manager.SkillBase[AttackType].MagPer);
                            p_dmg += Rnd.Next(0, p_dmg.ToString().Length);
                            if (p_dmg <= 0) p_dmg = 1;

                            if (Rnd.Next(20) > 15)
                            {
                                p_dmg *= 2;
                                crit = 2;
                            }

                            if (sys.Character.Stat.Absorb_mp > 0)
                            {
                                int static_dmg = (p_dmg * (100 - (int)sys.Character.Stat.Absorb_mp)) / 100;
                                sys.Character.Stat.SecondMP -= static_dmg;
                                if (sys.Character.Stat.SecondMP < 0) sys.Character.Stat.SecondMP = 0;
                                sys.UpdateMp();
                                p_dmg = static_dmg;
                            }

                            sys.Character.Stat.SecondHp -= p_dmg;

                            if (sys.Character.Stat.SecondHp <= 0)
                            {
                                sys.BuffAllClose();
                                status = 128;
                                sys.Character.Stat.SecondHp = 0;
                                sys.Character.State.Die = true;
                                sys.Character.State.DeadType = 1;

                                _agro agro = GetAgroClass(sys.Character.Information.UniqueID);
                                if (agro != null) AgressiveDMG.Remove(agro);
                                DeleteTarget();
                                AttackStop();
                                CheckAgro();

                                if (sys.Character.Action.nAttack) sys.StopAttackTimer();
                                else if (sys.Character.Action.sAttack || sys.Character.Action.sCasting) sys.StopSkillTimer();
                            }

                            sys.UpdateHp();

                            Writer.Byte(status);
                            Writer.Byte(crit);
                            Writer.DWord(p_dmg);
                            Writer.Byte(0);
                            Writer.Word(0);
                        }
                        else
                            Writer.Byte(2);
                    }
                    Send(Writer.GetBytes());
                    //Game.Effect.EffectMain(sys, AttackType);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
