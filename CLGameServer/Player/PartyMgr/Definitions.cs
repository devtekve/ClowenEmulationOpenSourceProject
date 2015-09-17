namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public enum PartyTypes
        {
            NONSHARE_NO_PERMISSION,
            EXPSHARE_NO_PERMISSION,
            ITEMSHARE_NO_PERMISSION,
            FULLSHARE_NO_PERMISSION,
            NONSHARE,
            EXPSHARE,
            ITEMSHARE,
            FULLSHARE
        };

        public enum PartyPurpose
        {
            HUNTING,
            QUEST,
            TRADE,
            THIEF
        };
    }
    
}
