using System;
using System.Collections.Generic;
using CLFramework;

namespace CLGameServer.Helpers
{
    class Manager
    {
        public static aList<PlayerMgr> clients = new aList<PlayerMgr>();
        public static List<WorldMgr.Monsters> Objects = new List<WorldMgr.Monsters>(13000);
        public static List<WorldMgr.Items> WorldItem = new List<WorldMgr.Items>();
        public static List<WorldMgr.party> Party = new List<WorldMgr.party>();
        public static List<WorldMgr.pet_obj> HelperObject = new List<WorldMgr.pet_obj>();
        public static List<WorldMgr.spez_obj> SpecialObjects = new List<WorldMgr.spez_obj>();
        public static int maxSlots;
        public static Servers.IPCServer IPC;
        public static DateTime ServerStartedTime;

        public static int GetOnlineClientCount
        {
            get
            {
                return clients.Count;
            }
        }
    }
}
