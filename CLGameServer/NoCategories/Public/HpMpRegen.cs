using System.Threading;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public Timer HPRegen;
        public Timer MPRegen;
       
        void HPregen(int time)
        {
            HPRegen = new Timer(new TimerCallback(delegate(object e) {
                if (!Character.State.Die && !Character.Action.sAttack && !Character.Action.sCasting && !Character.Action.nAttack && !Character.Spawning)
                {
                    double RegenHP = Character.Stat.Hp * 0.007; //also from that site.
                    if (Character.Blues.hpregen != 0)
                        RegenHP += RegenHP * (Character.Blues.hpregen / 100);

                    // HP regen
                    if (Character.Stat.SecondHp + (int)RegenHP < Character.Stat.Hp)
                    {
                        Character.Stat.SecondHp += (int)RegenHP;
                        UpdateHp();
                    }
                    else if (Character.Stat.SecondHp != Character.Stat.Hp)
                    {
                        Character.Stat.SecondHp += Character.Stat.Hp - Character.Stat.SecondHp;
                        UpdateHp();
                    }
                    else
                    {
                        StopHPRegen();
                    }
                    //SavePlayerHPMP();
                }
            }), 0, 0, time); //10 seconds retail
        }
        void MPregen(int time)
        {
            MPRegen = new Timer(new TimerCallback(delegate(object e) {
                if (!Character.State.Die && !Character.Action.sAttack && !Character.Action.sCasting && !Character.Action.nAttack && !Character.Spawning)
                {
                    double RegenMP = Character.Stat.Mp * 0.007; // 2% regen retail

                    if (Character.Blues.mpregen != 0)
                        RegenMP += RegenMP * (Character.Blues.mpregen / 100);

                    if (Character.Stat.SecondMP + (int)RegenMP < Character.Stat.Mp)
                    {
                        Character.Stat.SecondMP += (int)RegenMP;
                        UpdateMp();
                    }
                    else if (Character.Stat.SecondMP != Character.Stat.Mp)
                    {
                        Character.Stat.SecondMP += Character.Stat.Mp - Character.Stat.SecondMP;
                        UpdateMp();
                    }
                    else
                    {
                        StopMPRegen();
                    }
                    //SavePlayerHPMP();
                }
            }), 0, 0, time); //10 seconds retail
        }
        public void StopHPRegen()
        {
            if (HPRegen != null)
            {
                HPRegen.Dispose();
                HPRegen = null;
            }
        }
        public void StopMPRegen()
        {
            if (MPRegen != null)
            {
                MPRegen.Dispose();
                MPRegen = null;
            }
        }
    }
}
