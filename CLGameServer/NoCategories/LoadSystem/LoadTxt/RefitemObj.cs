using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldMgr.MonstersData
{
	
	class RefObject
	{
		public byte Service; //bool -> Indicates whether object is used or not.
		public int ID; //in packet reference described as RefObjID
		public string CodeName;
		public string ObjName; //Korean -> Localize by NameStrID
		public string OrgObjCodeName; //reference codeName to original object used by clones
		public string NameStrID; //reference for ObjName localization (SN_CODENAME)
		public string DescStrID; //references for Description localization (CODENAME_TT_DESC) 
		public byte CashItem; //bool -> Indicates whether object belongs to Item Mall or not
		public byte Bionic; //bool
		public byte TypeID1;
		public byte TypeID2;
		public byte TypeID3;
		public byte TypeID4;
		public int DecayTime; //time in milliseconds until object despawns
		public ObjectCountry Country; //Indicates where object is from
		public ObjectRarity Rarity;
		public byte CanTrade; //bool
		public byte CanSell; //bool
		public byte CanBuy; //bool
		public ObjectBorrowType CanBorrow; //link to ObjectBorrowType
		public ObjectDropType CanDrop; //link to ObjectDropType
		public byte CanPick; //bool
		public byte CanRepair; //bool
		public byte CanRevive; //bool
		public ObjectUseType CanUse; //link to ObjectUseType
		public byte CanThrow; //bool -> only ITEM_FORT_SHOCK_BOMB
		public int Price;
		public int CostRepair;
		public int CostRevive;
		public int CostBorrow;
		public int KeepingFee; //Storage cost
		public int SellPrice;
		public ObjectReqLevelType ReqLevelType1;
		public byte ReqLevel1;
		public ObjectReqLevelType ReqLevelType2;
		public byte ReqLevel2;
		public ObjectReqLevelType ReqLevelType3;
		public byte ReqLevel3;
		public ObjectReqLevelType ReqLevelType4;
		public byte ReqLevel4;
		public int MaxContain;
		public short RegionID; //for "STORE_" objects
		public short Dir; //unused
		public short OffsetZ; //for "STORE_" objects
		public short OffsetX; //for "STORE_" objects
		public short OffsetY; //for "STORE_" objects
		public short Speed1; //WalkSpeed
		public short Speed2; //RunSpeed
		public int Scale;
		public short BCHeight; //for object selection
		public short BCRadius; //for object selection
		public int EventID;
		public string AssocFileObj;
		public string AssocFileDrop;
		public string AssocFileIcon;
		public string AssocFile1;
		public string AssocFile2;
		public int MaxStack;
		public ItemGender ReqGender;
		public int ReqStr;
		public int ReqInt;
		public byte ItemClass; //Degree = ROUND(ItemClass / 3, UP)
		public int SetID;
		public float Dur_L; //Durability - lower bound
		public float Dur_U; //Durability - upper bound
		public float PD_L; //Physical defense - lower bound
		public float PD_U; //Physical defense - upper bound
		public float PDInc; //Physical defense - increase
		public float ER_L; //Evasion rate (Parry rate) - lower bound
		public float ER_U; //Evasion rate (Parry rate) - upper bound
		public float ERInc; //Evasion rate (Parry rate) - increase
		public float PAR_L; //Physical absorb rate - lower bound
		public float PAR_U; //Physical absorb rate - upper bound
		public float PARInc; //Physical absorb rate - increase
		public float BR_L; //Block rate - lower bound
		public float BR_U; //Block rate - upper bound
		public float MD_L; //Magical defense - lower bound
		public float MD_U; //Magical defense - upper bound
		public float MDInc; //Magical defense - increase
		public float MAR_L; //Magical absorb rate - lower bound
		public float MAR_U; //Magical absorb rate - upper bound
		public float MARInc; //Magical absorb rate - increase
		public float PDStr_L; //Physical (defense) reinforce - lower bound
		public float PDStr_U; //Physical (defense) reinforce - upper bound
		public float MDInt_L; //Magical (defense) reinforce - lower bound
		public float MDInt_U; //Magical (defense) reinforce - upper bound
		public byte Quivered;
		public byte Ammo1_TID4;
		public byte Ammo2_TID4;
		public byte Ammo3_TID4;
		public byte Ammo4_TID4;
		public byte Ammo5_TID4;
		public byte SpeedClass; //Seems to be ether 2 for weapons or 0 for everything else
		public byte TwoHanded;
		public short Range;
		public float PAttackMin_L; //Physical attack power (minimum) - lower bound
		public float PAttackMin_U; //Physical attack power (minimum) - upper bound
		public float PAttackMax_L; //Physical attack power (maximum) - lower bound
		public float PAttackMax_U; //Physical attack power (maximum) - upper bound
		public float PAttackInc; //Physical attack power - increase
		public float MAttackMin_L; //Physical attack power (minimum) - lower bound
		public float MAttackMin_U; //Physical attack power (minimum) - upper bound
		public float MAttackMax_L; //Physical attack power (maximum) - lower bound
		public float MAttackMax_U; //Physical attack power (maximum) - upper bound
		public float MAttackInc; //Physical attack power - increase
		public float PAStrMin_L; //Physical (attack) reinforce (minimum) - lower bound
		public float PAStrMin_U; //Physical (attack) reinforce (minimum) - upper bound
		public float PAStrMax_L; //Physical (attack) reinforce (maximum) - lower bound
		public float PAStrMax_U; //Physical (attack) reinforce (maximum) - upper bound
		public float MAInt_Min_L; //Magical (attack) reinforce (minimum) - lower bound
		public float MAInt_Min_U; //Magical (attack) reinforce (minimum) - upper bound
		public float MAInt_Max_L; //Magical (attack) reinforce (maximum) - lower bound
		public float MAInt_Max_U; //Magical (attack) reinforce (maximum) - upper bound
		public float HR_L; //Hit Rate (Attack rate) - lower bound
		public float HR_U; //Hit Rate (Attack rate) - upper bound
		public float HRInc; //Hit Rate (Attack rate) - increase
		public float CHR_L; //Critical - lower bound
		public float CHR_U; //Critical - upper bound
		public Dictionary<int,string> Params = new Dictionary<int,string>(); //List of 20 params
		public byte MaxMagicOptCount;
		public byte ChildItemCount;
	}
}
