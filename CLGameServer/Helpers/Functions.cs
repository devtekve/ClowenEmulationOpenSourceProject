using CLFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLGameServer.Helpers
{
    public class Functions
    {

        

        public static bool cRound(bool[] b)
        {
            try
            {
                foreach (bool bol in b)
                {
                    if (!bol) return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        public static bool aRound(ref bool[] b, ref float x, ref float y)
        {
            try
            {
                if (!b[0])
                {
                    x -= 1.5f;
                    y -= 1.8f;
                    b[0] = true;
                    return false;
                }
                else if (!b[1])
                {
                    y -= 3;
                    b[1] = true;
                    return false;
                }
                else if (!b[2])
                {
                    x += 1.8f;
                    y -= 1.5f;
                    b[2] = true;
                    return false;
                }
                else if (!b[3])
                {
                    x += 3;
                    b[3] = true;
                    return false;

                }
                else if (!b[4])
                {
                    x -= 3;
                    b[4] = true;
                    return false;
                }
                else if (!b[5])
                {
                    x -= 1.2f;
                    y += 1.8f;
                    b[5] = true;
                    return false;

                }
                else if (!b[6])
                {
                    y += 3;
                    b[6] = true;
                    return false;
                }
                else if (!b[7])
                {
                    x += 1.5f;
                    y += 1.5f;
                    b[7] = true;
                    return false;

                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return true;
        }
        public static void aRound2(ref float x, ref float y, int oran)
        {
            try
            {
                switch (Rnd.Next(0, 10))
                {
                    case 0:
                        x -= 1.5f * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        y -= 1.8f * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 1:
                        y -= 3 * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 2:
                        x += 1.8f * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        y -= 1.5f * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 3:
                        x += 3 * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        break;
                    case 4:
                        x -= 3 * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        break;
                    case 5:
                        x -= 1.2f * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        y += 1.8f * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 6:
                        y += 3 * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 7:
                        x += 1.5f * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        y += 1.5f * oran;
                        break;
                    case 8:
                        x += 2f * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        y += 1.5f * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                    case 9:
                        x -= 2 * oran + ObjData.Manager.AngleCos[Rnd.Next(0, 360)];
                        break;
                    case 10:
                        y -= 2 * oran + ObjData.Manager.AngleSin[Rnd.Next(0, 360)];
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void aRound(ref float x, ref float y, int oran)
        {
            try
            {
                switch (Rnd.Next(0, 11))
                {
                    case 0:
                        x -= 1.5f * oran;
                        y -= 1.8f * oran;
                        break;
                    case 1:
                        y -= 3 * oran;
                        break;
                    case 2:
                        x += 1.8f * oran;
                        y -= 1.5f * oran;
                        break;
                    case 3:
                        x += 3 * oran;
                        break;
                    case 4:
                        x -= 3 * oran;
                        break;
                    case 5:
                        x -= 1.2f * oran;
                        y += 1.8f * oran;
                        break;
                    case 6:
                        y += 3 * oran;
                        break;
                    case 7:
                        x += 1.5f * oran;
                        y += 1.5f * oran;
                        break;
                    case 8:
                        x += 2f * oran;
                        y += 1.5f * oran;
                        break;
                    case 9:
                        x -= 2 * oran;
                        break;
                    case 10:
                        y -= 2 * oran;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static byte RandomType(byte level, ref byte Kat, bool party, ref byte Agresif)
        {
            try
            {
                int rnd = Rnd.Next(0, 100);
                if (rnd > 60 && rnd < 79)//champion
                {
                    Kat = 2;
                    Agresif = 1;
                    if (rnd > 20 && rnd < 55) if (party) { Kat = 20; return 17; } //partymob
                    return 1;
                }
                else if (rnd > 80 && level > 14)//giant
                {
                    Kat = 20;
                    Agresif = 1;
                    return 4;
                }
                else if (rnd < 60)//general
                {
                    Kat = 1;
                    if (rnd > 20 && rnd < 55) if (party) return 16; //partymob
                    return 0;
                }
                else if (rnd > 30 && rnd < 45 && level >= 14)//giant
                {
                    Kat = 20;
                    Agresif = 1;
                    if (rnd > 32 && rnd < 39)
                        if (party)
                        {
                            Kat = 210; return 20; //partymob
                        }
                    return 4;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return 0;
        }
        public static int[] GetEliteIds(int ID)
        {
            try
            {
                int[] mid = new int[10];

                switch (ID)
                {
                    case 1954: // TIGER GIRL
                        mid[0] = 1953;
                        mid[1] = 1952;
                        mid[2] = 1947;
                        mid[3] = 1953;
                        mid[4] = 1952;
                        mid[5] = 1947;
                        mid[6] = 1953;
                        mid[7] = 1952;
                        mid[8] = 1947;
                        break;
                    case 5871: // CERBERUS
                        mid[0] = 5870;
                        mid[1] = 5868;
                        mid[2] = 5865;
                        mid[3] = 5866;
                        mid[4] = 5870;
                        mid[5] = 5868;
                        mid[6] = 5865;
                        mid[7] = 5866;
                        mid[8] = 5866;
                        break;
                    case 1982: //Urichi
                        mid[0] = 1980;
                        mid[1] = 1980;
                        mid[2] = 1981;
                        mid[3] = 1981;
                        mid[4] = 1980;
                        mid[5] = 1981;
                        break;
                    case 2002: //ISYTARU
                        mid[0] = 1995;
                        mid[1] = 2118;
                        mid[2] = 1996;
                        mid[3] = 2125;
                        mid[4] = 1996;
                        mid[5] = 2125;
                        break;

                    case 3810: // LORD YARKAN
                        mid[0] = 3802;
                        mid[1] = 3803;
                        mid[2] = 3804;
                        mid[3] = 3805;
                        mid[4] = 3806;
                        mid[5] = 3807;
                        mid[6] = 3808;
                        mid[7] = 3809;
                        break;

                    case 3875: // DEMON SHATİAN
                        mid[0] = 3874;
                        mid[1] = 3874;
                        mid[2] = 3872;
                        mid[3] = 3873;
                        mid[4] = 3874;
                        mid[5] = 3874;
                        mid[6] = 3872;
                        mid[7] = 3873;
                        break;
                }
                return mid;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return null;
        }
    }
}
