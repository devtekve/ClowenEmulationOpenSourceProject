using System;
using System.Threading;

namespace CLGameServer.WorldMgr
{
    public partial class Items
    {
        public Timer DeleteTimer;
        void StartDeleteTimer(int time)
        {
            if (DeleteTimer == null) DeleteTimer = new Timer(new TimerCallback(DeleteTimerCB), 0, time, 0);
        }
        public void StopDeleteTimer()
        {
            if (DeleteTimer != null)
            {
                DeleteTimer.Dispose();
                DeleteTimer = null;
            }
        }
        protected void DeleteTimerCB(object e)
        {
            lock (this)
            {
                try
                {
                    if (this != null)
                    {
                        DeSpawnMe();
                        //RandomID.Delete(UniqueID);
                        StopDeleteTimer();
                        //Dispose();
                        Helpers.Manager.WorldItem.Remove(this);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    CLFramework.Log.Exception(ex);
                }
            }
        }
    }
}
