using System;
using System.Collections.Generic;
using System.Linq;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        #region Skill Main

        public void ActionMain()
        {
            try
            {
                if (Character.State.Die) return;

                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte type = Reader.Byte();
                if (type != 2)
                {
                    if (Character.Action.Cast) return;
                    if (Character.Information.Scroll) return;

                    switch (Reader.Byte())
                    {
                        case 1: // normal attack
                            if (Character.Action.nAttack) return;
                            if (Character.Action.sAttack) return;
                            if (Character.Transport.Right) return;
                            Reader.Byte();
                            int id = Reader.Int32();
                            Reader.Close();
                            object os = Helpers.GetInformation.GetObjects(id);
                            if (/*(Character.Action.Target == id && Timer.Attack != null)  || */ id == Character.Information.UniqueID)
                            {
                                Reader.Close();
                                return;
                            }
                            Character.Action.PickUping = false;
                            Character.Action.Object = os;
                            Character.Action.nAttack = true;
                            Character.Action.Target = id;
                            StartAttackTimer();
                            break;
                        case 2://pickup
                            if (Character.Action.nAttack) return;
                            if (Character.Action.sAttack) return;
                            if (Character.Action.sCasting) return;
                            if (Character.Action.PickUping) return;

                            Reader.Byte();
                            int id2 = Reader.Int32();
                            Reader.Close();

                            Character.Action.Target = id2;
                            Character.Action.PickUping = true;
                            StartPickupTimer(1000);
                            break;
                        case 3://trace
                            if (Character.Action.sAttack) return;
                            if (Character.Action.sCasting) return;
                            if (Character.State.Sitting) return;
                            if (Character.Stall.Stallactive) return;

                            Reader.Byte();
                            int id3 = Reader.Int32();
                            Character.Action.Target = id3;
                            Reader.Close();
                            client.Send(Packet.ActionState(1, 1));
                            Character.Action.PickUping = false;
                            Player_Trace(id3);
                            break;
                        case 4://use skill
                            if (Character.Action.nAttack)
                            {
                                StopAttackTimer();
                                System.Threading.Thread.Sleep(300);
                            }
                            Character.Action.UsingSkillID = Reader.Int32();
                            SkillMain(Reader.Byte(), Reader);
                            break;
                        case 5:
                            int id4 = Reader.Int32();
                            byte b_index = SkillGetBuffIndex(id4);
                            SkillBuffEnd(b_index);
                            break;
                        default:
                            Console.WriteLine("ActionMain case: " + Reader.Byte());
                            break;
                    }
                }

                else
                    StopAttackTimer();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void Player_Trace(int targetid)
        {
            try
            {
                if (Character.Action.Target != 0)
                {
                    WorldMgr.Monsters monster =Helpers.GetInformation.GetObject(Character.Action.Target);
                    if (monster == null) return;
                    double distance = Formule.gamedistance(Character.Position.x, Character.Position.y, (float)monster.x, (float)monster.y);

                    if (distance >= 2)
                    {
                        Character.Position.wX =monster.x - Character.Position.x;// -ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.wY = monster.y - Character.Position.y;// -ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        if (!Character.InGame) return;
                        Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID,
                                    Formule.packetx(monster.x, monster.xSec),
                                    Character.Position.z,
                                    Formule.packety(monster.y, monster.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));

                        StartMovementTimer(GetMovementTime(distance));
                        return;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void SkillMain(byte type, PacketReader Reader)
        {
            try
            {
                if (!SkillGetOpened(Character.Action.UsingSkillID)) return;
                client.Send(Packet.ActionState(1, 1));
                switch (type)
                {
                    case 1:
                        if (Character.Action.sAttack) return;
                        if (Character.Action.sCasting) return;
                        if (Character.Action.nAttack) StopAttackTimer();

                        if (!Base.Skill.CheckWeapon(Character.Information.Item.wID, Character.Action.UsingSkillID))
                        {
                            client.Send(Packet.Message(OperationCode.SERVER_ACTION_DATA, Messages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                            client.Send(Packet.Messages2(OperationCode.SERVER_ACTIONSTATE, Messages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                            return;
                        }

                        Character.Action.Target = Reader.Int32();
                        Character.Action.Skill.MainSkill = Character.Action.UsingSkillID;
                        Character.Action.UsingSkillID = 0;
                        Character.Action.Object = Helpers.GetInformation.GetObjects(Character.Action.Target);

                        if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].isAttackSkill)
                        {
                            Character.Action.Skill = Base.Skill.Info(Character.Action.Skill.MainSkill, Character);
                            if (!Character.Action.Skill.canUse || Character.Action.Target == Character.Information.UniqueID) return;

                            WorldMgr.Monsters o = null;
                            if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "WorldMgr.Monsters")
                            {
                                o = Character.Action.Object as WorldMgr.Monsters;
                                //if (o.AgressiveDMG == null) o.AgressiveDMG = new List<_agro>();
                                //if (Character.Action.Skill.OzelEffect == 5 && o.State != 4) return;
                                //if (o.State == 4 && Character.Action.Skill.OzelEffect != 5) return;
                            }
                            
                            if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr")
                            {
                                if (!Character.Information.PvP || Character.State.Die) return;
                                PlayerMgr sys = Character.Action.Object as PlayerMgr;
                                //if (Character.Action.Skill.OzelEffect == 5 && sys.Character.State.LastState != 5) return;
                                //if (sys.Character.State.LastState == 4 && Character.Action.Skill.OzelEffect != 5) return;
                            }

                            Character.Action.sAttack = true;
                            ActionSkillAttack();

                            Reader.Close();
                        }
                        else
                        {
                            Character.Action.sAttack = true;
                            ActionSkill();
                        }
                        break;
                    case 0:
                        SkillBuff();
                        break;
                    case 2:
                        MovementSkill(Reader);
                        break;
                    default:
                        Console.WriteLine("Skillmain type: {0}", type);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        #endregion

        #region Buff

        void SkillBuff()
        {
            try
            {
                if (SkillGetSameBuff(Character.Action.UsingSkillID))
                    return;

                /*
                    ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste")
                    Bak buff kullanırken aynı özelliklere sahip buff varmı yokmu kontrol et misal hste speed artırma varsa tekrar çalıştırma hata ver
                    kontrölün ekleneceği fonksiyon : SkillGetSameBuff
                */

                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name.Contains("SKILL_OP") && Character.Information.GM == 0) return;
                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name.Contains("TRADE")) return;
                
                if (!Base.Skill.CheckWeapon(Character.Information.Item.wID, Character.Action.UsingSkillID))
                {
                    client.Send(Packet.ActionPacket(2, 0x0D));
                    client.Send(Packet.Messages2(OperationCode.SERVER_ACTIONSTATE, Messages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                    return;
                }
                
                if (Character.Action.ImbueID != 0 && 
                    ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SkillType == ObjData.s_data.SkillTypes.IMBUE &&
                    !ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste")) 
                    return;

                if (Character.Action.Buff.count > 21) return;
                byte slot = SkillBuffGetFreeSlot();
                if (slot == 255) return;
                if (Character.Stat.SecondMP < ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Mana) { client.Send(Packet.ActionPacket(2, 4)); Character.Action.Cast = false; return; }
                else
                {
                    Character.Stat.SecondMP -= ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Mana;

                    if (Character.Stat.SecondMP < 0) Character.Stat.SecondMP = 1;
                    UpdateMp();

                    Character.Action.CastingSkill = Character.Ids.GetCastingID();
                    
                    if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType == ObjData.s_data.RadiusTypes.ONETARGET)
                    {
                        Character.Action.Buff.SkillID[slot] = Character.Action.UsingSkillID;
                        Character.Action.Buff.OverID[slot] = Character.Ids.GetBuffID();

                        Character.Action.Buff.slot = slot;
                        Character.Action.Buff.count++;
                        Console.WriteLine("###############################################################################");
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1)
                        {
                            Console.WriteLine($"Properties1 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2)
                        {
                            Console.WriteLine($"Properties2 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties3)
                        {
                            Console.WriteLine($"Properties3 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties4)
                        {
                            Console.WriteLine($"Properties4 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties5)
                        {
                            Console.WriteLine($"Properties5 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties6)
                        {
                            Console.WriteLine($"Properties6 =| SkillName = {ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Name} Key = {p.Key} Value = {p.Value}");
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                        
                    }

                    List<int> lis = Character.Spawn;
                    Send(lis,Packet.ActionPacket(1, 0, Character.Action.UsingSkillID, Character.Information.UniqueID, Character.Action.CastingSkill, 0));

                    Character.Action.Cast = true;
                    Effect.EffectMain(Character.Action.Object, Character.Action.UsingSkillID);
                    StartCastingTimer(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].CastingTime, lis);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void SpecialBuff(int skillID)
        {
            try
            {
                Character.Action.UsingSkillID = skillID;
                if (SkillGetSameBuff(Character.Action.UsingSkillID)) return;
                if (Character.Action.Buff.count > 21) return;
                byte slot = SkillBuffGetFreeSlot();
                if (slot == 255) return;

                Character.Action.Buff.SkillID[slot] = Character.Action.UsingSkillID;
                Character.Action.Buff.OverID[slot] = Character.Ids.GetBuffID();

                Character.Action.Buff.slot = slot;
                Character.Action.Buff.count++;
                List<int> lis = Character.Spawn;

                //add properties
                foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1)
                {
                    if (SkillAdd_Properties(this, p.Key, true, slot)) 
                    { 
                        return; 
                    };
                }

                // if imbue add to current imbue
                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SkillType == ObjData.s_data.SkillTypes.IMBUE &&
                    !ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste"))
                {
                    if (Character.Action.ImbueID != 0) return;
                    Character.Action.ImbueID = Character.Action.UsingSkillID;
                }

                Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") &&
                    (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType == ObjData.s_data.RadiusTypes.ONETARGET || (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType != ObjData.s_data.RadiusTypes.ONETARGET && ObjData.Manager.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3)))
                {
                    // Skills with duration and self targeted
                    //Character.Action.CastingSkill = Character.Ids.GetCastingID();

                    //Send(lis, Packet.ActionPacket(1, 0, Character.Action.UsingSkillID, Character.Information.UniqueID, Character.Action.CastingSkill, 0));

                    StartBuffTimer(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["dura"], Character.Action.Buff.slot);
                }
                else if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("onff"))
                {
                    // mana consume/time buffs 
                    //Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                    string series = ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Length - 2);
                    Character.Action.Buff.InfiniteBuffs.Add(series, Character.Action.Buff.slot);
                }
                else if (!ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura"))
                {
                    // fast buffs
                    //Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                    SkillBuffEnd(Character.Action.Buff.slot);
                }

                Character.Action.Buff.slot = 255;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void SkillBuffCasting(List<int> list)
        {
            try
            {
                Character.Action.Buff.Casting = true;

                // add buff's special attributes (example: harmony terapy spawn packet etc)
                HandleSpecialBuff(Character.Action.UsingSkillID);

                Send(list, Packet.SkillPacket(0, Character.Action.CastingSkill));

                // every skill is handled as surround range radius
                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType == ObjData.s_data.RadiusTypes.ONETARGET || ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SkillType == ObjData.s_data.SkillTypes.IMBUE)
                {
                    SpecialBuff(Character.Action.UsingSkillID);
                }
                else if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType != ObjData.s_data.RadiusTypes.ONETARGET && ObjData.Manager.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3) // i use it as efr not handled here :) (example: harmony)
                {
                    byte currentSimult = 0;
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i <= Helpers.Manager.clients.Count - 1; i++)
                        {
                            try
                            {
                                PlayerMgr s = Helpers.Manager.clients[i];

                                //double distance = Formule.gamedistance((float)Character.Position.x, (float)Character.Position.y, s.Character.Position.x, s.Character.Position.y);
                                double distance = Formule.gamedistance(Character.Position, s.Character.Position);
                                if (distance <= ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Distance / 10)
                                {
                                    if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SimultAttack == 0 || currentSimult < ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SimultAttack)
                                    {
                                        // todo: here handle inv and steal detect // only add "buff" to char on pvp state (dttp)
                                        s.SpecialBuff(Character.Action.UsingSkillID);
                                        currentSimult++;
                                    }
                                    else break;
                                }
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine("SkillBuffCast Error on index {1}/{2}: {0}", ex, i, Helpers.Manager.clients.Count);
                                Log.Exception(ex);
                            }
                        }
                    }
                }

                if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].SkillType == ObjData.s_data.SkillTypes.IMBUE ||
                   (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") &&
                   (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType == ObjData.s_data.RadiusTypes.ONETARGET || (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].RadiusType != ObjData.s_data.RadiusTypes.ONETARGET && ObjData.Manager.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3))))
                {
                    //Skills with duration and self targeted
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot],false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));
                    StartBuffTimer(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["dura"], Character.Action.Buff.slot);

                }
                else if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("onff"))
                {
                    //area buffs / mana consumer - time buffs 
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));

                    string series = ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Length - 2);
                    Character.Action.Buff.InfiniteBuffs.Add(series, Character.Action.Buff.slot);
                }
                else if ( !ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") )
                {
                    // fast buffs
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));

                    SkillBuffEnd(Character.Action.Buff.slot);

                }
                Character.Action.Buff.Casting = false;
                Character.Action.Buff.slot = 255;
               
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void SkillBuffEnd(byte b)
        {
            try
            {
                foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b]].Properties1)
                {
                    SkillDelete_Properties(this, p.Key, true, b);
                }

                // if imbue delete the current imbue
                if (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b]].SkillType == ObjData.s_data.SkillTypes.IMBUE)
                {
                    Character.Action.ImbueID = 0;
                }

                if (Timer.Buff[b] != null)
                {
                    Timer.Buff[b].Dispose();
                    Timer.Buff[b] = null;
                }
                else
                {
                    foreach (var pair in Character.Action.Buff.InfiniteBuffs)
                    {
                        if (pair.Value == b)
                        {
                            Character.Action.Buff.InfiniteBuffs.Remove(pair.Key);
                            break;
                        }
                    }
                }

                Send(Packet.SkillEndBuffPacket(Character.Action.Buff.OverID[b]));
                GenerateUniqueID.Delete(Character.Action.Buff.OverID[b]);
                Character.Action.Buff.OverID[b] = 0;
                Character.Action.Buff.SkillID[b] = 0;
                Character.Action.Buff.count--;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public byte SkillBuffGetFreeSlot()
        {
            for (byte b = 0; b <= Character.Action.Buff.SkillID.Length - 1; b++)
                if (Character.Action.Buff.SkillID[b] == 0) return b;
            return 255;
        }
        public byte DeBuffGetFreeSlot()
        {
            for (byte b = 0; b <= Character.Action.DeBuff.Effect.EffectID.Length - 1; b++)
                if (Character.Action.DeBuff.Effect.EffectID[b] == 0) return b;
            return 255;
        }
        public bool SkillGetSameBuff(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Character.Action.Buff.SkillID[b] != 0)
                {
                    if (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b]].Series.Remove(ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b]].Series.Length - 2)
                        == ObjData.Manager.SkillBase[SkillID].Series.Remove(ObjData.Manager.SkillBase[SkillID].Series.Length - 2))
                    {
                        
                        return true;
                    }
                }
            }
            client.Send(Packet.Message(OperationCode.SERVER_ACTION_DATA, Messages.UIIT_MSG_SKILL_IS_ALREADY_USE));
                        // Skill Already Used
            return false;
        }
        public bool SkillGetOpened(int SkillID)
        {
            for (int b = 0; b <= Character.Stat.Skill.AmountSkill; b++)
            {
                if (Character.Stat.Skill.Skill[b] != 0 && Character.Stat.Skill.Skill[b] == SkillID) return true;
            }
            return false;
        }
        byte SkillGetBuffIndex(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Character.Action.Buff.SkillID[b] != 0 && Character.Action.Buff.SkillID[b] == SkillID)
                {
                    return b;
                }
            }
            return 255;
        }
        public void BuffAllClose()
        {
            try
            {
                if (Character != null)
                {
                    //Todo: Add item buff type "CBUF" check here, item buff remains after teleport etc
                    for (byte b = 0; b < Character.Action.Buff.SkillID.Length; b++)
                        if (Character.Action.Buff.SkillID[b] != 0) SkillBuffEnd(b);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        bool BuffAdd() // todo: need to clean up
        {
            try
            {
                
                //Console.WriteLine("BuffAdd Case: " + ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Length - 2));
                switch (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Length - 2))
                {
                    
                    case "SKILL_CH_COLD_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_FIRE_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        Console.WriteLine($"SkillINC:{ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["hste"]}");
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.UpdatededMagAttack += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["dru"]; //* (Character.Stat.MaxMagAttack / 100);
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.Parry += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["er"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.UpdatededPhyAttack += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["dru"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.MagDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                        Character.Stat.uMagDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.PhyDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        Character.Stat.uPhyDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.Absorb_mp = ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["dgmp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":

                        //if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1[""] == 0)
                        //{
                            Character.Stat.MagDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                            Character.Stat.uMagDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}

                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        //if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties4 != 7)
                        //{
                        Character.Stat.PhyDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        Character.Stat.uPhyDef += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        
                        break;
                    case "SKILL_CH_BOW_CALL":
                        StopAttackTimer();
                        //if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties4["summ"] == 0)
                            //Character.Stat.AttackPower += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        //Character.Stat.EkstraMetre += (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1);
                        break;
                    case "SKILL_CH_WATER_SELFHEAL":
                        Character.Stat.SecondHp += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["heal"];
                        UpdateHp();
                        break;
                    case "SKILL_CH_WATER_HEAL":
                        PlayerMgr s = Helpers.GetInformation.GetPlayer(Character.Action.Target);
                        
                        if (s.Character.Stat.SecondHp + ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Time < s.Character.Stat.Hp)
                            s.Character.Stat.SecondHp += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Time;
                        else
                            s.Character.Stat.SecondHp += s.Character.Stat.Hp - s.Character.Stat.SecondHp;

                        Character.Action.Buff.castingtime = (short)ObjData.Manager.SkillBase[Character.Action.UsingSkillID].CastingTime;
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED":
                    case "SKILL_EU_BARD_SPEEDUPA_MSPEED":
                        Console.WriteLine($"SkillINC:{ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["hste"]}");
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        Character.Action.Buff.castingtime = 3000;
                        break;
                    case "SKILL_EU_BARD_BATTLAA_GUARD":

                        break;
                        //////////////////////////////////////////////////////////////
                        // Eu buffs
                        //////////////////////////////////////////////////////////////

                    case "SKILL_EU_BARD_SPEEDUPA_HITRATE":
                        Character.Stat.Hit += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["hr"];
                        break;
                    //default:
                        //Console.WriteLine("Non coded skill case: " + ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Series.Length - 2));
                        //Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                    //    break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        void BuffDelete(byte b_index)
        {
            try
            {
                switch (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Series.Remove(ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Series.Length - 2))
                {
                    case "SKILL_CH_COLD_GIGONGTA":
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                    case "SKILL_CH_FIRE_GIGONGTA":
                        Character.Action.ImbueID = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        //Character.Speed.Updateded -= ObjData.Manager.SkillBase[Character.Speed.DefaultSpeedSkill].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        Character.Stat.UpdatededMagAttack -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["dru"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        Character.Stat.Parry -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["er"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        Character.Stat.UpdatededPhyAttack = 0;
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        Character.Stat.MagDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                        Character.Stat.uMagDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        Character.Stat.PhyDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                        Character.Stat.uPhyDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        Character.Stat.Absorb_mp = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":
                        //if (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1 == 0)
                        //{
                            Character.Stat.MagDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                            Character.Stat.uMagDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        //if (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties4 != 7)
                        //{
                            Character.Stat.PhyDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                            Character.Stat.uPhyDef -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        
                        break;
                    case "SKILL_CH_BOW_CALL":
                        //if (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties3 == 0) Character.Stat.AttackPower -= ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        //Character.Stat.EkstraMetre -= (ObjData.Manager.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1);
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED_":
                    //////////////////////////////////////////////////////////////
                    // Eu buffs
                    //////////////////////////////////////////////////////////////
                    case "SKILL_EU_BARD_SPEEDUPA_MSPEED":
                        //Character.Speed.Updateded -= ObjData.Manager.SkillBase[Character.Speed.DefaultSpeedSkill].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        break;
                    case "SKILL_EU_BARD_BATTLAA_GUARD":

                        break;
                    case "SKILL_EU_BARD_SPEEDUPA_HITRATE":
                        Character.Stat.Hit += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["hr"];
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
 
        #endregion

        #region Regular Attack

        void ActionNormalAttack()
        {
            try
            {
                float x = 0, y = 0;
                bool[] aRound = null;
                if (Character.Action.sAttack)
                    return;
                if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.WorldMgr.Monsters")
                {
                    WorldMgr.Monsters o = Character.Action.Object as WorldMgr.Monsters;
                    if (o.State == 4 || o.LocalType != 1)
                    {
                        Character.Action.nAttack = false;
                        StopAttackTimer();
                    }
                    /*if (o.AgressiveDMG == null)
                        o.AgressiveDMG = new List<_agro>();*/
                    x = o.x;
                    y = o.y;
                    aRound = o.aRound;
                    if (o.Die || o.GetDie)
                    {
                        StopAttackTimer();
                    }
                    //if (o.State.SafeState)
                    //{
                    //    Character.Action.nAttack = false;
                    //    StopAttackTimer();
                    //    client.Send(Packet.SafeState_SkillUse_Fail());
                    //    return;
                    //}
                }
				
                else if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr")
                {
                    if (!Character.Information.PvP)
                    {
                        Character.Action.nAttack = false;
                        StopAttackTimer();
                        return;
                    }
                    PlayerMgr sys = Character.Action.Object as PlayerMgr;
                    if (sys.Character.State.LastState == 4)
                    {
                        StopAttackTimer();
                        return;
                    }
                    if (sys.Character.State.SafeState || Character.State.SafeState)
                    {
                        Character.Action.nAttack = false;
                        StopAttackTimer();
                        client.Send(Packet.SafeState_SkillUse_Fail());
                        return;
                    }
                    if (!(Character.Information.PvP && sys.Character.Information.PvP)) 
                    { 
                        StopAttackTimer(); 
                        return; 
                    }
                    if (!Character.InGame)
                    {
                        StopAttackTimer();
                        return;
                    } 
                    x = sys.Character.Position.x;
                    y = sys.Character.Position.y;
                    aRound = sys.Character.aRound;
                }
                else
                {
                    // Invalid Object
                    client.Send(Packet.Messages2(OperationCode.SERVER_ACTIONSTATE, Messages.UIIT_STT_ERR_COMMON_INVALID_TARGET));
                    StopSkillTimer();
                    StopAttackTimer();
                    return;
                }

                if (CheckBow())
                {
                    StopSkillTimer();
                    StopAttackTimer();
                    return;
                }
                if (x == 0 && y == 0 && aRound == null)
                {  
                    StopAttackTimer();
                    StopSkillTimer();
                    return; 
                }

                double distance = Formule.gamedistance(Character.Position.x,
                        Character.Position.y,
                        x,
                        y);

                if (Character.Information.Item.wID != 0)
                    distance -= ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE + Character.Stat.EkstraMetre;

                if (distance > 1.5)
                {
                    float farkx = x;
                    float farky = y;

                    if (Character.Information.Item.wID == 0)
                    {
                        Character.Position.wX = farkx - Character.Position.x - 0;
                        Character.Position.wY = farky - Character.Position.y - 0;
                        Character.Position.kX = Character.Position.wX;
                        Character.Position.kY = Character.Position.wY;
                    }
                    else
                    {
                        Character.Position.wX = farkx - Character.Position.x - (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE - (float)Character.Stat.EkstraMetre;
                        Character.Position.wY = farky - Character.Position.y - (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE - (float)Character.Stat.EkstraMetre;
                        Character.Position.kX = Character.Position.wX;
                        Character.Position.kY = Character.Position.wY;
                    }
                    Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID,
                                Formule.packetx(farkx, Character.Position.xSec),
                                Character.Position.z,
                                Formule.packety(farky, Character.Position.ySec),
                                Character.Position.xSec,
                                Character.Position.ySec)));

                    

                    Character.Position.packetxSec = Character.Position.xSec;
                    Character.Position.packetySec = Character.Position.ySec;

                    Character.Position.packetX = (ushort)Formule.packetx((float)farkx, Character.Position.xSec);
                    Character.Position.packetY = (ushort)Formule.packety((float)farky, Character.Position.ySec);

                    Character.Position.Walking = true;

                    StartMovementTimer(GetMovementTime(distance));

                    return;
                }

                ActionAttack();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        bool CheckBow()
        {
            if (!Character.Action.Skill.P_M)
            {
                if (ObjData.Manager.ItemBase[Character.Information.Item.wID].TypeID4 == 6)
                {
                    if (Character.Information.Item.sID == 0) //arrow yoksa slota
                    {
                        if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                        {
                            Character.Action.sAttack = false; 
                            Character.Action.nAttack = false;
                            client.Send(Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                            StopAttackTimer();StopSkillTimer();
                            return true;
                        }
                    }
                    else  //arrow varsa slotta
                    {
                        Character.Information.Item.sAmount--;
                        client.Send(Packet.Arrow(Character.Information.Item.sAmount));

                        if (Character.Information.Item.sAmount <= 0) // arrow bitti
                        {
                            Character.Information.Item.sID = 0;
                            DB.query("delete from char_items where itemnumber='item" + 7 + "' AND owner='" + Character.Information.CharacterID + "'");
                            if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                            {
                                Character.Action.sAttack = false;
                                Character.Action.nAttack = false;
                                client.Send(Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                StopAttackTimer(); StopSkillTimer();
                                return true;
                            }
                        }
                        else
                        {
                            DB.query("UPDATE char_items SET quantity='" + Math.Abs(Character.Information.Item.sAmount) + "' WHERE itemnumber='" + "item" + 7 + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + Character.Information.Item.sID + "'");
                        }

                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        void ActionAttack()
        {
            try
            {
                //Predefined info needs work!
                #region Predefined info
                byte NumberAttack = 2;
                int AttackType = 1;
                int[] found = new int[3];
                byte numbert = 1;
                int p_dmg = 0;
                byte status = 0, crit = 1;
                #endregion
                client.Send(Packet.ActionState(1, 1));
                //Bow information
                if (CheckBow())
                {
                    StopSkillTimer();
                    StopAttackTimer();
                    return;
                }

                WorldMgr.targetObject target = new WorldMgr.targetObject(Character.Action.Object, this);

                if (Character.Action.ImbueID != 0 && ObjData.Manager.SkillBase[Character.Action.ImbueID].Series.Remove(ObjData.Manager.SkillBase[Character.Action.ImbueID].Series.Length - 2) == "SKILL_CH_LIGHTNING_GIGONGTA")
                {
                    numbert = ActionGetObject(ref found, 2, target.x, target.y, Character.Action.Target, 5);
                }
                else found[1] = Character.Action.Target;

                if (Character.Information.Item.wID != 0)
                {
                    switch (ObjData.Manager.ItemBase[Character.Information.Item.wID].TypeID4)
                    {
                        //Chinese base skills
                        case 2:                 //One handed sword
                        case 3:
                            NumberAttack = 2;
                            AttackType = 2;
                            break;
                        case 4:                 //Spear attack + glavie
                        case 5:                 
                            NumberAttack = 1;
                            AttackType = 40;
                            break;
                        case 6:                 //Bow attack
                            NumberAttack = 1;
                            AttackType = 70;
                            break;
                        //Europe Base skills
                        case 7:
                            NumberAttack = 1;
                            AttackType = 7127; // One handed sword
                            break;
                        case 8:
                            NumberAttack = 1;
                            AttackType = 7128; // Two handed sword
                            break;
                        case 9:
                            NumberAttack = 2;
                            AttackType = 7129; // Axe basic attack
                            break;
                        case 10:
                            NumberAttack = 1;
                            AttackType = 9069; // Warlock base
                            break;
                        case 11:
                            NumberAttack = 1;
                            AttackType = 8454; // Staff / Tstaff
                            break;
                        case 12:
                            NumberAttack = 1;
                            AttackType = 7909; // Crossbow base
                            break;
                        case 13:
                            NumberAttack = 2; //Dagger
                            AttackType = 7910;
                            break;
                        case 14:
                            NumberAttack = 1;
                            AttackType = 9606; // Harp base
                            break;
                        case 15:
                            NumberAttack = 1;
                            AttackType = 9970; // Light staff cleric
                            break;
                        case 16:
                            NumberAttack = 1;
                            AttackType = ObjData.Manager.SkillBase[Character.Action.UsingSkillID].ID;
                            break;
                        default:
                            Console.WriteLine("Action attack case: {0} , SkillID = {1}" + ObjData.Manager.ItemBase[Character.Information.Item.wID].TypeID4, ObjData.Manager.SkillBase[Character.Action.UsingSkillID].ID);
                            break;
                    }
                }
                else
                {
                    //Punch attack
                    NumberAttack = 1;
                    AttackType = 1;
                }
                //Get casting id
                Character.Action.AttackingID = Character.Ids.GetCastingID();
                //Create new packet writer
                PacketWriter Writer = new PacketWriter();
                Writer.Create(OperationCode.SERVER_ACTION_DATA);
                Writer.Byte(1);
                Writer.Byte(2);
                Writer.Byte(0x30);
                Writer.DWord(AttackType);
                Writer.DWord(Character.Information.UniqueID);
                Writer.DWord(Character.Action.AttackingID);
                Writer.DWord(Character.Action.Target);
                Writer.Bool(true);
                Writer.Byte(NumberAttack);
                Writer.Byte(numbert);

                for (int t = 1; t <= numbert; t++)
                {
                    Writer.DWord(found[t]);

                    for (byte n = 1; n <= NumberAttack; n++)
                    {
                        //Set defaults
                        p_dmg = (int)Formule.gamedamage(Character.Stat.MaxPhyAttack, Character.Stat.AttackPower + MasteryGetPower(AttackType), 0, (double)target.PhyDef, Character.Information.Phy_Balance, Character.Stat.UpdatededPhyAttack);
                        status = 0;
                        crit = 0;
                        //Set target
                        target = new WorldMgr.targetObject(Helpers.GetInformation.GetObjects(found[t]), this);

                        //Make sure we dont get null error
                        if (Character.Action.UsingSkillID != 0)
                        {
                            //If we have an active imbue
                            if (ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Definedname == ObjData.s_data.Definedtype.Imbue)
                            {
                                p_dmg += ObjData.Manager.SkillBase[Character.Action.UsingSkillID].MaxAttack;
                                p_dmg += Rnd.Next(0, p_dmg.ToString().Length);
                            }
                        }
                        if (status != 128)
                        {
                            status = target.HP((int)p_dmg);
                        }
                        else
                        {
                            target.GetDead();
                            Character.Action.nAttack = false;
                        }
                        Writer.Byte(status);
                        Writer.Byte(crit);
                        Writer.DWord(p_dmg);
                        Writer.Byte(0);
                        Writer.Word(0);

                        p_dmg = 0;
                    }
                }
            
                Send(Writer.GetBytes());
                client.Send(Packet.ActionState(2, 0));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        byte ActionGetObject(ref int[] found, byte max, float ox, float oy, int objectid, byte metre)
        {
            byte founded = 1;
            found[1] = Character.Action.Target;
            try
            {
                float x = (float)ox - metre;
                float y = (float)oy - metre;
                for (int i = 0; i <= Helpers.Manager.Objects.Count - 1; i++)
                {
                    if (founded == max) return founded;
                    WorldMgr.Monsters o = Helpers.Manager.Objects[i];
                    if (!o.Die && o.LocalType == 1)
                    {
                        if (o.x >= x && o.x <= (x + (metre * 2)) && o.y >= y && o.y <= (y + (metre * 2)) && o.UniqueID != objectid)
                        {
                            founded++;
                            //if (o.AgressiveDMG == null) o.AgressiveDMG = new List<_agro>();
                            found[founded] = o.UniqueID;
                        }
                    }
                }
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (founded == max) return founded;
                            PlayerMgr sys = Helpers.Manager.clients[i];
                            if (sys.Character.Information.PvP)
                            {
                                if (sys.Character.Information.UniqueID != objectid && Character.Information.UniqueID != objectid && Helpers.Manager.clients[i].Character.Information.UniqueID != Character.Information.UniqueID)
                                {
                                    if (sys.Character.Position.x >= x && sys.Character.Position.x <= (x + (metre * 2)) && sys.Character.Position.y >= y && sys.Character.Position.y <= (y + (metre * 2)))
                                    {
                                        founded++;
                                        found[founded] = sys.Character.Information.UniqueID;
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
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return founded;

        }

        #endregion

        #region Skills

        void ActionSkill()
        {
            try
            {
                ObjData.s_data.TargetTypes TargetType = ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].TargetType;

                foreach (KeyValuePair<string, int> p in ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Properties1)
                {
                    Console.WriteLine($"{p.Key} -> {p.Value}");
                }

                bool gotoTarget = false;

                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Character.Action.sAttack) return;

                WorldMgr.Monsters o = null;
                PlayerMgr sys = null;

                if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.WorldMgr.Monsters")
                    //&& (TargetType == ObjData.s_data.TargetTypes.MOB
                    // || TargetType == (ObjData.s_data.TargetTypes.MOB | ObjData.s_data.TargetTypes.PLAYER)
                    // || TargetType == (ObjData.s_data.TargetTypes.MOB | ObjData.s_data.TargetTypes.NOTHING)
                    // || TargetType == (ObjData.s_data.TargetTypes.MOB | ObjData.s_data.TargetTypes.PLAYER | ObjData.s_data.TargetTypes.NOTHING)))
                {
                    o = Character.Action.Object as WorldMgr.Monsters;
                    if (o.Die || o.GetDie || o.LocalType != 1) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    //if (o.AgressiveDMG == null) o.AgressiveDMG = new List<_agro>();

                    x = o.x;
                    y = o.y;
                    aRound = o.aRound;


                    gotoTarget = true;
                }
                else if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr")
                    // && (TargetType == ObjData.s_data.TargetTypes.PLAYER
                    // || TargetType == ObjData.s_data.TargetTypes.PLAYER | TargetType == ObjData.s_data.TargetTypes.MOB
                    // || TargetType == ObjData.s_data.TargetTypes.PLAYER | TargetType == ObjData.s_data.TargetTypes.NOTHING
                    // || TargetType == ObjData.s_data.TargetTypes.PLAYER | TargetType == ObjData.s_data.TargetTypes.MOB | TargetType == ObjData.s_data.TargetTypes.NOTHING))
                {
                    if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].canSelfTargeted && (Character.Action.Target == Character.Information.UniqueID))
                    {
                        if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].needPVPstate && Character.Information.PvP)
                        { }
                        else
                        {
                            // have to seek for error msg
                            client.Send(Packet.Messages2(OperationCode.SERVER_ACTIONSTATE, Messages.UIIT_STT_ERR_COMMON_INVALID_TARGET));
                            Character.Action.sAttack = false;
                            StopSkillTimer();
                            return;
                        }

                        sys = Character.Action.Object as PlayerMgr;
                        x = sys.Character.Position.x;
                        y = sys.Character.Position.y;
                        aRound = sys.Character.aRound;

                        gotoTarget = true;
                    }
                    else
                    {
                        /* have to seek for error msg*/
                        client.Send(Packet.Messages2(OperationCode.SERVER_ACTIONSTATE, Messages.UIIT_STT_ERR_COMMON_INVALID_TARGET));
                        Character.Action.sAttack = false;
                        StopSkillTimer();
                        return;
                    }
                }
                // no target TargetType = 3;
                else if (Character.Action.Object == null
                     && (TargetType == ObjData.s_data.TargetTypes.NOTHING
                     || TargetType == ObjData.s_data.TargetTypes.NOTHING | TargetType == ObjData.s_data.TargetTypes.MOB
                     || TargetType == ObjData.s_data.TargetTypes.NOTHING | TargetType == ObjData.s_data.TargetTypes.PLAYER
                     || TargetType == ObjData.s_data.TargetTypes.NOTHING | TargetType == ObjData.s_data.TargetTypes.MOB | TargetType == ObjData.s_data.TargetTypes.PLAYER))
                {
                    gotoTarget = false;
                }
                else
                {
                    Character.Action.sAttack = false;
                    StopSkillTimer();
                    return;
                }

                // go to target
                if (gotoTarget)
                {
                    double distance = Formule.gamedistance(Character.Position.x,
                                                                    Character.Position.y,
                                                                    x,
                                                                    y);

                    if (Character.Action.Skill.Distance == 0)
                        distance -= ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    else distance -= Character.Action.Skill.Distance;
                    Console.WriteLine("Distance:" + distance);
                    if (distance > 1)
                    {
                        float farkx = x;
                        float farky = y;

                        //if (Character.Action.Skill.Distance == 0)
                        //{
                        //    if (ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE <= 10)
                        //        if (Systems.aRound(ref aRound, ref farkx, ref farky))
                        //        { Systems.aRound(ref farkx, ref farky, 1); }
                        //}

                        Character.Position.wX = farkx - Character.Position.x - (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.wY = farky - Character.Position.y - (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.kX = Character.Position.wX;
                        Character.Position.kY = Character.Position.wY;
                        if (!Character.InGame) return;
                        Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID,
                                    Formule.packetx(farkx, Character.Position.xSec),
                                    Character.Position.z,
                                    Formule.packety(farky, Character.Position.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));

                        distance += ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        
                        Character.Position.packetxSec = Character.Position.xSec;
                        Character.Position.packetySec = Character.Position.ySec;

                        Character.Position.packetX = (ushort)Formule.packetx(farkx, Character.Position.xSec);
                        Character.Position.packetY = (ushort)Formule.packety(farky, Character.Position.ySec);


                        Character.Position.Walking = true;
                        StartMovementTimer(GetMovementTime(distance));
                        return;
                    }
                }

                StartSkill();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            } 
        }

        void ActionSkillAttack()
        {
            try
            {

                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Character.Action.sAttack) return;
                WorldMgr.Monsters o = null;
                if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.WorldMgr.Monsters")
                {
                    o = Character.Action.Object as WorldMgr.Monsters;
                    if (o.Die || o.GetDie || o.LocalType != 1) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    //if (o.AgressiveDMG == null) o.AgressiveDMG = new List<_agro>();
                    x = o.x;
                    y = o.y;
                    aRound = o.aRound;

                }
                //else { StopAttackTimer(); return; }
                if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr")
                {
                    PlayerMgr sys = Character.Action.Object as PlayerMgr;
                    if (!Character.Information.PvP || sys.Character.State.Die) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    if (!(Character.Information.PvP && sys.Character.Information.PvP)) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    x = sys.Character.Position.x;
                    y = sys.Character.Position.y;
                    aRound = sys.Character.aRound;
                }
                //else { StopAttackTimer(); return; }

                if (x == 0 && y == 0 && aRound == null) { Character.Action.sAttack = false; StopSkillTimer(); return; }

                double distance = Formule.gamedistance(Character.Position.x,
                        Character.Position.y,
                        x,
                        y);

                /*if (Character.Action.Skill.Distance == 0)
                    distance -= ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                else distance -= Character.Action.Skill.Distance + Character.Stat.EkstraMetre;*/
                if (Character.Information.Item.wID != 0)
                    distance -= ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                


                    distance -= ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Distance;
                    distance -= Character.Stat.EkstraMetre;
                if (distance >= 1)
                {
                    float farkx = x;
                    float farky = y;

                    //if (Character.Action.Skill.Distance == 0)
                    //{
                    //    if (ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE <= 10)
                    //        if (Systems.aRound(ref aRound, ref farkx, ref farky))

                    //        { 
                    //            //Systems.aRound(ref farkx, ref farky, 1); 
                    //        }
                    //}
                    float eksilt = 0;
                    if (Character.Information.Item.wID != 0)
                    {
                        eksilt = ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Distance + (float)Character.Stat.EkstraMetre + (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    }
                    else
                    {
                        eksilt = ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Distance + (float)Character.Stat.EkstraMetre + (float)ObjData.Manager.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    }
                    Character.Position.wX = farkx - Character.Position.x - eksilt;
                    Character.Position.wY = farky - Character.Position.y - eksilt;
                    Character.Position.kX = Character.Position.wX;
                    Character.Position.kY = Character.Position.wY;
                    if (!Character.InGame) return;

                    Send(Packet.Movement(new ObjData.vektor(Character.Information.UniqueID,
                                Formule.packetx(farkx, Character.Position.xSec),
                                Character.Position.z,
                                Formule.packety(farky, Character.Position.ySec),
                                Character.Position.xSec,
                                Character.Position.ySec)));

                   
                    //Todo implent modified speed checks
                    
                    Character.Position.packetxSec = Character.Position.xSec;
                    Character.Position.packetySec = Character.Position.ySec;

                    Character.Position.packetX = (ushort)Formule.packetx(farkx - eksilt, Character.Position.xSec);
                    Character.Position.packetY = (ushort)Formule.packety(farky - eksilt, Character.Position.ySec);

                    Character.Position.Walking = true;
                    StartMovementTimer(GetMovementTime(distance));
                    return;
                }

                StartSkill();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void StartSkill()
        {
            try
            {
                if (!Character.Action.sAttack) return;
                if (CheckBow())
                {
                    StopSkillTimer();
                    StopAttackTimer();
                    return;
                }

                if (Character.Stat.SecondMP < ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Mana) 
                { 
                    Character.Action.sAttack = false; 
                    StopSkillTimer(); 
                    client.Send(Packet.ActionPacket(2, 4)); 
                    return; 
                }
                else
                {
                    Character.Stat.SecondMP -= ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Mana;

                    if (Character.Stat.SecondMP < 0) Character.Stat.SecondMP = 1;
                    UpdateMp();

                    Character.Action.Skill.MainCasting = Character.Ids.GetCastingID();
                    List<int> lis = Character.Spawn;

                    Send(lis, Packet.ActionPacket(1, 0, Character.Action.Skill.MainSkill, Character.Information.UniqueID, Character.Action.Skill.MainCasting, Character.Action.Target));

                    if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].isAttackSkill)
                    {

                        if (Character.Action.Skill.Instant == 0) 
                            MainSkill_Attack(lis);
                        else
                            StartSkillCastingTimer(Character.Action.Skill.Instant * 1000, lis);
                    }
                    else
                    {
                        Character.Action.sAttack = false;
                        Character.Action.sCasting = false;

                        // todo: here iterate through all target player (not only one object => efr)
                        if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Properties1.ContainsKey("heal") ||
                            ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Properties1.ContainsKey("curl"))
                        {
                            PlayerMgr Target = Character.Action.Object as PlayerMgr;

                            Target.SpecialBuff(Character.Action.Skill.MainSkill);
                        }
                        Effect.EffectMain(Character.Action.Object, Character.Action.Skill.MainSkill);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void MainSkill_Attack(List<int> list)
        {
            if (!Character.Action.sAttack) return;
            try
            {

                AmountControl();
                //Send(Packet.SkillBirdAttack(Character.Action.Target));
                int[,] p_dmg = new int[Character.Action.Skill.Found, Character.Action.Skill.NumberOfAttack];
                int[] statichp = new int[Character.Action.Skill.Found];

                PacketWriter Writer = new PacketWriter();

                Writer.Create(OperationCode.SERVER_SKILL_DATA);
                Writer.Byte(1);
                Writer.DWord(Character.Action.Skill.MainCasting);
                Writer.DWord(Character.Action.Target);
                Writer.Byte(1);
                Writer.Byte(Character.Action.Skill.NumberOfAttack);
                Writer.Byte(Character.Action.Skill.Found);
                byte[] status;
                status = new byte[Character.Action.Skill.Found];
                WorldMgr.targetObject[] target = new WorldMgr.targetObject[Character.Action.Skill.Found];
                for (byte f = 0; f < Character.Action.Skill.Found; f++)
                {
                    if (Character.Action.Skill.FoundID[f] != 0)
                    {
                        Writer.DWord(Character.Action.Skill.FoundID[f]);
                        target[f] = new WorldMgr.targetObject(Helpers.GetInformation.GetObjects(Character.Action.Skill.FoundID[f]), this);
                        if (target[f].sys == null && target[f].os == null) { }
                        else
                        {
                            statichp[f] = target[f].GetHp;
                            for (byte n = 0; n < Character.Action.Skill.NumberOfAttack; n++)
                            {
                                bool block = false;
                                /*if (Character.Information.Item.sID != 0 && Character.Information.Item.sID != 62)
                                {
                                    if (ObjData.RandomID.GetRandom(0, 25) < 10) block = true;
                                }*/
                                if (!block)
                                {
                                    byte crit = 1;
                                    p_dmg[f, n] = 1;

                                    if (Character.Action.Skill.P_M) // for magic damage
                                    {
                                        p_dmg[f, n] = (int)Formule.gamedamage((Character.Stat.MaxMagAttack + ObjData.Manager.SkillBase[Character.Action.Skill.SkillID[n]].MaxAttack), MasteryGetPower(Character.Action.Skill.SkillID[n]), target[f].AbsrobMag, target[f].MagDef, Character.Information.Mag_Balance, (ObjData.Manager.SkillBase[Character.Action.Skill.SkillID[n]].MagPer + Character.Stat.UpdatededMagAttack));
                                        if (Character.Action.ImbueID != 0) p_dmg[f, n] += (int)Formule.gamedamage((Character.Stat.MinMagAttack + ObjData.Manager.SkillBase[Character.Action.ImbueID].MaxAttack), MasteryGetPower(Character.Action.ImbueID), 0, target[f].MagDef, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);
                                    }
                                    else // for phy damage
                                    {
                                        p_dmg[f, n] = (int)Formule.gamedamage((Character.Stat.MaxPhyAttack + ObjData.Manager.SkillBase[Character.Action.Skill.SkillID[n]].MaxAttack), MasteryGetPower(Character.Action.Skill.SkillID[n]), target[f].AbsrobPhy, target[f].PhyDef, Character.Information.Phy_Balance, Character.Stat.UpdatededPhyAttack + ObjData.Manager.SkillBase[Character.Action.Skill.SkillID[n]].MagPer);
                                        if (Character.Action.ImbueID != 0) p_dmg[f, n] += (int)Formule.gamedamage((Character.Stat.MinMagAttack + ObjData.Manager.SkillBase[Character.Action.ImbueID].MaxAttack), MasteryGetPower(Character.Action.ImbueID), 0, target[f].MagDef, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);
                                        if (Rnd.Next(16) < 5)
                                        {
                                            p_dmg[f, n] *= 2;
                                            crit = 2;
                                        }
                                    }

                                    if (f > 0) p_dmg[f, n] = (p_dmg[f, n] * (100 - (f * 10))) / 100;

                                    if (Character.Information.Berserking)
                                        p_dmg[f, n] = (p_dmg[f, n] * Character.Information.BerserkOran) / 100;

                                    if (p_dmg[f, n] <= 0)
                                        p_dmg[f, n] = 1;
                                    else
                                    {
                                        if (target[f].mAbsorb() > 0)
                                        {
                                            int static_dmg = (p_dmg[f, n] * (100 - (int)target[f].mAbsorb())) / 100;
                                            target[f].MP(static_dmg);
                                            p_dmg[f, n] = static_dmg;
                                        }
                                        p_dmg[f, n] += Rnd.Next(0, p_dmg.ToString().Length);
                                    }


                                    statichp[f] -= p_dmg[f, n];
                                    if (statichp[f] < 1)
                                    {
                                        status[f] = 128;
                                        target[f].GetDead();
                                    }


                                    if (Character.Action.Skill.OzelEffect == 4 && status[f] != 128)
                                    {
                                        if (Rnd.Next(20) < 5)
                                            status[f] = 4;
                                    }
                                    if (Character.Action.Skill.OzelEffect == 5 && status[f] != 128)
                                    {
                                        if (Rnd.Next(20) < 5)
                                            status[f] = 5;
                                    }

                                    Writer.Byte(status[f]); //so here we add status same opcode 132:Knock down yaptığın zaman ölecekse 133:knock back yaptığın zaman ölecekse
                                    Writer.Byte(crit);
                                    Writer.DWord(p_dmg[f, n]);
                                    Writer.Byte(0);
                                    Writer.Word(0);

                                   

                                    if (status[f] == 4) // if status was knockdown just add the new position of the mob where it should be knocked down
                                    {
                                        Writer.Byte(target[f].xSec);
                                        Writer.Byte(target[f].ySec);
                                        Writer.Word(Formule.packetx(target[f].x, target[f].xSec));
                                        Writer.Word(0);
                                        Writer.Word(Formule.packety(target[f].y, target[f].ySec));
                                         Writer.Word(65535);
                                        Writer.Word(1832);
                                        Writer.Word(0);
                                    }
                                    else if (status[f] == 5)
                                    {
                                        if (target[f].os == null && target[f].sys != null)
                                        {
                                            target[f].sys.Character.Position.x += 1;
                                            target[f].sys.Character.Position.y += 1;
                                        }
                                        else
                                        {
                                            target[f].os.x += 1;
                                            target[f].os.y += 1;
                                        }
                                        Writer.Byte(target[f].xSec);
                                        Writer.Byte(target[f].ySec);
                                        Writer.Word(Formule.packetx(target[f].x, target[f].xSec));
                                        Writer.Word(0);
                                        Writer.Word(Formule.packety(target[f].y, target[f].ySec));
                                        Writer.Word(65535);
                                        Writer.Word(1832);
                                        Writer.Word(0);
                                    }
                                }
                                else Writer.Byte(2);
                            }
                        }

                    }
                }
                Send(list, Writer.GetBytes());

                for (byte f = 0; f < Character.Action.Skill.Found; f++)
                {
                    if (target[f].sys == null && target[f].os == null)
                    { }
                    else
                    {
                        if (target[f].sys != null && target[f].os == null) // player
                            Effect.EffectMain(target[f].sys, Character.Action.Skill.MainSkill);

                        if (target[f].sys == null && target[f].os != null) // mob
                            Effect.EffectMain(target[f].os, Character.Action.Skill.MainSkill);
                    }
                }

                Character.Action.sCasting = true;
                Character.Action.sAttack = false;
                /*if (ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Action_CoolTime == 0) 
                    ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Action_CoolTime = 150;*/

                StartsWaitTimer(ObjData.Manager.SkillBase[Character.Action.Skill.MainSkill].Action_ActionDuration, target, p_dmg, status);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        void AmountControl()
        {
            try
            {
                Character.Action.Skill.FoundID[Character.Action.Skill.Found] = Character.Action.Target;
                Character.Action.Skill.Found++;
                if (Character.Action.Skill.Tdistance > 1)
                {
                    object mainObject = Helpers.GetInformation.GetObjects(Character.Action.Target);
                    if (mainObject == null) return;
                    WorldMgr.targetObject target = new WorldMgr.targetObject(mainObject, this);
                    float x = target.x - Character.Action.Skill.Tdistance;
                    float y = target.y - Character.Action.Skill.Tdistance;
                    for (int i = 0; i < Helpers.Manager.Objects.Count; i++)
                    {
                        if (Character.Action.Skill.Found == Character.Action.Skill.Tdistance) return;
                        if (Helpers.Manager.Objects[i] != null)
                        {
                            WorldMgr.Monsters o = Helpers.Manager.Objects[i];
                            if (!o.Die && o.LocalType == 1)
                            {
                                if (o.x >= x && o.x <= (x + (Character.Action.Skill.Tdistance * 2)) && o.y >= y - Character.Action.Skill.Tdistance && o.y <= (y + (Character.Action.Skill.Tdistance * 2)) && o.UniqueID != Character.Action.Target)
                                {
                                    //if (o.AgressiveDMG == null) o.AgressiveDMG = new List<_agro>();
                                    Character.Action.Skill.FoundID[Character.Action.Skill.Found] = o.UniqueID;
                                    Character.Action.Skill.Found++;
                                }
                            }
                        }
                    }
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            try
                            {
                                if (Character.Action.Skill.Found == Character.Action.Skill.Tdistance) return;
                                if (Helpers.Manager.clients[i] != null)
                                {
                                    PlayerMgr sys = Helpers.Manager.clients[i];
                                    if (sys.Character.Information.PvP && sys != this && !sys.Character.State.Die)
                                        if (sys.Character.Information.UniqueID != Character.Action.Target && Character.Information.UniqueID != Character.Action.Target)
                                        {
                                            if (sys.Character.Position.x >= x && sys.Character.Position.x <= (x + (Character.Action.Skill.Tdistance * 2)) && sys.Character.Position.y >= y && sys.Character.Position.y <= (y + (Character.Action.Skill.Tdistance * 2)))
                                            {
                                                Character.Action.Skill.FoundID[Character.Action.Skill.Found] = sys.Character.Information.UniqueID;
                                                Character.Action.Skill.Found++;
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
                    target = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        #endregion
        #region Petskills
        void PetSkill(int skillid, WorldMgr.pet_obj o)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(OperationCode.SERVER_ACTION_DATA);
            /* Will add this later
             * Packet sniff:
             * [S -> C][B070]
             * 01                                                ................
             * 02 30                                             .0..............
             * 47 0F 00 00                                       G...............
             * AF FD 1A 03                                       ................
             * 1F 51 03 00                                       .Q..............
             * 93 AA 1A 00                                       ................
             * 01                                                ................
             * 01                                                ................
             * 01                                                ................
             * 93 AA 1A 00                                       ................
             * 00                                                ................
             * 01 0F 00 00                                       ................
             * 00 00 00 00                                       ................
             */
        }
        #endregion

        #region Moving Skill
        void MovementSkill(PacketReader Reader)
        {
            try
            {
                if (!Character.Action.movementskill)
                {
                    Character.Action.movementskill = true;
                    MovementSkillTimer(ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties1["tele"] + 500);
                    if (Character.Action.sAttack || Character.Action.sCasting) return;

                    if (Character.Stat.SecondMP < ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Mana) { client.Send(Packet.ActionPacket(2, 4)); return; }
                    else
                    {
                        Character.Stat.SecondMP -= ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Mana;
                        UpdateMp();
                        if (Timer.Movement != null) { Timer.Movement.Dispose(); Character.Position.Walking = false; }

                        byte xSec = Reader.Byte(), ySec = Reader.Byte();
                        int x = Reader.Int32(), z = Reader.Int32(), y = Reader.Int32();
                        Reader.Close();

                        float gamex = Formule.gamex((float)x, xSec);
                        float gamey = Formule.gamey((float)y, ySec);

                        float farkx = gamex - Character.Position.x;
                        float farky = gamey - Character.Position.y;

                        float hesapy = 0, hesapx = 0;

                        while (hesapx + hesapy < ObjData.Manager.SkillBase[Character.Action.UsingSkillID].Properties2["tele"] / 10)
                        {
                            Character.Position.x += (farkx / 30);
                            Character.Position.y += (farky / 30);
                            hesapx += Math.Abs((farkx / 30));
                            hesapy += Math.Abs((farky / 30));
                        }

                        PacketWriter Writer = new PacketWriter();

                        Writer.Create(OperationCode.SERVER_ACTION_DATA);
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(0x30);
                        int overid = Character.Ids.GetCastingID();
                        Writer.DWord(Character.Action.UsingSkillID);//skillid
                        Writer.DWord(Character.Information.UniqueID); //charid
                        Writer.DWord(overid);//overid
                        Writer.DWord(0);
                        Writer.Byte(8);
                        Writer.Byte(xSec);
                        Writer.Byte(ySec);
                        Writer.DWord(Formule.packetx(Character.Position.x, xSec));
                        Writer.DWord(0);
                        Writer.DWord(Formule.packety(Character.Position.y, ySec));

                        Send(Writer.GetBytes());

                        client.Send(Packet.ActionState(2, 0));

                        ObjectSpawnCheck();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            } 
        }
        #endregion

        #region Handle Buff Attributes
        public bool SkillAdd_Properties(PlayerMgr Target, string PropertiesName, bool UpdatePacket, byte slot = 255, int skillid = -1)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "hpi":
                        ChangeMaxHP_hpi(Target, slot, false, UpdatePacket);
                        break;
                    case "mpi":
                        ChangeMaxMP_mpi(Target, slot, false, UpdatePacket);
                        break;
                    case "dru":
                        ChangeAtk_dru(Target, slot, false,UpdatePacket);
                        break;
                    case "er":
                        ChangeParry_er(Target, slot, false,UpdatePacket);
                        break;
                    case "stri":
                        ChangeStr_stri(Target, slot, false,UpdatePacket);
                        break;
                    case "inti":
                        ChangeInt_inti(Target, slot, false, UpdatePacket);
                        break;
                    case "cr":
                        ChangeCrit_cr(Target, slot, false,UpdatePacket);
                        break;
                    case "br":
                        ChangeBlockingRatio_br(Target, slot, false,UpdatePacket);
                        break;
                    case "spda":
                        Change_spda(Target, slot, false,UpdatePacket);
                        break;
                    case "ru":
                        ChangeRange_ru(Target, slot, false,UpdatePacket);
                        break;
                    case "dgmp":
                        ChangeAbsorbMP_dgmp(Target, slot, false,UpdatePacket);
                        break;
                    case "defp":
                        ChangeDefPower_defp(Target, slot, false, UpdatePacket);
                        break;
                    case "hste":
                        ChangeSpeed_hste(Target,slot,false,UpdatePacket);
                        break;
                    case "drci":
                        ChangeCriticalParry_dcri(Target, slot, false, UpdatePacket);
                        break;
                    case "heal":
                        HealHPMP(Target, slot, skillid, false, UpdatePacket);
                        break;
                    case "E1SA": // setvaluek ( valószínű ) nem így lesznek 
                        ChangePhyAtk_E1SA(Target, slot, false, UpdatePacket);
                        break;
                    case "E2SA":
                        ChangePhyAtk_E2SA(Target, slot, false, UpdatePacket);
                        break;
                    case "E2AH":
                        ChangeHitRate_E2AH(Target, slot, false, UpdatePacket);
                        break;
                    case "terd":
                        ChangeParry_terd(Target, slot, false, UpdatePacket);
                        break;
                    case "chcr":
                        ChangeTargetHp_chcr(Target, slot, false, UpdatePacket);
                        break;
                    case "cmcr":
                        ChangeTargetHp_cmcr(Target, slot, false, UpdatePacket);
                        break;
                    case "thrd":
                        ChangeDecAttkRate_thrd(Target, slot, false, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        bool SkillDelete_Properties(PlayerMgr Target, string PropertiesName, bool UpdatePacket, byte slot = 255, int skillid = -1)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "hpi":
                        ChangeMaxHP_hpi(Target, slot, true, UpdatePacket);
                        break;
                    case "mpi":
                        ChangeMaxMP_mpi(Target, slot, true, UpdatePacket);
                        break;
                    case "dru":
                        ChangeAtk_dru(Target, slot, true,UpdatePacket);
                        break;
                    case "er":
                        ChangeParry_er(Target, slot, true,UpdatePacket);
                        break;
                    case "stri":
                        ChangeStr_stri(Target, slot, true,UpdatePacket);
                        break;
                    case "inti":
                        ChangeInt_inti(Target, slot, true, UpdatePacket);
                        break;
                    case "cr":
                        ChangeCrit_cr(Target, slot, true,UpdatePacket);
                        break;
                    case "br":
                        ChangeBlockingRatio_br(Target, slot, true,UpdatePacket);
                        break;
                    case "spda":
                        Change_spda(Target, slot, true,UpdatePacket);
                        break;
                    case "ru":
                        ChangeRange_ru(Target, slot, true,UpdatePacket);
                        break;
                    case "dgmp":
                        ChangeAbsorbMP_dgmp(Target, slot, true,UpdatePacket);
                        break;
                    case "defp":
                        ChangeDefPower_defp(Target, slot, true, UpdatePacket);
                        break;
                    case "hste":
                        ChangeSpeed_hste(Target, slot, true, UpdatePacket);
                        break;
                    case "drci":
                        ChangeCriticalParry_dcri(Target, slot, true, UpdatePacket);
                        break;
                    case "heal":
                        HealHPMP(Target, slot, skillid, true, UpdatePacket);
                        break;
                    case "E1SA": // setvaluek ( valószínű ) nem így leszneek 
                        ChangePhyAtk_E1SA(Target, slot, true, UpdatePacket);
                        break;
                    case "E2SA":
                        ChangePhyAtk_E2SA(Target, slot, true, UpdatePacket);
                        break;
                    case "E2AH":
                        ChangeHitRate_E2AH(Target, slot, true, UpdatePacket);
                        break;
                    case "terd":
                        ChangeParry_terd(Target, slot, true, UpdatePacket);
                        break;
                    case "chcr":
                        ChangeTargetHp_chcr(Target, slot, true, UpdatePacket);
                        break;
                    case "cmcr":
                        ChangeTargetHp_cmcr(Target, slot, true, UpdatePacket);
                        break;
                    case "thrd":
                        ChangeDecAttkRate_thrd(Target, slot, true, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }

        public void ChangeMaxHP_hpi(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hpi"] != 0) // point inc
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = amount;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["hpi"] != 0) // %inc
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["hpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = (Target.Character.Stat.Hp / 100) * (amount);
                    }
                    // add it
                    Target.Character.Stat.Hp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;

                    if ((Target.Character.Stat.SecondHp + Target.Character.Action.Buff.UpdatedStats[slot].Hp) > Target.Character.Stat.Hp)
                    {
                        Target.Character.Stat.SecondHp = Target.Character.Stat.Hp;
                    }
                    else
                    {
                        Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }
                }
                else
                {
                    // sub it
                    Target.Character.Stat.Hp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;

                    // dont kill him :)
                    if (Target.Character.Stat.SecondHp - Target.Character.Action.Buff.UpdatedStats[slot].Hp < 1)
                    {
                        Target.Character.Stat.SecondHp = 1;
                    }
                    else
                    {
                        Target.Character.Stat.SecondHp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }

                    Target.Character.Action.Buff.UpdatedStats[slot].Hp = 0;

                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeMaxMP_mpi(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["mpi"] != 0) // point inc
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["mpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = amount;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["mpi"] != 0) // %inc
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["mpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = (Target.Character.Stat.Hp / 100) * (amount);
                    }

                    Target.Character.Stat.Mp += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;

                    if ((Target.Character.Stat.SecondMP + Target.Character.Action.Buff.UpdatedStats[slot].Mp) > Target.Character.Stat.Mp)
                    {
                        Target.Character.Stat.SecondMP = Target.Character.Stat.Mp;
                    }
                    else
                    {
                        Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    }
                }
                else
                {
                    // sub it
                    Target.Character.Stat.Mp -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;

                    // dont want negative mana
                    if (Target.Character.Stat.SecondMP - Target.Character.Action.Buff.UpdatedStats[slot].Mp < 1)
                    {
                        Target.Character.Stat.SecondMP = 1;
                    }
                    else
                    {
                        Target.Character.Stat.SecondMP -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    }

                    Target.Character.Action.Buff.UpdatedStats[slot].Mp = 0;
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
               Log.Exception(ex);
            }
        }
        public void ChangeAtk_dru(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"] != 0) // phy attack %inc
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * (amount);
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * (amount);
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"] != 0) // mag attack %inc
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack = (Target.Character.Stat.MinMagAttack / 100) * (amount);
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack = (Target.Character.Stat.MaxMagAttack / 100) * (amount);
                        Target.Character.Stat.MinMagAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack;
                        Target.Character.Stat.MaxMagAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack;
                    }


                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"] != 0) // phy attack %inc
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"] != 0) // mag attack %inc
                    {
                        Target.Character.Stat.MinMagAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack;
                        Target.Character.Stat.MaxMagAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeParry_er(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"] != 0) // parry inc
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"];

                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = amount;
                        Target.Character.Stat.Parry += Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                    }
                    else if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["er"] != 0) // parry %inc?
                    {

                    }
                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"] != 0) // parry inc
                    {
                        Target.Character.Stat.Parry -= Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = 0;
                    }
                    else if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["er"] != 0) // parry %inc?
                    {
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeStr_stri(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["stri"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["stri"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Strength = (short)amount;
                    }
                    /*if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("stri"))
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["stri"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Strength = (short)((Target.Character.Stat.Strength / 100) * (amount));
                    }*/
                    Target.Character.Stat.Strength += Target.Character.Action.Buff.UpdatedStats[slot].Strength;
                }
                else
                {
                    Target.Character.Stat.Strength -= Target.Character.Action.Buff.UpdatedStats[slot].Strength;
                    Target.Character.Action.Buff.UpdatedStats[slot].Strength = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeInt_inti(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["inti"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["inti"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = (short)amount;
                    }
                    //TODO majd uncomment.
                    /*if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("inti"))
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["inti"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = (short)((Target.Character.Stat.Intelligence / 100) * (amount));
                    }*/
                    Target.Character.Stat.Intelligence += Target.Character.Action.Buff.UpdatedStats[slot].Intelligence;
                }
                else
                {
                    Target.Character.Stat.Intelligence -= Target.Character.Action.Buff.UpdatedStats[slot].Intelligence;
                    Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeBlockingRatio_br(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["br"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"];
                        Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = amount;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"];
                        Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = (Target.Character.Stat.BlockRatio / 100) * (amount);
                    }
                    Target.Character.Stat.BlockRatio += Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio;
                }
                else
                {
                    Target.Character.Stat.BlockRatio -= Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio;
                    Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeCrit_cr(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            if (!delete)
            {

            }
        }
        public void Change_spda(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                double amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1.ContainsKey("spda"))  //Phydef decrease?
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["spda"];
                        Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef = (Target.Character.Stat.PhyDef / 100) * (amount);
                        Target.Character.Stat.PhyDef -= Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("spda")) //Phy attack inc?
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["spda"];
                        Target.Character.Stat.UpdatededPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * (amount);
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Stat.UpdatededPhyAttack;
                    }
                }
                else
                {
                    Target.Character.Stat.PhyDef += Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef;
                    Target.Character.Stat.MaxPhyAttack -= Target.Character.Stat.UpdatededPhyAttack;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeSpeed_hste(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hste"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hste"];
                        Target.Character.Speed.INC += amount;
                        Character.Speed.INC = amount;

                    }
                }
                else
                {
                    Target.Character.Speed.INC -= ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hste"];
                }
                if (UpdatePacket) Target.client.Send(Packet.SetSpeed(Target.Character.Information.UniqueID, Target.Character.Speed.WalkSpeed, Target.Character.Speed.RunSpeed));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeRange_ru(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["ru"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["ru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre = (byte)amount;
                        Target.Character.Stat.EkstraMetre += Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre;
                    }
                }
                else
                {
                    Target.Character.Stat.EkstraMetre -= Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre;
                    Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeAbsorbMP_dgmp(PlayerMgr Target, int slot, bool delete,bool UpdatePacket) // dgmp stat ellenőrzése hogy mire jó??
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dgmp"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dgmp"];
                        Target.Character.Stat.Absorb_mp = amount;
                    }
                }
                else
                {
                    Target.Character.Stat.Absorb_mp = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeCriticalParry_dcri(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"];
                        Target.Character.Stat.CritParryRatio += amount;
                    }
                }
                else
                {
                    amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"];
                    Target.Character.Stat.CritParryRatio -= amount;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeDefPower_defp(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"];
                        Target.Character.Action.Buff.UpdatedStats[slot].PhyDef = amount;
                        Target.Character.Stat.PhyDef += Target.Character.Action.Buff.UpdatedStats[slot].PhyDef;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MagDef = amount;
                        Target.Character.Stat.MagDef += Target.Character.Action.Buff.UpdatedStats[slot].MagDef;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties3["defp"] != 0)
                    {
                        //nemtudjuk
                    }
                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"] != 0)
                    {
                        Target.Character.Stat.PhyDef -= Target.Character.Action.Buff.UpdatedStats[slot].PhyDef;
                        Target.Character.Action.Buff.UpdatedStats[slot].PhyDef = 0;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"] != 0)
                    {
                        Target.Character.Stat.MagDef -= Target.Character.Action.Buff.UpdatedStats[slot].MagDef;
                        Target.Character.Action.Buff.UpdatedStats[slot].MagDef = 0;
                    }
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties3["defp"] != 0)
                    {
                        //nemtudjuk
                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        static public void HealHPMP(PlayerMgr Target, int slot, int skillid, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                int sid;

                // get skillid from parameters
                if (skillid == -1)
                    sid = Target.Character.Action.Buff.SkillID[slot];
                else
                    sid = skillid;

                if (!delete)
                {
                    // if hp full
                    if (Target.Character.Stat.SecondHp == Target.Character.Stat.Hp) return;

                    if (ObjData.Manager.SkillBase[sid].Properties1["heal"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[sid].Properties1["heal"];

                        // add the calculated amount
                        if (Target.Character.Stat.SecondHp + amount < Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += amount;
                        else if (Target.Character.Stat.SecondHp != Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += Target.Character.Stat.Hp - Target.Character.Stat.SecondHp;

                        if (UpdatePacket) Target.UpdateHp();
                    }
                    if (ObjData.Manager.SkillBase[sid].Properties2["heal"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[sid].Properties2["heal"];
                        amount = (Target.Character.Stat.Hp / 100) * amount;

                        // add the calculated amount
                        if (Target.Character.Stat.SecondHp + amount < Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += amount;
                        else if (Target.Character.Stat.SecondHp != Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += Target.Character.Stat.Hp - Target.Character.Stat.SecondHp;

                        if (UpdatePacket) Target.UpdateHp();
                    }
                    if (ObjData.Manager.SkillBase[sid].Properties3["heal"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[sid].Properties3["heal"];

                        // add the calculated amount
                        if (Target.Character.Stat.SecondMP + amount < Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += amount;
                        else if (Target.Character.Stat.SecondMP != Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += Target.Character.Stat.Mp - Target.Character.Stat.SecondMP;

                        if (UpdatePacket) Target.UpdateMp();

                    }
                    if (ObjData.Manager.SkillBase[sid].Properties3["heal"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[sid].Properties4["heal"];
                        amount = (Target.Character.Stat.Mp / 100) * amount;

                        // add the calculated amount
                        if (Target.Character.Stat.SecondMP + amount < Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += amount;
                        else if (Target.Character.Stat.SecondMP != Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += Target.Character.Stat.Mp - Target.Character.Stat.SecondMP;

                        if (UpdatePacket) Target.UpdateMp();
                    }
                }
                else
                {
                    //dunno....
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            
        }
        public void ChangeHitrate_hr(PlayerMgr Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                //AttackRate = HitRate ???
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1.ContainsKey("hr"))
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hr"];


                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangePhyAtk_E1SA(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * amount;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * amount;
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }

                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"] != 0)
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangePhyAtk_E2SA(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"] != 0)
                    {
                        amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * amount;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * amount;
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }

                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"] != 0)
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        public void ChangeHitRate_E2AH(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"] != 0)
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = amount;
                        Target.Character.Stat.Hit += Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                    }

                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"] != 0)
                    {
                        Target.Character.Stat.Hit -= Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = 0;
                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }      // setvaluek ( valószínű ) nem így leszneek 
        public void ChangeParry_terd(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"] != 0)
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = amount;
                        Target.Character.Stat.Parry -= Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                    }

                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"] != 0)
                    {
                        Target.Character.Stat.Parry += Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeTargetHp_chcr(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"] != 0)
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = amount;
                        Target.Character.Stat.Hp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Stat.SecondHp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }
                }

                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"] != 0)
                    {
                        Target.Character.Stat.Hp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void ChangeTargetHp_cmcr(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"] != 0)
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = amount;
                        Target.Character.Stat.Mp -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Stat.SecondMP -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;


                    }
                }

                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"] != 0)
                    {
                        Target.Character.Stat.Mp += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        public void ChangeDecAttkRate_thrd(PlayerMgr Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"] != 0)
                    {
                        int amount = ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = amount;
                        Target.Character.Stat.Hit -= Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                    }
                }
                else
                {
                    if (ObjData.Manager.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"] != 0)
                    {
                        Target.Character.Stat.Hit += Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        public void HandleEffect()
        {

        }
        public void HandleSpecialBuff(int skillid) // theese attributes are not serializable
        {
            try
            {
                string series = ObjData.Manager.SkillBase[skillid].Series.Remove(ObjData.Manager.SkillBase[skillid].Series.Length - 2);
                switch (series)
                {
                    case "SKILL_OP_HARMONY":
                    case "SKILL_CH_WATER_HARMONY":

                        WorldMgr.spez_obj so = new WorldMgr.spez_obj();

                        so.Name = series;
                        so.ID = skillid;
                        so.spezType = 0x850;
                        so.Radius = ObjData.Manager.SkillBase[skillid].Distance / 10;
                        so.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                        so.UniqueID = so.Ids.GetUniqueID;

                        so.xSec = Character.Position.xSec;
                        so.ySec = Character.Position.ySec;
                        so.x = Character.Position.x;
                        so.z = Character.Position.z;
                        so.y = Character.Position.y;

                        Helpers.Manager.SpecialObjects.Add(so);
                        so.SpawnMe(ObjData.Manager.SkillBase[skillid].Properties1["dura"]);

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        #endregion

       /*#region Handle Skill Attributes
        bool SkillHandle_Properties(Systems Target, string PropertiesName, bool UpdatePacket)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "heal":
                        ChangeHeal(Target, slot, false, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SkillHandle_Properties() error..");
                Log.Exception(ex);
            }

            return false;
        }
        #endregion*/
    }
}
