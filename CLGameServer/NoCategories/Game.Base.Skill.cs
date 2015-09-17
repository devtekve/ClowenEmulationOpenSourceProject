using CLFramework;
using System;

namespace CLGameServer.Base
{
    class Skill
    {
        #region Using skills
        public static WorldMgr.character._usingSkill Info(int SkillID, WorldMgr.character Character)
        {
            //Create new global information
            WorldMgr.character._usingSkill info = new WorldMgr.character._usingSkill();
            //Wrap our function inside a catcher
            try
            {
                //Set default skill information
                info.MainSkill = SkillID;
                info.SkillID = new int[10];
                info.FoundID = new int[10];
                info.TargetType = new bool[10];
                info.NumberOfAttack = NumberAttack(SkillID, ref info.SkillID);
                info.Targethits = 1;
                info.Distance = Convert.ToByte(ObjData.Manager.SkillBase[SkillID].Distance);
                info.Tdistance = 0;
                info.canUse = true;
                //Switch on skills series
                switch (ObjData.Manager.SkillBase[SkillID].Series)
                {
                    case "SKILL_EU_ROG_TRANSFORMA_MASK_A":
                        break;
                    #region Bicheon
                    #region Smashing Series
                    case "SKILL_CH_SWORD_SMASH_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_C":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_D":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_E":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Chain Sword Attack Series
                    case "SKILL_CH_SWORD_CHAIN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_E":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_F":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_G":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Blade Force Series
                    case "SKILL_CH_SWORD_GEOMGI_A":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_B":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_C":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_D":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_E":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Hidden Blade Series
                    case "SKILL_CH_SWORD_KNOCKDOWN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 3;
                        info.OzelEffect = 4;
                        break;
                    #endregion
                    #region Killing Heaven Blade Series
                    case "SKILL_CH_SWORD_DOWNATTACK_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    #endregion
                    #region Sword Dance Series
                    case "SKILL_CH_SWORD_SPECIAL_A":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_B":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_C":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_D":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 15;
                        break;
                    #endregion
                    #endregion

                    #region Heuksal
                    #region Annihilating Blade Series
                    case "SKILL_CH_SPEAR_PIERCE_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_C":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_D":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_E":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Heuksal Spear Series
                    case "SKILL_CH_SPEAR_FRONTAREA_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_E":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    #endregion
                    #region Soul Departs Spear Series
                    case "SKILL_CH_SPEAR_STUN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_E":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Ghost Spear Attack Series
                    case "SKILL_CH_SPEAR_ROUNDAREA_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Chain Spear Attack Series
                    case "SKILL_CH_SPEAR_CHAIN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_E":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_F":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    #endregion
                    #region Flying Dragon Spear Series
                    case "SKILL_CH_SPEAR_SHOOT_A":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_B":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_C":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_D":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion

                    #region Pacheon
                    #region Anti Devil Bow Series
                    case "SKILL_CH_BOW_CRITICAL_A":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_B":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_D":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_E":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Arrow Combo Attack Series
                    case "SKILL_CH_BOW_CHAIN_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 7;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_BOW_CHAIN_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.Distance = 7;
                        info.OzelEffect = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_C":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.Distance = 7;
                        info.OzelEffect = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_D":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.Distance = 7;
                        info.OzelEffect = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_E":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.Distance = 7;
                        info.OzelEffect = 5;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Autumn Wind Arrow Series
                    case "SKILL_CH_BOW_PIERCE_A":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_B":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_D":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Explosion Arrow Series
                    case "SKILL_CH_BOW_AREA_A":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_B":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_D":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Strong Bow Series
                    case "SKILL_CH_BOW_POWER_A":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_POWER_B":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_POWER_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_POWER_D":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Mind Bow Series
                    case "SKILL_CH_BOW_SPECIAL_A":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 2;
                        info.Tdistance = 20;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_B":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 22;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 23;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_D":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 24;
                        break;
                    #endregion
                    #endregion

                    #region Cold
                    #region Snow Storm Series
                    case "SKILL_CH_COLD_GIGONGSUL_A":
                        info.Instant = 2;
                        info.Distance = 5;
                        info.P_M = true;

                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_B":
                        info.Instant = 2;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 6;
                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_C":
                        info.Instant = 2;
                        info.Distance = 5;
                        info.P_M = true;
                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_D":
                        info.Instant = 2;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 7;
                        break;
                    #endregion
                    #endregion

                    #region Light
                    #region Lion Shout Series
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_A":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_B":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_C":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_D":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_E":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    #endregion
                    #region Thunderbolt Force Series
                    case "SKILL_CH_LIGHTNING_STORM_A":
                    case "SKILL_CH_LIGHTNING_STORM_B":
                    case "SKILL_CH_LIGHTNING_STORM_C":
                    case "SKILL_CH_LIGHTNING_STORM_D":
                        info.Instant = 2;
                        info.Distance = 6;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #endregion

                    #region Fire
                    #region Flame Wave Series
                    case "SKILL_CH_FIRE_GIGONGSUL_A":
                    case "SKILL_CH_FIRE_GIGONGSUL_B":
                    case "SKILL_CH_FIRE_GIGONGSUL_D":
                    case "SKILL_CH_FIRE_GIGONGSUL_E":
                        info.Instant = 2;
                        info.Distance = 6;
                        info.P_M = true;
                        break;
                    case "SKILL_CH_FIRE_GIGONGSUL_F":
                    case "SKILL_CH_FIRE_GIGONGSUL_C":
                        info.Instant = 2;
                        info.Distance = 6;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 6;
                        break;
                    #endregion
                    #endregion

                    #region Force
                    case "SKILL_CH_WATER_CURE_A":
                    case "SKILL_CH_WATER_CURE_B":
                    case "SKILL_CH_WATER_CURE_C":
                    case "SKILL_CH_WATER_CURE_D":
                    case "SKILL_CH_WATER_CURE_E":
                    case "SKILL_CH_WATER_CURE_F":
                    case "SKILL_CH_WATER_HEAL_A":
                    case "SKILL_CH_WATER_HEAL_B":
                    case "SKILL_CH_WATER_HEAL_C":
                    case "SKILL_CH_WATER_HEAL_D":
                    case "SKILL_CH_WATER_HEAL_E":
                    case "SKILL_CH_WATER_HEAL_F":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr") info.canUse = false;

                        break;
                    case "SKILL_CH_WATER_CANCEL_A":
                    case "SKILL_CH_WATER_CANCEL_B":
                    case "SKILL_CH_WATER_CANCEL_C":
                    case "SKILL_CH_WATER_CANCEL_D":
                    case "SKILL_CH_WATER_CANCEL_E":
                    case "SKILL_CH_WATER_CANCEL_F":
                    case "SKILL_CH_WATER_CANCEL_G":
                    case "SKILL_CH_WATER_CANCEL_H":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr") info.canUse = false;

                        break;
                    case "SKILL_CH_WATER_RESURRECTION_A":
                    case "SKILL_CH_WATER_RESURRECTION_B":
                    case "SKILL_CH_WATER_RESURRECTION_C":
                    case "SKILL_CH_WATER_RESURRECTION_D":
                    case "SKILL_CH_WATER_RESURRECTION_E":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "CLGameServer.PlayerMgr") info.canUse = false;

                        break;
                    #endregion

                    #region Europe
                    #region Europe Wizard
                    #region Earth
                    case "SKILL_EU_WIZARD_EARTHA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_POINT_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_AREA_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_AREA_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Cold
                    case "SKILL_EU_WIZARD_COLDA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_POINT_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_AREA_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_AREA_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Fire
                    case "SKILL_EU_WIZARD_FIREA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_SPRAY_A":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_POINT_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_SPRAY_B":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Light
                    case "SKILL_EU_WIZARD_PSYCHICA_LIGHT_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 2;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_AREA_A":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_LIGHT_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_AREA_B":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #endregion
                    #region Europe Warrior
                    #region Axe
                    case "SKILL_EU_WARRIOR_DUALA_CROSS_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_TWIST_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_STUN_A":
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_COUNTER_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_WHIRLWIND_A":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_TWIST_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_COUNTER_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_WHIRLWIND_B":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Two-Handed
                    case "SKILL_EU_WARRIOR_TWOHANDA_DASH_A":
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_RISING_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CHARGE_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CRY_A":
                        info.Targethits = 3;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CHARGE_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CRY_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region One-Handed
                    case "SKILL_EU_WARRIOR_ONEHANDA_STRIKE_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_SHIELD_A":
                        info.Instant = 0;
                        info.Targethits = 3;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_PIERCE_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_CRITICAL_A":
                        info.Instant = 0;
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_SHIELD_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Targethits = 3;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_PIERCE_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_CRITICAL_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Warrior Others
                    case "SKILL_EU_WARRIOR_FRENZYA_TOUNT_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Distance = 15;
                        info.Targethits = 3;
                        break;
                    case "SKILL_EU_WARRIOR_FRENZYA_TOUNT_SPRINT_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Distance = 15;
                        break;
                    #endregion
                    #endregion
                    #region Europe Rogue
                    #region Rogue
                    case "SKILL_EU_ROG_BOWA_FAST_A":
                        info.Instant = 0;
                        break;
                    case "SKILL_EU_ROG_BOWA_FAST_B":
                        info.Instant = 0;
                        break;
                    case "SKILL_EU_ROG_BOWA_POWER_A":
                    case "SKILL_EU_ROG_BOWA_POWER_B":
                    case "SKILL_EU_ROG_BOWA_RANGE_A":
                    case "SKILL_EU_ROG_BOWA_RANGE_B":
                    case "SKILL_EU_ROG_BOWA_KNOCK_A":
                    case "SKILL_EU_ROG_BOWA_KNOCK_B":
                        info.Instant = 1;
                        break;
                    #endregion
                    #region Dagger
                    case "SKILL_EU_ROG_DAGGERA_CHAIN_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_WOUND_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SCREW_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SLASH_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_STEALTHA_ATTACK_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_WOUND_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SLASH_B":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion
                    #endregion
                    default:
                        //Set default for skills that havent been added
                        info.Targethits = info.NumberOfAttack;
                        info.Tdistance = 0;
                        info.Instant = 2;
                        info.P_M = false;
                        info.canUse = true;
                        //Write info if skill not added
                        Log.Exception("Skill System Not Found Number of attacks : " + info.NumberOfAttack + " Skillname: " + ObjData.Manager.SkillBase[SkillID].Series);
                        break;
                }
                //Return information
                return info;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return info;
        }
        #endregion
        #region Number of attacks
        public static byte NumberAttack(int SkillID, ref int[] ID)
        {
            //Set default values information
            byte NumberAttack = 0;
            int Next1 = ObjData.Manager.SkillBase[SkillID].NextSkill;
            ID[NumberAttack] = SkillID;
            NumberAttack++;
            //Wrap our function inside a catcher
            try
            {
                //If next isnt null
                if (Next1 != 0)
                {
                    //Set id + number of attack +
                    ID[NumberAttack] = Next1;
                    NumberAttack++;
                    while (Next1 != 0)
                    {
                        if (ObjData.Manager.SkillBase[Next1].NextSkill != 0)
                        {
                            ID[NumberAttack] = ObjData.Manager.SkillBase[Next1].NextSkill;
                            NumberAttack++;
                            Next1 = ObjData.Manager.SkillBase[Next1].NextSkill;
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return NumberAttack;
        }
        #endregion
        #region Check weapon
        public static bool CheckWeapon(int itemid, int skillid)
        {
            //Wrap our function inside a catcher
            try
            {

                if (ObjData.Manager.SkillBase[skillid].Weapon1 == 255)
                    return true;
                byte[] weapons = new byte[2];

                weapons[0] = ObjData.Manager.SkillBase[skillid].Weapon1;
                weapons[1] = ObjData.Manager.SkillBase[skillid].Weapon2;

                if (weapons[1] == 255)
                    weapons[1] = 6;

                foreach (byte b in weapons)
                {
                    if (b == ObjData.Manager.ItemBase[itemid].TypeID4)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        #endregion
    }
}
