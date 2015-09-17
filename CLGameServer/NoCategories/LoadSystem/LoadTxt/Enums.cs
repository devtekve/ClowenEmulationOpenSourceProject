namespace WorldMgr.MonstersData
{
    public enum ItemGender : byte
    {
        Female = 0,
        Male = 1,
        Sexless = 2 //Neutral
    }
    public enum ObjectUseType : byte
    {
        //Bit 1: AskBeforeUsing
        //Bit 8: CanBeUsed
        ClassA = 0,   //0 000000 0
        ClassB = 1,   //0 000000 1
        ClassC = 129, //1 000000 1 
        ClassD = 255, //1 111111 1
    }
    public enum ObjectReqLevelType : int
    {
        None = -1,
        Character = 1, //캐릭터          
        MasteryTrader = 2, //상인마스터리
        MasteryTief = 3, //도적마스터리
        MasteryHunter = 4, //헌터마스터리
        Pet2System = 5,
        GuildLevel = 10,
        //European clothing limitation
        MasteryWarrior = 513, //워리어201
        MasteryRogue = 515,  //로그203
        MasteryWizard = 514,  //위저드202        
        MasteryWarlock = 516, //워락204
        MasteryBard = 517,  //바드205
        MasteryCleric = 518, //클레릭206
    }
    public enum ObjectRarity : byte
    {
        ClassA = 0, //White
        ClassB = 1, //Blue
        ClassC = 2, //Yellow (SOX)
        ClassD = 3, //SET
        ClassE = 4, //
        ClassF = 5, //
        ClassG = 6, //ROC SET       
        ClassH = 7, //LEGEND
        ClassI = 8 //
                   //For ITEM_: see above, this rarity is also used in SERVER_AGENT_OBJECT_SPAWN when OBJECT equals ITEM...
                   //For COS_T / TRADE_COS: it might be used for TIEF/HUNTER AI target priority since behemoth and lvl60+ cos are is higher than normal ones
                   //For MOB_: it could affect on spawn chance unless its a unique?
    }
    public enum ObjectDropType : byte
    {
        ClassA = 0, //000000 0 0 -> Can't drop
        ClassB = 1, //000000 0 1 -> Do not ask before drop?
        ClassC = 2, //000000 1 0 -> Unseen but this is the 'ask before drop' bit?
        ClassD = 3, //000000 1 1 -> Ask then drop
    }
    public enum ObjectCountry : byte
    {
        Chinese = 0,
        Europe = 1,
        Islam = 2, //NOT IMPLEMENTED YET
        Unassigned = 3 //Does not belong to any country
    }
    public enum ObjectBorrowType : byte
    {
        //Bit 1: EXCHANGE ?
        //Bit 2: STORAGE / GUILD_STORAGE ?
        //Bit 3: PET2 ?
        //Bit 4-8: ???

        ClassA = 000, //0 0 0 00000
        ClassB = 128, //1 0 0 00000
        ClassC = 159, //1 0 0 11111
        ClassD = 160, //1 0 1 00000
        ClassE = 223, //1 1 0 11111
        ClassF = 192, //1 1 0 00000
        ClassG = 255, //1 1 1 11111
    }
}
