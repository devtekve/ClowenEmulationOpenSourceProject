using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /* Rewriting potion system shortly */
        void HandlePotion(byte type, int ItemID)
        {
            try
            {
                if (type == 1) // hp
                {
                    long Total = (Character.Stat.Hp * Character.Information.Level * (long)ObjData.Manager.ItemBase[ItemID].Use_Time) / HandlePotionLevel(Character.Stat.Hp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 2)//mp
                {
                    long Total = (Character.Stat.Mp * Character.Information.Level * (long)ObjData.Manager.ItemBase[ItemID].Use_Time2) / HandlePotionLevel(Character.Stat.Mp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 3) //hp %25
                {
                    long Total = (Character.Stat.Hp * 25) / 100;
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    Character.Information.Item.Potion[pslot] = 4;
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 4) //mp %25
                {
                    long Total = (Character.Stat.Mp * 25) / 100;
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    Character.Information.Item.Potion[pslot] = 4;
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 5)// Vigor potions
                {
                    long Total = (Character.Stat.Hp * Character.Information.Level * (long)ObjData.Manager.ItemBase[ItemID].Use_Time) / HandlePotionLevel(Character.Stat.Hp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }
        public static int HandlePotionLevel(int hp)
        {
            int hpinfo = hp;
            byte info = 0;

            while (hpinfo > 0)
            {
                info++;
                hpinfo /= 10;
            }
            int b = 10;
            for (int i = 1; i <= info; i++)
            {
                b *= 10;
            }

            return b;
        }
    }
}
