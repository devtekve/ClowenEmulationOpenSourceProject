using CLFramework;
using System;
using System.Collections.Generic;

namespace Function
{
    public class Items
    {
        public static void PrivateItemPacket(PacketWriter Writer, int id, byte max, byte avatar, bool mspawn)
        {
            try
            {
                List<byte> slots = new List<byte>();
                DB ms = new DB("SELECT * FROM char_items WHERE owner='" + id + "' AND slot >= '0' AND slot <= '" + max + "' AND inavatar='" + avatar + "' AND storagetype='0' AND quantity='1'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    int count = ms.Count();

                    Writer.Byte(count);

                    while (reader.Read())
                    {
                        byte slot = reader.GetByte(5);
                        byte plusvalue = reader.GetByte(4);
                        int itemid = reader.GetInt32(2);

                        if (itemid != 0)
                        {
                            if (!slots.Exists(delegate (byte bk)
                            {
                                return bk == slot;
                            }))
                            {
                                slots.Add(slot);
                            }
                            Writer.DWord(itemid);
                            Writer.Byte(plusvalue);
                        }
                    }
                }
                ms.Close();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }


        public static byte RandomPlusValue()
        {
            int RandomNumber = Rnd.Next(0, 200);
            //TODO: Calculate random values on chance / ticket / rates / add more values
            if (RandomNumber == 200) return 8;
            else if (RandomNumber == 180) return 7;
            else if (RandomNumber == 120) return 6;
            else if (RandomNumber <= 110 && RandomNumber >= 105) return 5;
            else if (RandomNumber <= 77 && RandomNumber >= 72) return 4;
            else if (RandomNumber <= 74 && RandomNumber >= 68) return 3;
            else if (RandomNumber <= 67 && RandomNumber >= 60) return 2;
            else if (RandomNumber <= 50 && RandomNumber >= 35) return 1;
            return 0;
        }

        public static byte RandomStatValue()
        {
            int RandomNumber = Rnd.Next(0, 200);
            Random rnd = new Random();
            //TODO: Calculate random values on chance / ticket / rates / add more values
            if (RandomNumber == 200) return Convert.ToByte(rnd.Next(80, 100));
            else if (RandomNumber == 180) return Convert.ToByte(rnd.Next(70, 80));
            else if (RandomNumber == 120) return Convert.ToByte(rnd.Next(60, 70));
            else if (RandomNumber <= 110 && RandomNumber >= 105) return Convert.ToByte(rnd.Next(50, 60));
            else if (RandomNumber <= 100 && RandomNumber >= 62) return Convert.ToByte(rnd.Next(40, 50));
            else if (RandomNumber <= 120 && RandomNumber >= 48) return Convert.ToByte(rnd.Next(20, 30));
            else if (RandomNumber <= 150 && RandomNumber >= 30) return Convert.ToByte(rnd.Next(10, 20));
            else if (RandomNumber <= 200 && RandomNumber >= 5) return Convert.ToByte(rnd.Next(1, 10));
            return 0;
        }
    }
}
