using CLFramework;
namespace CLGameServer.Client
{
    public partial class Packet
    {
        public static byte[] Movement(ObjData.vektor p)
        {
           PacketWriter Writer = new PacketWriter();
                Writer.Create(OperationCode.SERVER_MOVEMENT);     //Select opcode
                Writer.DWord(p.ID);                         //Player ID
                Writer.Bool(true);                          //Bool 1
                Writer.Byte(p.xSec);                        //Player X Sector
                Writer.Byte(p.ySec);                        //Player Y Sector
                if (!FileDB.CheckCave(p.xSec, p.ySec))
                {
                Writer.Word(p.x);                    //Player X Location
                Writer.Word(p.z);                    //Player Z Location
                Writer.Word(p.y);                    //Player Y Location
                }
                else
                {
                    if (p.x < 0)
                    {
                        Writer.Word(p.x);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(p.x);
                    }
                    Writer.DWord(p.z);

                    if (p.y < 0)
                    {
                        Writer.Word(p.y);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(p.y);
                    }
                }
                Writer.Bool(false);
                /* ReSearch this is ext packets
				Writer.Byte(p.xSec);                        //Player X Sector
                Writer.Byte(p.ySec);                        //Player Y Sector
                Writer.Word(p.x);                    //Player X Location
                Writer.DWord(p.z);                    //Player Z Location
                Writer.Word(p.y);         */           //Player Y Location
                return Writer.GetBytes();
        }
        public static byte[] StopMovement(ObjData.vektor p)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_PICKUPITEM_MOVE);
            Writer.DWord(p.ID);
            Writer.Byte(p.xSec);
            Writer.Byte(p.ySec);
            Writer.Float(p.x);
            Writer.Float(p.z);
            Writer.Float(p.y);
            Writer.Word(0/*p.Angle*/); // Angle
            return Writer.GetBytes();
        }
       public static byte[] Angle(int UniqueID, ushort angle)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_ANGLE);    
            Writer.DWord(UniqueID);                       
            Writer.Word(angle);                     
            return Writer.GetBytes();
        }
    }
}
