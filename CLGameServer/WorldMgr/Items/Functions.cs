using CLFramework;
using System;

namespace CLGameServer.WorldMgr
{
    public partial class Items
    {
        public void Send(byte[] buff, bool b)
        {
            try
            {
                if (b && Model != 0)
                {
                    lock (Helpers.Manager.clients)
                    {
                        for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                        {
                            if (!Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                if (Helpers.Manager.clients[i].Character.Position.x >= (x - 50) && Helpers.Manager.clients[i].Character.Position.x <= ((x - 50) + 100) && Helpers.Manager.clients[i].Character.Position.y >= (y - 50) && Helpers.Manager.clients[i].Character.Position.y <= ((y - 50) + 100))
                                {
                                    Spawn.Add(Helpers.Manager.clients[i].Character.Information.UniqueID);
                                    Helpers.Manager.clients[i].client.Send(buff);
                                }
                            }
                        }
                    }

                    StartDeleteTimer(Rnd.Next(500, 1000) * 10);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                        {
                            Helpers.Manager.clients[i].client.Send(buff);
                        }
                    }
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
                byte[] buff = Client.Packet.ObjectDeSpawn(UniqueID);
                lock (Helpers.Manager.clients)
                {
                    for (int i = 0; i < Helpers.Manager.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Helpers.Manager.clients[i].Character.Information.UniqueID))
                            {
                                Helpers.Manager.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                }

                Spawn.Clear();
            }
            catch (Exception ex)
            {

                Log.Exception(ex);
            }
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate (int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public int FoundPosition()
        {
            int FoundCount = 0;
            for (int i = 0; i < Helpers.Manager.WorldItem.Count; i++)
            {
                if (Helpers.Manager.WorldItem[i] == null)
                {
                    Console.WriteLine("Null Değer Bulundu:{0}", i);
                    continue;
                }
                if (Helpers.Manager.WorldItem[i].x == x &&
                    Helpers.Manager.WorldItem[i].y == y)
                {
                    FoundCount++;
                }
            }
            return FoundCount;
        }
        public void CalculateNewPosition()
        {
            //int FoundCount = FoundPosition();
            int CAngle = 0;
            /* if (FoundCount >= 0)
             {*/
            int AngleIndex = Rnd.Next(0, 360);
            float INC = Rnd.Next(0, 3) * (float)Rnd.NextDouble(),
                  MinRadius = 0.85f,
                  MaxRadius = 8.85f;
            float AddX = (INC * ObjData.Manager.AngleCos[AngleIndex]);
            float AddY = (INC * ObjData.Manager.AngleSin[AngleIndex]);
            double distance = Formule.gamedistance(x + AddX, y + AddY, x, y);
            if (distance >= MinRadius && distance <= MaxRadius)
            {
                x += AddX;
                y += AddY;
                CAngle = Convert.ToInt32(Math.Atan2(x, y) * 10430.38208);
                if (CAngle >= 32000)
                {
                    Log.Exception("Angle Hatası:" + CAngle);
                }
                else
                {
                    Angle = (short)CAngle;
                }
            }
            else
            {
                // Recalc
                CalculateNewPosition();
            }

            // CalculateNewPosition();
            /* }
             else
             {
                 // İtem'in şimdiki pozisyonu iyi.
                 // TODO:Bu algoritmanın geliştirilmesi gerekiyor.
             }*/
        }
        public void Dispose()
        {
            // this gc uses a lots of resource ( really lot )
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
