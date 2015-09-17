using CLFramework;

namespace CLGameServer.Client
{
	public partial class Packet
	{
		public static byte[] ObjectSpawn(WorldMgr.pet_obj o)
		{
			PacketWriter Writer = new PacketWriter();

			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			switch (o.Named)
			{
				///////////////////////////////////////////////////////////////////////////
				// Job transports
				///////////////////////////////////////////////////////////////////////////
				case 4:
					Writer.DWord(o.Model);                                      //Pet Model id
					Writer.DWord(o.UniqueID);                                   //Pet Unique id
					Writer.Byte(o.xSec);                                        //X sector
					Writer.Byte(o.ySec);                                        //Y sector
					Writer.Float(Formule.packetx((float)o.x, o.xSec)); //X
					Writer.Float(o.z);                                          //Z
					Writer.Float(Formule.packety((float)o.y, o.ySec)); //Y

					Writer.Word(0);                                             //Angle

					Writer.Byte(1);                                             //Walking state
					Writer.Byte(1);                                             //Static

					Writer.Byte(o.xSec);                                        //X sector
					Writer.Byte(o.ySec);                                        //Y sector

					Writer.Word(0);                                             //Static
					Writer.Word(0);                                             //
					Writer.Word(0);                                             //

					Writer.Word(1);
					Writer.Word(3);

					Writer.Byte(0);
					Writer.Float(o.Walk);                                       //Object walking
					Writer.Float(o.Run);                                        //Object running
					Writer.Float(o.Zerk);                                       //Object zerk

					Writer.Word(0);                                             //

					Writer.Text(o.OwnerName);

					Writer.Word(2);                                             //
					Writer.DWord(o.OwnerID);                                    //Owner unique id
					Writer.Byte(4);                                             //Static byte 4
					break;
				case 3:
					///////////////////////////////////////////////////////////////////////////
					// Attack pet main packet
					///////////////////////////////////////////////////////////////////////////
					Writer.DWord(o.Model);
					Writer.DWord(o.UniqueID);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);
					Writer.Float(Formule.packetx((float)o.x, o.xSec));
					Writer.Float(o.z);
					Writer.Float(Formule.packety((float)o.y, o.ySec));
					Writer.Word(0);//angle
					Writer.Byte(0);
					Writer.Byte(o.Level);//level
					Writer.Byte(0);
					Writer.Word(0);//angle
					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(0);
					Writer.Float(o.Walk);                                       //Object walking
					Writer.Float(o.Run);                                        //Object running
					Writer.Float(o.Zerk);                                       //Object zerk

					Writer.Byte(0);
					Writer.Byte(0);
					if (o.Named == 1)
						Writer.Text(o.Petname);
					else
						Writer.Word(0);
					Writer.Text(o.OwnerName);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.DWord(o.OwnerID);
					Writer.Byte(1);
					break;
				case 2:
					///////////////////////////////////////////////////////////////////////////
					// Grab pet main packet
					///////////////////////////////////////////////////////////////////////////
					Writer.DWord(o.Model);                                      //Pet Model id
					Writer.DWord(o.UniqueID);                                   //Pet Unique id
					Writer.Byte(o.xSec);                                        //X sector
					Writer.Byte(o.ySec);                                        //Y sector
					Writer.Float(Formule.packetx((float)o.x, o.xSec)); //X
					Writer.Float(o.z);                                          //Z
					Writer.Float(Formule.packety((float)o.y, o.ySec)); //Y

					Writer.Word(0xDC72);                                        //Angle

					Writer.Byte(0);                                             //Walking state
					Writer.Byte(1);                                             //Static
					Writer.Byte(0);                                             //Static
					Writer.Word(0xDC72);                                        //Angle

					Writer.Byte(1);                                             //Static
					Writer.Word(0);                                             //
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Float(o.Walk);                                       //Object walking
					Writer.Float(o.Run);                                        //Object running
					Writer.Float(o.Zerk);                                       //Object zerk
					Writer.Word(0);                                             //
					if (o.Petname != "No name")
						Writer.Text(o.Petname);
					else
						Writer.Word(0);
					Writer.Text(o.OwnerName);                                   //Pet owner name
					Writer.Byte(4);                                             //Static byte 4?
					Writer.DWord(o.OwnerID);                                    //Owner unique id
					Writer.Byte(1);                                             //Static byte 1

					///////////////////////////////////////////////////////////////////////////
					break;
				default:
					///////////////////////////////////////////////////////////////////////////
					// // Horse //
					///////////////////////////////////////////////////////////////////////////
					Writer.DWord(o.Model);
					Writer.DWord(o.UniqueID);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);
					Writer.Float(Formule.packetx((float)o.x, o.xSec));
					Writer.Float(o.z);
					Writer.Float(Formule.packety((float)o.y, o.ySec));

					Writer.Word(0);
					Writer.Byte(0);
					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Word(0);
					Writer.Byte(1);
					Writer.Word(0);
					Writer.Byte(0);

					Writer.Float(o.Speed1);
					Writer.Float(o.Speed2);
					Writer.Float(o.Zerk);
					Writer.Word(0);
					Writer.Byte(1);
					///////////////////////////////////////////////////////////////////////////
					break;
			}
			return Writer.GetBytes();
		}
		public static byte[] ObjectSpawn(WorldMgr.spez_obj so)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			Writer.DWord(0xFFFFFFFF);                                           //Static
			Writer.DWord(so.spezType);                                           //Type
			Writer.DWord(so.ID);                                                //skillid
			Writer.DWord(so.UniqueID);                                          //UniqueID of spawn
			Writer.Byte(so.xSec);                                               //XSec
			Writer.Byte(so.ySec);                                               //Ysec
			Writer.Float(Formule.packetx((float)so.x, so.xSec));       //X
			Writer.Float(so.z);                                                 //Z
			Writer.Float(Formule.packety((float)so.y, so.ySec));       //Y
			Writer.Word(0);                                                     //Angle
			Writer.Byte(1);                                                     //Static
			return Writer.GetBytes();
		}
		public static byte[] ObjectSpawn(WorldMgr.Monsters o)//Monster spawns
		{

			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			Writer.DWord(o.ID);
			Writer.DWord(o.UniqueID);
			Writer.Byte(o.xSec);
			Writer.Byte(o.ySec);
			Writer.Float(Formule.packetx((float)o.x, o.xSec));
			Writer.Float(o.z);
			Writer.Float(Formule.packety((float)o.y, o.ySec));

			switch (o.LocalType)
			{
				case 0:

					Writer.Word(0);
					Writer.Word(1);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);

					if (!FileDB.CheckCave(o.xSec, o.ySec))
					{
						if (o.xSec == 0)
							Writer.Word(0);
						else
							Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
						Writer.Word(o.z);
						if (o.ySec == 0)
							Writer.Word(0);
						else
							Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
					}
					else
					{
						if (o.x < 0)
						{
							Writer.Word(Formule.cavepacketx((float)(o.x + o.wx)));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.cavepacketx((float)(o.x + o.wx)));
						}

						Writer.DWord(o.z);

						if (o.y < 0)
						{
							Writer.Word(Formule.cavepackety((float)(o.y + o.wy)));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.cavepackety((float)(o.y + o.wy)));
						}
					}

					Writer.Byte(1);
					Writer.Byte(o.Runing == true ? 2 : 0);
					Writer.Byte(0);
					Writer.Float(o.WalkingSpeed);// Walk speed
					Writer.Float(o.RunningSpeed);// Run speed
					Writer.Float(o.BerserkerSpeed);// Berserk speed

					Writer.Byte(0);//new ?

					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(1);
					Writer.Byte(5);
					Writer.Byte(o.Type);
					break;

				case 1:
					Writer.Word(o.Angle);
					Writer.Word(1);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);

					if (!FileDB.CheckCave(o.xSec, o.ySec))
					{
						Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
						Writer.Word(o.z);
						Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
					}
					else
					{
						if (o.x < 0)
						{
							Writer.Word(Formule.cavepacketx((float)(o.x + o.wx)));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.cavepacketx((float)(o.x + o.wx)));
						}

						Writer.DWord(o.z);

						if (o.y < 0)
						{
							Writer.Word(Formule.cavepackety((float)(o.y + o.wy)));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.cavepackety((float)(o.y + o.wy)));
						}
					}

					Writer.Byte(1);
					Writer.Byte(o.Runing == true ? 2 : 0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Float(o.WalkingSpeed);// Walk speed
					Writer.Float(o.RunningSpeed);// Run speed
					Writer.Float(o.BerserkerSpeed);// Berserk speed
					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(1);
					Writer.Byte(5);
					Writer.Byte(o.Type);
					Writer.Byte(4);
					break;
				case 2:
					Writer.Word(o.Angle); // Should be angle? yet not changing ingame..
					Writer.Byte(0);
					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Word(o.Angle); // Should be angle? yet not changing ingame..
					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.DWord(0);
					Writer.DWord(0);
					Writer.Float(100);
					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(0);
					Writer.Byte(2);
					break;

				case 3:
					Writer.Word(0);
					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(1);
					Writer.DWord(0);
					Writer.DWord(0);
					Writer.Byte(0);
					break;
				case 4:
					/*Writer.Word(0); // Angle
				   Writer.Byte(0);
				   Writer.Byte(1);
				   Writer.Byte(0);
				   Writer.Word(0); // Angle
				   Writer.Byte(1);
				   Writer.Byte(0);
				   Writer.Byte(0);
				   Writer.Byte(0);
				   Writer.Float(o.WalkingSpeed);// Walk speed
				   Writer.Float(o.RunningSpeed);// Run speed
				   Writer.Float(o.BerserkerSpeed);// Berserk speed
				   Writer.Byte(0);
				   Writer.Byte(2);
				   Writer.Byte(1);
				   Writer.Byte(5);
				   Writer.Byte(0);
				   Writer.Byte(34);
				   Writer.Byte(1);
				  * */
					// Thiefs and trader spawns
					Writer.Word(0); // Angle
					Writer.Word(1);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);

					if (!FileDB.CheckCave(o.xSec, o.ySec))
					{
						Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
						Writer.Word(o.z);
						Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
					}
					else
					{
						if (o.x < 0)
						{
							Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.packetx((float)(o.x + o.wx), o.xSec));
						}

						Writer.DWord(o.z);

						if (o.y < 0)
						{
							Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.packety((float)(o.y + o.wy), o.ySec));
						}
					}

					Writer.Byte(1);
					Writer.Byte(o.Runing == true ? 2 : 0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Float(o.WalkingSpeed);// Walk speed
					Writer.Float(o.RunningSpeed);// Run speed
					Writer.Float(o.BerserkerSpeed);// Berserk speed
					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(1);
					Writer.Byte(5);
					Writer.Byte(0);
					Writer.Byte(34); // 227 Need to check what this is...
					Writer.Byte(1);

					break;
				case 5:
					// Static monster spawns
					Writer.Word(0);
					Writer.Word(1);
					Writer.Byte(o.xSec);
					Writer.Byte(o.ySec);

					if (!FileDB.CheckCave(o.xSec, o.ySec))
					{
						Writer.Word(Formule.packetx((float)o.x, o.xSec));
						Writer.Word(o.z);
						Writer.Word(Formule.packety((float)o.y, o.ySec));
					}
					else
					{
						if (o.x < 0)
						{
							Writer.Word(Formule.packetx((float)o.x, o.xSec));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.packetx((float)o.x, o.xSec));
						}

						Writer.DWord(o.z);

						if (o.y < 0)
						{
							Writer.Word(Formule.packety((float)o.y, o.ySec));
							Writer.Word(0xFFFF);
						}
						else
						{
							Writer.DWord(Formule.packety((float)o.y, o.ySec));
						}
					}

					Writer.Byte(1);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Byte(0);
					Writer.Float(0);    // Walk speed
					Writer.Float(0);    // Run speed
					Writer.Float(100);  // Berserk speed
					Writer.Byte(0);
					Writer.Byte(2);
					Writer.Byte(1);
					Writer.Byte(5);
					Writer.Byte(o.Type);
					Writer.Byte(0);
					o.LocalType = 1;
					break;
				default:
					break;
			}
			return Writer.GetBytes();
		}
		public static byte[] SpawnPortal(WorldMgr.Monsters o, WorldMgr.character c, int itemid)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			Writer.DWord(o.ID);
			Writer.DWord(o.UniqueID);
			Writer.Byte(o.xSec);
			Writer.Byte(o.ySec);
			Writer.Float(Formule.packetx((float)o.x, o.xSec));
			Writer.Float(o.z);
			Writer.Float(Formule.packety((float)o.y, o.ySec));
			Writer.Word(0);
			Writer.Byte(1);
			Writer.Byte(0);
			Writer.Byte(1);
			Writer.Byte(6);
			Writer.Text(c.Information.Name);
			Writer.DWord(itemid);
			Writer.Byte(1);
			return Writer.GetBytes();
		}
		public static byte[] ObjectSpawn(WorldMgr.Items w)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			Writer.DWord(w.Model);
			switch (w.Type)
			{
				case 1:
					Writer.DWord(w.amount);
					Writer.DWord(w.UniqueID);
					Writer.Byte(w.xSec);
					Writer.Byte(w.ySec);
					Writer.Float(Formule.packetx((float)w.x, w.xSec));
					Writer.Float(w.z);
					Writer.Float(Formule.packety((float)w.y, w.ySec));
					Writer.DWord(0);
					Writer.Byte(w.fromType);
					Writer.DWord(0);
					break;
				case 2:
					//Weapon and armory drops
					Writer.Byte(w.PlusValue);
					Writer.DWord(w.UniqueID);
					Writer.Byte(w.xSec);
					Writer.Byte(w.ySec);
					Writer.Float(Formule.packetx((float)w.x, w.xSec));
					Writer.Float(w.z);
					Writer.Float(Formule.packety((float)w.y, w.ySec));
					Writer.Word(0); // Angle
					Writer.Bool(w.downType);
					if (w.downType)
						Writer.DWord(w.Owner);
					Writer.Byte(ObjData.Manager.ItemBase[w.Model].SOX); // if rare 2 not 0
					Writer.Byte(w.fromType);
					Writer.DWord(w.fromOwner);
					break;
				case 3:
					//Other item types
					//TODO: Define more detailed drops (Quest items, Mall types etc).
					Writer.DWord(w.UniqueID);
					Writer.Byte(w.xSec);
					Writer.Byte(w.ySec);
					Writer.Float(Formule.packetx((float)w.x, w.xSec));
					Writer.Float(w.z);
					Writer.Float(Formule.packety((float)w.y, w.ySec));
					Writer.Word(0);
					Writer.Bool(w.downType);
					if (w.downType) Writer.DWord(w.Owner);
					Writer.Byte(0);
					Writer.Byte(w.fromType);
					Writer.DWord(w.fromOwner);
					break;
			}
			return Writer.GetBytes();
		}
		public static byte[] ObjectSpawnJob(WorldMgr.character c)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);

			/////////////////////////////////////////////////////// Character basic info
			#region Basic info
			Writer.DWord(c.Information.Model);
			Writer.Byte(c.Information.Volume);                      //Char Volume
			Writer.Byte(c.Information.Title);                       //Char Title
			Writer.Byte(c.Information.Pvpstate);                    //Pvp state
			if (c.Information.Pvpstate != 0) c.Information.PvP = true;
			Writer.Bool((c.Information.Level < 20 ? true : false)); //Beginners Icon

			Writer.Byte(c.Information.Slots);                       // Amount of items
			#endregion
			/////////////////////////////////////////////////////// Item info
			#region Item info
			Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 8, 0, true);
			Writer.Byte(5);
			Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 5, 1, true);
			Writer.Byte(0);
			#endregion
			/////////////////////////////////////////////////////// Character Location / id
			#region Location info / state
			Writer.DWord(c.Information.UniqueID);
			Writer.Byte(c.Position.xSec);
			Writer.Byte(c.Position.ySec);
			Writer.Float(Formule.packetx(c.Position.x, c.Position.xSec));
			Writer.Float(c.Position.z);
			Writer.Float(Formule.packety(c.Position.y, c.Position.ySec));
			Writer.Word(0);//angle
			Writer.Bool(c.Position.Walking);
			Writer.Byte(1); // walk:0 run:1 ;)

			if (c.Position.Walking)
			{
				Writer.Byte(c.Position.packetxSec);
				Writer.Byte(c.Position.packetySec);

				if (!FileDB.CheckCave(c.Position.packetxSec, c.Position.packetySec))
				{
					Writer.Word(c.Position.packetX);
					Writer.Word(c.Position.packetZ);
					Writer.Word(c.Position.packetY);
				}
				else
				{
					if (c.Position.packetX < 0)
					{
						Writer.Word(c.Position.packetX);
						Writer.Word(0xFFFF);
					}
					else
					{
						Writer.DWord(c.Position.packetX);
					}

					Writer.DWord(c.Position.packetZ);

					if (c.Position.packetY < 0)
					{
						Writer.Word(c.Position.packetY);
						Writer.Word(0xFFFF);
					}
					else
					{
						Writer.DWord(c.Position.packetY);
					}
				}
			}
			else
			{
				Writer.Byte(1);
				Writer.Word(0);//angle
			}


			Writer.Byte((byte)(c.State.LastState == 128 ? 2 : 1));
			Writer.Byte(0);
			Writer.Byte(3);
			Writer.Byte((byte)(c.Information.Berserking ? 1 : 0));

			Writer.Float(c.Speed.WalkSpeed);
			Writer.Float(c.Speed.RunSpeed);
			Writer.Float(c.Speed.BerserkSpeed);

			Writer.Byte(c.Action.Buff.count);
			for (byte b = 0; b < c.Action.Buff.SkillID.Length; b++)
			{
				if (c.Action.Buff.SkillID[b] != 0)
				{
					Writer.DWord(c.Action.Buff.SkillID[b]);
					Writer.DWord(c.Action.Buff.OverID[b]);
				}
			}
			#endregion
			/////////////////////////////////////////////////////// Character Job information / name
			#region Job information & name
			Writer.Text(c.Job.Jobname);
			Writer.Byte(1);
			Writer.Byte(c.Job.level);//Level job
			Writer.Byte(c.Information.Level);//Level char
			Writer.Byte(0);

			if (c.Transport.Right)
			{
				Writer.Byte(1);
				Writer.Byte(0);
				Writer.DWord(c.Transport.Horse.UniqueID);
			}

			else
			{
				Writer.Byte(0);
				Writer.Byte(0);
			}

			Writer.Byte(0);
			Writer.Byte(0);

			if (c.Network.Guild.Guildid > 0)
			{
				Writer.Text(c.Network.Guild.Name);
			}
			else
			{
				Writer.Word(0);//No guild
			}

			Writer.Byte(0);
			Writer.Byte(0xFF);
			Writer.Byte(4);
			#endregion
			return Writer.GetBytes();
		}
		public static byte[] ObjectSpawn(WorldMgr.character c)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_SPAWN);
			/////////////////////////////////////////////////////// Character basic info
			#region Basic info
			Writer.DWord(c.Information.Model);
			Writer.Byte(c.Information.Volume);                      //Char Volume
			Writer.Byte(c.Information.Title);                       //Char Title
			Writer.Byte(c.Information.Pvpstate);                    //Pvp state
			if (c.Information.Pvpstate != 0) c.Information.PvP = true;
			Writer.Bool((c.Information.Level < 20 ? true : false)); //Beginners Icon

			Writer.Byte(c.Information.Slots);                       // Amount of items
			#endregion
			/////////////////////////////////////////////////////// Item info
			#region Item info
			Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 8, 0, true);
			Writer.Byte(5);
			Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 5, 1, true);
			Writer.Byte(0);
			#endregion
			/////////////////////////////////////////////////////// Character Location / id
			#region Location info / state
			Writer.DWord(c.Information.UniqueID);
			Writer.Byte(c.Position.xSec);
			Writer.Byte(c.Position.ySec);
			Writer.Float(Formule.packetx(c.Position.x, c.Position.xSec));
			Writer.Float(c.Position.z);
			Writer.Float(Formule.packety(c.Position.y, c.Position.ySec));
			Writer.Word(0);//angle
			Writer.Bool(c.Position.Walking);
			Writer.Byte(1); // walk:0 run:1 ;)
			//This should send the location information while moving. and where we moving 
			if (c.Position.Walking)
			{
				Writer.Byte(c.Position.packetxSec);
				Writer.Byte(c.Position.packetySec);
				if (!FileDB.CheckCave(c.Position.packetxSec, c.Position.packetySec))
				{
					Writer.Word(c.Position.packetX);
					Writer.Word(c.Position.packetZ);
					Writer.Word(c.Position.packetY);
				}
				else
				{
					if (c.Position.packetX < 0)
					{
						Writer.Word(c.Position.packetX);
						Writer.Word(0xFFFF);
					}
					else
					{
						Writer.DWord(c.Position.packetX);
					}

					Writer.DWord(c.Position.packetZ);

					if (c.Position.packetY < 0)
					{
						Writer.Word(c.Position.packetY);
						Writer.Word(0xFFFF);
					}
					else
					{
						Writer.DWord(c.Position.packetY);
					}
				}
				/*byte[] x = BitConverter.GetBytes(c.Position.packetX);
				Array.Reverse(x);
				Writer.Buffer(x);

				Writer.Word(c.Position.packetZ);

				byte[] y = BitConverter.GetBytes(c.Position.packetY);
				Array.Reverse(y);
				Writer.Buffer(y);*/


			}
			else
			{
				Writer.Byte(1);
				Writer.Word(0);//angle
			}


			Writer.Byte((byte)(c.State.LastState == 128 ? 2 : 1));

			Writer.Byte(0);
			//Info : If a player spawns at your location and is walking it send byte 3, else 0 byte.
			if (c.Transport.Right)
				Writer.Byte(c.Transport.Horse.Walking == true ? 3 : 0);
			else
				Writer.Byte(c.Position.Walking == true ? 3 : 0);

			Writer.Byte((byte)(c.Information.Berserking ? 1 : 0));
			Writer.Byte(0);
			Writer.Float(c.Speed.WalkSpeed);
			Writer.Float(c.Speed.RunSpeed);
			Writer.Float(c.Speed.BerserkSpeed);

			Writer.Byte(c.Action.Buff.count);
			for (byte b = 0; b < c.Action.Buff.SkillID.Length; b++)
			{
				if (c.Action.Buff.SkillID[b] != 0)
				{
					Writer.DWord(c.Action.Buff.SkillID[b]);
					Writer.DWord(c.Action.Buff.OverID[b]);
				}
			}
			#endregion
			/////////////////////////////////////////////////////// Character Job information / name
			#region Job information & name
			Writer.Text(c.Information.Name);
			Writer.Byte(0);
			if (c.Transport.Right)
			{
				Writer.Byte(1);
				Writer.Byte(0);
				Writer.DWord(c.Transport.Horse.UniqueID);
			}

			else
			{
				Writer.Byte(0);
				Writer.Byte(0);
			}

			Writer.Byte(0);
			if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
				Writer.Byte(0x04);
			else
				Writer.Byte(0);
			//Writer.Byte(0);

			if (c.Network.Guild.Guildid > 0)
			{
				Writer.Text(c.Network.Guild.Name);
				if (c.Network.Guild.GrantName != "")
				{
					Writer.DWord(0);//Icon ?
					Writer.Text(c.Network.Guild.GrantName);
				}
				else
				{
					Writer.DWord(0);//Icon
					Writer.Word(0);//No grantname
				}
			}
			else
			{
				Writer.Word(0);//No guild
				Writer.DWord(0);//No icon
				Writer.Word(0);//No grantname
			}

			Writer.DWord(0);// emblem (guild)  
			Writer.DWord(0); // unique id (union)  
			Writer.DWord(0); //emblem (union)  

			Writer.Byte(0); // flag (guild_war)
			Writer.Byte(0); // flag (fortress_rank)
			if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
			{
				Writer.Text3(c.Network.Stall.StallName);
				Writer.DWord(c.Information.StallModel);
			}
			Writer.Byte(0);

			#endregion
			/////////////////////////////////////////////////////// Pvp state
			#region pvpstate
			if (c.Information.Pvpstate > 0 || c.Information.Murderer)
			{
				Writer.Byte(0x22);
			}
			else
			{
				Writer.Byte(0xFF);
			}
			#endregion
			Writer.Byte(4);
			return Writer.GetBytes();
		}
		public static byte[] ObjectDeSpawn(int id)
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(OperationCode.SERVER_SOLO_DESPAWN);
			Writer.DWord(id);
			return Writer.GetBytes();
		}
	}
}
