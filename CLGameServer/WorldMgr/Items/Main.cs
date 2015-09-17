namespace CLGameServer.WorldMgr
{
    public partial class Items
    {
        public int Model, amount = 1;
        public int UniqueID, fromOwner, Owner;
        public float x, z, y;
        public short Angle;
        public byte xSec, ySec, PlusValue, Type, fromType, PhyAtt, PhyDef, MagAtt, MagDef, BlockRatio, AttackRating;
        public System.Collections.Generic.List<ObjData.MagicOption> blues = new System.Collections.Generic.List<ObjData.MagicOption>(500);
        public bool downType;
        public CLFramework.GenerateUniqueID Ids;
        public System.Collections.Generic.List<int> Spawn = new System.Collections.Generic.List<int>();
    }
}
