using CLGameServer.Client;
using CLFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLGameServer.WorldMgr
{
    public partial class Monsters
    {
        Timer Time;
        Timer Movement;
        Timer AggresiveTimer;
        Timer Dead;
        Timer AutoRun;
        Timer objeSleep;
        Stopwatch SpawnWatch = new Stopwatch();
        public Timer[] EffectTimer = new Timer[20];

        #region Start / Stop functions
        public void StartObjeSleep(int time)
        {
            if (!bSleep)
            {
                bSleep = true;
                objeSleep = new Timer(new TimerCallback(ObjeSleepCallBack), 0, time, 0);
            }
        }
        public void StartRunTimer(int time)
        {
            //time *= 3;
            if (AutoRun != null) AutoRun.Dispose();
            AutoRun = new Timer(new TimerCallback(AutoRunCallBack), 0, 0, time);
        }
        public void StartAgressiveTimer(int time)
        {
            if (AggresiveTimer != null)
            {
                AggresiveTimer.Dispose();
                AggresiveTimer = null;
            }
            MonsterAgroCount += 1;
            AggresiveTimer = new Timer(new TimerCallback(CheckEveryOne), 0, time, 0);
        }
        public void Sleep(int time)
        {
            Busy = true;
            Time = new Timer(new TimerCallback(sleepcallback), 0, time, 0);
        }
        public void StartEffectTimer(int time, byte e_index)
        {
            if (EffectTimer[e_index] != null) EffectTimer[e_index].Dispose();
            EffectTimer[e_index] = new Timer(new TimerCallback(Mob_Effect_CallBack), e_index, time, 0);
        }
        public void StartDeadTimer(int time)
        {
            if (Dead != null) Dead.Dispose();
            Dead = new Timer(new TimerCallback(deadcallback), 0, time, 0);
        }
        public void StartMovement(int perTime)
        {
            Movement = new Timer(new TimerCallback(walkcallback), 0, 0, perTime);
        }
        public void StopMovement()
        {
            if (Movement != null)
            {
                Movement.Dispose();
                Movement = null;
            }
        }
        public void AttackStop()
        {
            try
            {
                Busy = false;
                Attacking = false;

                // mob start walking <-- have to parse this one
                //ChangeState(0, 3);

                //start auto walking
                if (AutoMovement && AutoRun == null) StartRunTimer(Rnd.Next(1, 5) * 1000);
                else if (Agresif == 1) StartAgressiveTimer(1000);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void StopAgressiveTimer()
        {
            if (AggresiveTimer != null)
            {
                AggresiveTimer.Dispose();
                AggresiveTimer = null;
            }
            MonsterAgroCount -= 1;
        }
        public void StopEffectTimer(byte e_index)
        {
            if (EffectTimer[e_index] != null)
            {
                EffectTimer[e_index].Dispose();
                EffectTimer[e_index] = null;
                Send(Packet.EffectUpdate(UniqueID, DeBuff.Effect.EffectID[e_index], false));
                DeBuff.Effect.EffectID[e_index] = 0;
            }
            //damage timer again
            if (DeBuff.Effect.EffectImpactTimer[e_index] != null)
            {
                DeBuff.Effect.EffectImpactTimer[e_index].Dispose();
                DeBuff.Effect.EffectImpactTimer[e_index] = null;
            }
        }
        public void StopAutoRunTimer()
        {
            if (AutoRun != null)
                AutoRun.Dispose();
            WalkingTime = 0;
            Runing = false;
            wx = 0;
            wy = 0;
        }
        #endregion
        #region Call Backs
        public void ObjeSleepCallBack(object e)
        {
            try
            {
                State = 3;
                bSleep = false;

                Send(Packet.Movement(new ObjData.vektor(UniqueID,
                (float)Formule.packetx((float)x, xSec),
                (float)z,
                (float)Formule.packety((float)y, ySec),
                xSec,
                ySec)));
                objeSleep.Dispose();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void deadcallback(object e)
        {
            try
            {
                Die = true;
                DeSpawnMe();
                Dead.Dispose();
                Sleep(5000);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void walkcallback(object e)
        {
            try
            {
                if (this != null)
                {
                    if (RecordedTime <= 0)
                    {
                        Runing = false;
                        //if (Attacking) AttackHim();
                        StopMovement();
                        if (!Attacking)
                        {
                            StartRunTimer(Rnd.Next(1, 5) * 1000);
                        }
                    }
                    else
                    {
                        // only 40% left from the movement and attacking
                        if (RecordedTime <= WalkingTime * 0.50 && Attacking && AgressiveDMG != null)
                        {
                            PlayerMgr sys = (PlayerMgr)GetTarget();
                            double distance = Formule.gamedistance((float)x, (float)y, sys.Character.Position.x, sys.Character.Position.y);
                            if (distance > 3 && distance < 19)
                            {
                                StopMovement();
                                GotoPlayer(sys.Character, distance);
                            }
                        }

                        x += wx * 0.1f;
                        y += wy * 0.1f;
                        RecordedTime -= (WalkingTime * 0.1);
                    }
                    //CheckEveryOne();
                    if (SpawnWatch.ElapsedMilliseconds >= 1000)
                    {
                        //Console.Write("StopWatch şuan {0} ms", SpawnWatch.ElapsedMilliseconds);
                        CheckEveryOne();
                        SpawnWatch.Restart();
                    }
                }
            }
            catch (Exception ex)
            {

                Log.Exception(ex);
            }
        }
        void sleepcallback(object e)
        {
            try
            {
                Time.Dispose();
                if (AutoSpawn) reSpawn();
                else
                {
                    /*if (Spawn.Count != 0)
                        GlobalUnique.ClearObject(this);*/

                    if (LastCasting != 0)
                        Dispose();

                    Helpers.Manager.Objects.Remove(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void Mob_Effect_CallBack(object e)
        {
            try
            {
                StopEffectTimer((byte)e);

                foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[DeBuff.Effect.SkillID[(byte)e]].Properties1)
                {
                    switch (p.Key)
                    {
                        case "fb":
                            CLGameServer.Effect.DeleteEffect_fb(this, (byte)e);
                            break;
                        case "bu":
                            //GenerateEffect_bu(target, EffectNumbers.BURN, ObjData.Manager.SkillBase[skillid].Properties1["bu"], ObjData.Manager.SkillBase[skillid].Properties2["bu"], ObjData.Manager.SkillBase[skillid].Properties3["bu"]);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void AutoRunCallBack(object e)
        {
            float NewX = x, NewY = y;
            double Distance = 0.0;
            try
            {
                if (RecordedTime <= 0 && AutoMovement && !Die)
                {
                    double MaxDistance = 0.0;
                    if (RunningSpeed <= 0 && BerserkerSpeed <= 0)
                    {
                        StopMovement();
                        StopAutoRunTimer();
                        return;
                    }
                    if (UniqueGuardMode)
                    {
                        MaxDistance = 15;
                    }
                    else
                    {
                        MaxDistance = 100;
                    }
                    double OrjDistance = Formule.gamedistance((float)x, (float)y, (float)OriginalX, (float)OriginalY);

                    if (OrjDistance > MaxDistance)
                    {
                        wx = OriginalX - x;
                        wy = OriginalY - y;
                        xSec = (byte)((OriginalX / 192) + 135);
                        ySec = (byte)((OriginalY / 192) + 92);
                        if (LastState == 1 && LastState2 == 2)
                        {
                            ChangeState(1, 3);
                            Send(Packet.SetSpeed(UniqueID, WalkingSpeed, RunningSpeed));
                        }
                        Runing = true;
                        WalkingTime = (OrjDistance / (WalkingSpeed * 0.0768)) * 1000.0;
                        RecordedTime = WalkingTime;
                        StartMovement((int)(WalkingTime * 0.0768));
                        Send(Packet.Movement(new ObjData.vektor(UniqueID,
                                                        Formule.packetx((float)OriginalX, xSec),
                                                        (float)z,
                                                        Formule.packety((float)OriginalY, ySec),
                                                        xSec,
                                                        ySec)));
                    }
                    else
                    {
                        Helpers.Functions.aRound(ref NewX, ref NewY, 2);
                        wx = NewX - x;
                        wy = NewY - y;
                        xSec = (byte)((NewX / 192) + 135);
                        ySec = (byte)((NewY / 192) + 92);
                        Runing = true;
                        Distance = Formule.gamedistance((float)x, (float)y, (float)NewX, (float)NewY);
                        if (Runing == true || WalkingSpeed != 0)
                        {
                            if (LastState == 1 && LastState2 == 3)
                            {
                                ChangeState(1, 2);
                                Send(Packet.SetSpeed(UniqueID, WalkingSpeed, RunningSpeed));
                            }
                            Runing = false;
                            WalkingTime = (Distance / (WalkingSpeed * 0.0768)) * 1000.0;
                            RecordedTime = WalkingTime;
                        }
                        else if (RunningSpeed != 0 && WalkingSpeed == 0)
                        {
                            if (LastState == 1 && LastState2 == 2)
                            {
                                ChangeState(1, 3);
                                Send(Packet.SetSpeed(UniqueID, WalkingSpeed, RunningSpeed));
                            }
                            Runing = true;
                            WalkingTime = (Distance / (RunningSpeed * 0.0768)) * 1000.0;
                            RecordedTime = WalkingTime;
                        }
                        if (!FileDB.CheckCave(xSec, ySec))
                        {
                            Send(Packet.Movement(new ObjData.vektor(UniqueID,
                                                    Formule.packetx((float)NewX, xSec),
                                                (float)z,
                                                    Formule.packety((float)NewY, ySec),
                                                xSec,
                                                ySec)));
                        }
                        else
                        {
                            Send(Packet.Movement(new ObjData.vektor(UniqueID,
                                                    Formule.cavepacketx((float)NewX),
                                                (float)z,
                                                    Formule.cavepackety((float)NewY),
                                                xSec,
                                                ySec)));
                        }
                        CheckEveryOne();
                        StartMovement((int)(WalkingTime * 0.0768));

                    }
                }
                else
                {
                    return;
                }

            }
            catch
            {
                StopMovement();
                StopAgressiveTimer();
            }
        }
        #endregion
    }
}
