using System;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public class Effect
    {
        public enum EffectNumbers
        { FROZEN = 1, FROSTBITE = 2, SHOCKED = 4, BURN = 8, POISIONING = 16, ZOMBIE = 32, SLEEP = 64, BLIND = 128, DULL = 256, FEAR = 512, SHORT_SIGHT = 1024, BLEED = 2048, DARKNESS = 8192, STUN = 16384, DISEASE = 32768, CONFUSION = 65536, DECAY = 131072, WEAKEN = 262144, IMPOTENT = 524288, DIVISION = 1048576, PANIC = 2097152, COMBUSTION = 4194304, HIDDEN = 16777216 } ;

        public enum StatusTypes
        { KNOCKDOWN = 1, KNOCKBACK = 2 } ;
        public static void EffectMain(object target, int skillid)
        {
            try
            {
                if (target != null)
                {
                    foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[skillid].Properties1)
                    {
                        switch (p.Key)
                        {
                            case "fb":
                                //Frostbite
                                GenerateEffect_fb(target, skillid);
                                break;
                            case "fz":
                                //Frozen
                                GenerateEffect_fz(target, skillid);
                                break;
                            case "bu":
                                //Burn
                                GenerateEffect_bu(target, skillid);
                                break;
                            case "es":
                                //Lightning
                                GenerateEffect_es(target, skillid);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Console.WriteLine("Player_Effect_CallBack() {0}",ex);
            }
        }
        
        #region Frost Bite
        public static void GenerateEffect_fb(object target, int skillid)
        {
            // Get effects parameters
            int Power = ObjData.Manager.SkillBase[skillid].Properties1["fb"];
            int Probability = ObjData.Manager.SkillBase[skillid].Properties2["fb"];

            int Random = Rnd.Next(1, 100);

            byte slot;


            // if effect succeeded
            if (Random <= Probability)
            {

                if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                {
                    PlayerMgr sys = target as PlayerMgr;

                    if (sys.Character.State.Die == true) return;

                    slot = sys.DeBuffGetFreeSlot();
                    sys.Character.Action.DeBuff.Effect.EffectID[slot] = EffectNumbers.FROSTBITE;
                    sys.Character.Action.DeBuff.Effect.SkillID[slot] = skillid;

                    sys.StartEffectTimer(Power * 100, slot);
                    sys.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, EffectNumbers.FROSTBITE, true));

                    // attack speed
                    //sys.Character.Speed.AttackSpeedModifier = 1.5;

                    sys.Character.Speed.RunSpeed /= 2;
                    sys.Character.Speed.WalkSpeed /= 2;
                    sys.Character.Speed.BerserkSpeed /= 2;

                    sys.Send(Packet.SetSpeed(sys.Character.Information.UniqueID, sys.Character.Speed.WalkSpeed, sys.Character.Speed.RunSpeed));
                }
                else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                {
                    WorldMgr.Monsters os = target as WorldMgr.Monsters;

                    if (os.Die == true) return;

                    slot = os.DeBuffGetFreeSlot();
                    os.DeBuff.Effect.EffectID[slot] = EffectNumbers.FROSTBITE;
                    os.DeBuff.Effect.SkillID[slot] = skillid;
                    //target.os.DeBuff.Effect.EffectPower[slot] = Power;

                    os.StartEffectTimer(Power * 100, slot);

                    os.Send(Packet.EffectUpdate(os.UniqueID, EffectNumbers.FROSTBITE, true));

                    // attack speed
                    os.AttackSpeed = 1.5;

                    // movement speed
                    os.RunningSpeed /= 2;
                    os.WalkingSpeed /= 2;

                    //Set our bool active
                    os.Frostbite = true;

                    os.Send(Packet.SetSpeed(os.UniqueID, os.WalkingSpeed, os.RunningSpeed));
                }
            }
        }
        #endregion
        #region Frozen
        public static void GenerateEffect_fz(object target, int skillid)
        {
            try
            {
                // Get effects parameters
                int Power = ObjData.Manager.SkillBase[skillid].Properties1["fz"];
                int Probability = ObjData.Manager.SkillBase[skillid].Properties2["fz"];

                int Random = Rnd.Next(1, 100);

                byte slot;

                // if effect succeeded
                if (Random <= Probability)
                {

                    if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                    {
                        PlayerMgr sys = target as PlayerMgr;

                        if (sys.Character.State.Die == true) return;

                        slot = sys.DeBuffGetFreeSlot();
                        sys.Character.Action.DeBuff.Effect.EffectID[slot] = EffectNumbers.FROZEN;
                        sys.Character.Action.DeBuff.Effect.SkillID[slot] = skillid;

                        sys.StartEffectTimer(Power * 100, slot);
                        sys.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, EffectNumbers.FROZEN, true));

                        // attack speed
                        //sys.Character.Speed.AttackSpeedModifier = 0;

                        sys.Character.Speed.RunSpeed = 0;
                        sys.Character.Speed.WalkSpeed = 0;
                        sys.Character.Speed.BerserkSpeed = 0;
                        sys.Character.State.Frostbite = true;
                        sys.Send(Packet.SetSpeed(sys.Character.Information.UniqueID, sys.Character.Speed.WalkSpeed, sys.Character.Speed.RunSpeed));
                    }
                    else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                    {
                        WorldMgr.Monsters os = target as WorldMgr.Monsters;

                        if (os.Die == true) return;

                        slot = os.DeBuffGetFreeSlot();
                        os.DeBuff.Effect.EffectID[slot] = EffectNumbers.FROZEN;
                        os.DeBuff.Effect.SkillID[slot] = skillid;
                        //target.os.DeBuff.Effect.EffectPower[slot] = Power;

                        os.StartEffectTimer(Power * 100, slot);

                        os.Send(Packet.EffectUpdate(os.UniqueID, EffectNumbers.FROZEN, true));

                        // attack speed
                        os.AttackSpeed = 0;

                        // movement speed
                        os.RunningSpeed = 0;
                        os.WalkingSpeed = 0;

                        os.Send(Packet.SetSpeed(os.UniqueID, os.WalkingSpeed, os.RunningSpeed));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        #endregion
        public static void DeleteEffect_fb(object target, byte EffectSlot)
        {
            try
            {
                if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                {
                    PlayerMgr sys = target as PlayerMgr;

                    sys.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, sys.Character.Action.DeBuff.Effect.EffectID[EffectSlot], false));

                    sys.Character.Action.DeBuff.Effect.EffectID[EffectSlot] = 0;
                    sys.Character.Action.DeBuff.Effect.SkillID[EffectSlot] = 0;

                    //sys.Character.Speed.AttackSpeedModifier = 1;

                    sys.Character.Speed.RunSpeed *= 2;
                    sys.Character.Speed.WalkSpeed *= 2;
                    sys.Character.Speed.BerserkSpeed *= 2;

                    sys.Send(Packet.SetSpeed(sys.Character.Information.UniqueID, sys.Character.Speed.WalkSpeed, sys.Character.Speed.RunSpeed));
                }
                else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                {
                    WorldMgr.Monsters os = target as WorldMgr.Monsters;

                    os.Send(Packet.EffectUpdate(os.UniqueID, os.DeBuff.Effect.EffectID[EffectSlot], false));

                    os.DeBuff.Effect.EffectID[EffectSlot] = 0;
                    os.DeBuff.Effect.SkillID[EffectSlot] = 0;

                    // attack speed
                    os.AttackSpeed = 1;

                    // movement speed
                    os.RunningSpeed *= 2;
                    os.WalkingSpeed *= 2;
                    //Set bool to false again
                    os.Frostbite = false;

                    os.Send(Packet.SetSpeed(os.UniqueID, os.WalkingSpeed, os.RunningSpeed));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static void DeleteEffect_fz(object target, byte EffectSlot)
        {
            try
            {
                if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                {
                    PlayerMgr sys = target as PlayerMgr;

                    sys.client.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, sys.Character.Action.DeBuff.Effect.EffectID[EffectSlot], false));

                    sys.Character.Action.DeBuff.Effect.EffectID[EffectSlot] = 0;
                    sys.Character.Action.DeBuff.Effect.SkillID[EffectSlot] = 0;

                    //sys.Character.Speed.AttackSpeedModifier = 1;

                    sys.Character.Speed.RunSpeed *= 2;
                    sys.Character.Speed.WalkSpeed *= 2;
                    sys.Character.Speed.BerserkSpeed *= 2;

                    sys.client.Send(Packet.SetSpeed(sys.Character.Information.UniqueID, sys.Character.Speed.WalkSpeed, sys.Character.Speed.RunSpeed));
                }
                else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                {
                    WorldMgr.Monsters os = target as WorldMgr.Monsters;

                    os.Send(Packet.EffectUpdate(os.UniqueID, os.DeBuff.Effect.EffectID[EffectSlot], false));

                    os.DeBuff.Effect.EffectID[EffectSlot] = 0;
                    os.DeBuff.Effect.SkillID[EffectSlot] = 0;

                    // attack speed
                    os.AttackSpeed = 1;

                    // movement speed
                    os.RunningSpeed *= 2;
                    os.WalkingSpeed *= 2;

                    os.Send(Packet.SetSpeed(os.UniqueID, os.WalkingSpeed, os.RunningSpeed));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public static void GenerateEffect_bu(object target, int skillid)
        {
            // Get effects parameters
            int Power = ObjData.Manager.SkillBase[skillid].Properties1["bu"];
            int Probability = ObjData.Manager.SkillBase[skillid].Properties2["bu"];

            int Random = Rnd.Next(1, 100);

            byte slot;

            // if effect succeeded
            if (Random <= Probability)
            {

                if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                {
                    PlayerMgr sys = target as PlayerMgr;

                    if (sys.Character.State.Die == true) return;

                    slot = sys.DeBuffGetFreeSlot();
                    sys.Character.Action.DeBuff.Effect.EffectID[slot] = EffectNumbers.BURN;
                    sys.Character.Action.DeBuff.Effect.SkillID[slot] = skillid;

                    sys.StartEffectTimer(Power * 100, slot);
                    sys.client.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, EffectNumbers.BURN, true));

                    // Timer for burn state

                }
                else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                {
                    WorldMgr.Monsters os = target as WorldMgr.Monsters;

                    if (os.Die == true) return;

                    slot = os.DeBuffGetFreeSlot();
                    os.DeBuff.Effect.EffectID[slot] = EffectNumbers.BURN;
                    os.DeBuff.Effect.SkillID[slot] = skillid;

                    os.StartEffectTimer(Power * 100, slot);
                    os.Send(Packet.EffectUpdate(os.UniqueID, EffectNumbers.BURN, true));

                    // Burn timer
                    // Add (character details).
                }
            }
        }
        /*
        public static void StartEffectDamage(int intval, int HitPower, byte e_index, object target)
        {
            try
            {
                if (target.sys.Character.Action.DeBuff.Effect.EffectImpactTimer[e_index] != null) target.sys.Character.Action.DeBuff.Effect.EffectImpactTimer[e_index].Dispose();

                target.sys.Character.Action.DeBuff.Effect.EffectImpactTimer[e_index] = new Timer(
                    new TimerCallback(
                        delegate(object e)
                        {
                            if (target.type == true) // player
                            {
                                if (!target.sys.Character.State.Die)
                                {
                                    target.HP(HitPower);
                                    target.sys.Send(Packet.Effects2Dmg(target.sys.Character.Information.UniqueID, HitPower));
                                }
                                else return;
                            }
                            else if (target.type == false) // mob
                            {
                                if (!target.os.Die)
                                {
                                    target.HP(HitPower);
                                    target.os.Send(Packet.Effects2Dmg(target.ID, HitPower));
                                }
                                else return;
                            }
                        }
                ), null, 0, intval);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Console.WriteLine("StartEffectDamage() Error {0}", ex);
            }
        }
        */
        ///////////////////////////////////////////////////////////////////////////
        // Shock state
        ///////////////////////////////////////////////////////////////////////////
        public static void GenerateEffect_es(object target, int skillid)
        {
            // Get effects parameters
            int Power = ObjData.Manager.SkillBase[skillid].Properties1["es"];
            int Probability = ObjData.Manager.SkillBase[skillid].Properties2["es"];

            int Random = Rnd.Next(1, 100);

            byte slot;

            // if effect succeeded
            if (Random <= Probability)
            {

                if (target.GetType().ToString() == "CLGameServer.PlayerMgr") // player
                {
                    PlayerMgr sys = target as PlayerMgr;

                    if (sys.Character.State.Die == true) return;

                    slot = sys.DeBuffGetFreeSlot();
                    sys.Character.Action.DeBuff.Effect.EffectID[slot] = EffectNumbers.SHOCKED;
                    sys.Character.Action.DeBuff.Effect.SkillID[slot] = skillid;

                    sys.StartEffectTimer(Power * 100, slot);
                    sys.client.Send(Packet.EffectUpdate(sys.Character.Information.UniqueID, EffectNumbers.SHOCKED, true));

                    // Timer for burn state

                }
                else if (target.GetType().ToString() == "CLGameServer.WorldMgr.Monsters") // mob
                {
                    WorldMgr.Monsters os = target as WorldMgr.Monsters;

                    if (os.Die == true) return;

                    slot = os.DeBuffGetFreeSlot();
                    os.DeBuff.Effect.EffectID[slot] = EffectNumbers.SHOCKED;
                    os.DeBuff.Effect.SkillID[slot] = skillid;

                    os.StartEffectTimer(Power * 100, slot);
                    os.Send(Packet.EffectUpdate(os.UniqueID, EffectNumbers.SHOCKED, true));

                    // Burn timer
                    // Add (character details).
                }
            }
        }
    }
}