using System;
using System.Linq;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
	public partial class PlayerMgr
	{
		enum GM_COMMAND : ushort
		{
			FINDUSER = 1,
			GOTOWN = 2,
			TOTOWN = 3,
			WORLDSTATUS = 4,
			STAT = 5,
			LOADMONSTER = 6,
			MAKEITEM = 7,
			MOVETOUSER = 8,
			SETTIME = 10,
			ZOE = 12,
			BAN = 13,
			INVISIBLE = 14,
			INVINCIBLE = 15,
			WARP = 16,
			RECALLUSER = 17,
			RECALLGUILD = 18,
			MOBKILL = 20,
			BLOCKLOGOUT = 23,
			LIENAME = 25,
			INITQ = 27,
			MOVETONPC = 31,
			MAKERENTITEM = 38,
			FLAGWORLD = 48,
			CLEARINVENTORY = 49,
			TRIGGERACTION = 55,
			ROTIME = 65,
			ENTERGMM = 66,
			WEATHER_RAIN = 105,
			WEATHER_SNOW = 106,
			WEATHER_CLEAR = 107
		};
        public void GameMaster()
		{
			if (Character.Information.GM == 1)
			{
				PacketReader Reader = new PacketReader(PacketInformation.buffer);
				if (Reader == null)
				{
					Disconnect("normal");
					return;
				}
				ushort CommandType = Reader.UInt16();
				Console.WriteLine("GM_COMMAND::{0} USED.BY:{1} ", (GM_COMMAND)CommandType, Character.Information.Name);
				switch ((GM_COMMAND)CommandType)
				{
					case GM_COMMAND.FINDUSER:
						GMfinduser(Reader.Text());
						break;
					case GM_COMMAND.GOTOWN:
						GMgotown();
						break;
					case GM_COMMAND.TOTOWN:
						GMtotown(Reader.Text());
						break;
					case GM_COMMAND.WORLDSTATUS:
						GMworldstatus();
						break;
					case GM_COMMAND.LOADMONSTER:
						GMloadmonster(Reader.Int32(), Reader.Byte(), Reader.Byte());
						break;
					case GM_COMMAND.MAKEITEM:
					case GM_COMMAND.MAKERENTITEM:
						GMmakeitem(Reader.Int32(), Reader.Byte());
						break;
					case GM_COMMAND.MOVETOUSER:
						GMmovetouser(Reader.Text());
						break;
					case GM_COMMAND.ZOE:
						GMzoe(Reader.Int32(), Reader.Byte());
						break;
					case GM_COMMAND.BAN:
						GMban(Reader.Text());
						break;
					case GM_COMMAND.INVISIBLE:
						GMinvisible();
						break;
					case GM_COMMAND.INVINCIBLE:
						GMinvincible();
						break;
					case GM_COMMAND.WARP:
						if (PacketInformation.buffer.Length > 4)
							GM_WP(Reader.Byte(), Reader.Byte(), Reader.Single(), Reader.Single(), Reader.Single());
						break;
					case GM_COMMAND.RECALLUSER:
						GMrecalluser(Reader.Text());
						break;
					case GM_COMMAND.MOBKILL:
						GMmobkill(Reader.Int32(), Reader.UInt16());
						break;
					case GM_COMMAND.BLOCKLOGOUT:
						GMblocklogout(Reader.Text(), Reader.Byte());
						break;
					case GM_COMMAND.LIENAME:
						GM_LIENAME(Reader.Text());
						GM_TRANSFORM(Reader.Text());
						break;
					case GM_COMMAND.INITQ:
						GMinitq();
						break;
					case GM_COMMAND.MOVETONPC:
						GMmovetonpc(Reader.Text());
						break;
					case GM_COMMAND.ROTIME: // Kontrol Edilecek
						//GMspawnuniques();
						break;
					case GM_COMMAND.CLEARINVENTORY:
						GMclearinventory();
						break;
					case GM_COMMAND.ENTERGMM:
						GMentergmm();
						break;
					case GM_COMMAND.WEATHER_RAIN:
                        Helpers.SendToClient.SendAll(Packet.Weather(2, Reader.Int32()));
						break;
					case GM_COMMAND.WEATHER_SNOW:
                        Helpers.SendToClient.SendAll(Packet.Weather(3, Reader.Int32()));
						break;
					case GM_COMMAND.WEATHER_CLEAR:
                        Helpers.SendToClient.SendAll(Packet.Weather(1, Reader.Int32()));
						break;
					default:
						Print.Format("Non Coded GM Command:{0} -> {1}", CommandType, Decode.StringToPack(PacketInformation.buffer));
						break;
				}
				Reader.Close();
			}
			else
			{
				Disconnect("ban");
			}
		}
		void GMgotown() // need to rework...
		{

			Character.InGame = false;
			BuffAllClose();

			DeSpawnMe();
			ObjectDeSpawnCheck();

			Teleport_UpdateXYZ(Character.Information.Place);
			client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[Character.Information.Place].xSec, ObjData.Manager.PointBase[Character.Information.Place].ySec));
			Character.Teleport = true;
			Character.State.Sitting = false;
			Character.Position.Walking = false;
		}
		void GMmovetonpc(string NPCNAME)
		{
            WorldMgr.Monsters selectednpc = Helpers.GetInformation.GetObject(NPCNAME);
			if (selectednpc != null)
			{
				Character.Position.xSec = selectednpc.ySec;
				Character.Position.ySec = selectednpc.ySec;
				Character.Position.x = (float)selectednpc.x;
				Character.Position.z = (float)selectednpc.z;
				Character.Position.y = (float)selectednpc.y;
				BuffAllClose();
				ObjectDeSpawnCheck();
				DeSpawnMe();
				//client.Send(Packet.TeleportOtherStart());
				Character.InGame = false;
				client.Send(Packet.TeleportImage(selectednpc.xSec, selectednpc.ySec));
				Character.Teleport = true;
			}
			else
			{
				client.Send(Packet.GameMaster(1, 0, 0, 0, "Could not find the chosen npc, teleporting has been stopped"));
			}
		}
		void GMworldstatus()
		{
			client.Send(Packet.GameMaster(4, Helpers.Manager.clients.Count, Helpers.Manager.Objects.Count, Helpers.Manager.WorldItem.Count));
		}
		void GMfinduser(string UserNickName)
		{
			for (int i = 0; i < Helpers.Manager.clients.Count; i++)
			{
				if (Helpers.Manager.clients[i] != null)
				{
					if (Helpers.Manager.clients[i].Character.Information.Name == UserNickName)
					{
						string Text = string.Format("Karakter Bulundu : X:{0} Y:{1} Z:{2}",
							Helpers.Manager.clients[i].Character.Position.x,
							Helpers.Manager.clients[i].Character.Position.y,
							Helpers.Manager.clients[i].Character.Position.z);
						client.Send(Packet.GameMaster(1, 0, 0, 0, Text));
					}
					else
					{
						if (i == Helpers.Manager.clients.Count)
						{
							client.Send(Packet.GameMaster(1, 0, 0, 0, "Karakter Bulunamadi"));
						}
					}
				}
			}
		}
		void GMentergmm()
		{
			Character.Position.GM = true;
			Character.Position.xSec = 23;
			Character.Position.ySec = 128;
			Character.Position.x = -21504;
			Character.Position.z = 0;
			Character.Position.y = 6911;
			BuffAllClose();
			ObjectDeSpawnCheck();
			DeSpawnMe();
			Character.InGame = false;
			client.Send(Packet.TeleportImage(23, 128));
			Character.Teleport = true;
		}
		void GMzoe(int Monsterid, byte Amount)
		{
			if (ObjData.SpawnData.TempSpawn == null)
			{
                ObjData.SpawnData.TempSpawn = new List<int>();
			}
			GMloadmonster(Monsterid, Amount, 0, true);
			for (int i = 0; i < ObjData.SpawnData.TempSpawn.Count; i++)
			{
				GMmobkill(ObjData.SpawnData.TempSpawn[i], 0);
				//SpawnObjData.Manager.TempSpawn.Remove(SpawnObjData.Manager.TempSpawn[i]);
			}
            ObjData.SpawnData.TempSpawn.Clear();
		}
		void GMinvincible()
		{
			Character.Stat.MaxMagAttack = (500000);
			Character.Stat.MaxPhyAttack = (500000);
			Character.Stat.Hp = (500000);
			Character.Stat.Mp = (500000);
			Character.Stat.PhyDef = (500000);
			Character.Stat.MagDef = (500000);
			Character.Stat.Parry = (500000);
			Character.InGame = false;

			BuffAllClose();
			DeSpawnMe();
			ObjectDeSpawnCheck();
			client.Send(Packet.TeleportOtherStart());

			Teleport_UpdateXYZ(Character.Information.Place);
			client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[Character.Information.Place].xSec, ObjData.Manager.PointBase[Character.Information.Place].ySec));
			Character.Teleport = true;
			Timer.Scroll.Dispose();
			Timer.Scroll = null;
			Character.Information.Scroll = false;
		}
		void GMclearinventory()
		{
			DB.query("delete from char_items where owner='" + Character.Information.CharacterID + "'");
            Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
			StartScrollTimer(1000);
			SavePlayerReturn();
		}
		void GM_TRANSFORM(string model)
		{
			//Check names monsters only
			if (ObjData.Manager.ObjectBase[Convert.ToInt32(model)].Name.Contains("MOB_"))
			{
				//Send list
				List<int> To = Character.Spawn;
				//Transform skill id
				int skillid = 7126;
				//Send to item buff
				SpecialBuff(skillid);
                //Send packet
                Send(To, Packet.Transform(Convert.ToInt32(model), Character.Information.UniqueID));
			}
			else
			{
				return;
			}
		}
		void GMblocklogout(string name, byte type)
		{
			//Type one we will use for kicking a user (disconnect).
			if (type == 1)
			{
				PlayerMgr playerid = Helpers.GetInformation.GetPlayerName(name);
				if (playerid.Character != null)
				{
					KickPlayer(playerid);
				}
			}
			//Type two not sure but we can use more types if needed
			else if (type == 2)
			{

			}
		}
		void GMinvisible()
		{
			if (!Character.Information.Invisible)
			{
				Send(Character.Spawn, Packet.StatePack(Character.Information.UniqueID, 4, 4, true));
				Character.Information.Invisible = true;
				return;
			}
			else if (Character.Information.Invisible)
			{
				Send(Character.Spawn, Packet.StatePack(Character.Information.UniqueID, 4, 0, false));
				Character.Information.Invisible = false;
				return;
			}
		}
		void GMinitq()  // only for buffs now
		{
			PacketReader reader = new PacketReader(PacketInformation.buffer);
			short ignore = reader.Int16();
			short skilllenght = reader.Int16();
			string skill = reader.String(skilllenght);
			Character.Action.UsingSkillID = Convert.ToInt32(skill);
			SkillBuff();
		}
		void GMmakeitem(int itemID, byte PlusValue)
		{
			try
			{
				WorldMgr.Items sitem = new WorldMgr.Items();
				sitem.Model = itemID;
				sitem.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.World);
				sitem.UniqueID = sitem.Ids.GetUniqueID;
				sitem.amount = 1;
				sitem.PlusValue = PlusValue;
				sitem.x = Character.Position.x;
				sitem.z = Character.Position.z;
				sitem.y = Character.Position.y;
				sitem.xSec = Character.Position.xSec;
				sitem.ySec = Character.Position.ySec;
				sitem.CalculateNewPosition();
				#region Amount definition
				if (ObjData.Manager.ItemBase[sitem.Model].Equip)
					sitem.Type = 2;
				else
					sitem.Type = 3;
				#endregion

				sitem.fromType = 5;
				sitem.fromOwner = 0;
				sitem.downType = false;
				sitem.Owner = Character.Account.ID;
				Helpers.Manager.WorldItem.Add(sitem);
				
				sitem.Send(Packet.ObjectSpawn(sitem), true);
				Print.Format("[Gameserver:" + Character.Information.Name + " Has created:  {0}", ObjData.Manager.ItemBase[itemID].Name);
			}
			catch (Exception ex)
			{
				Console.WriteLine("GM_MakeItem: {0}", ex);
			}
		}
		void GMloadmonster(int M_ObjID, byte M_Count, byte M_Type, bool Zoe = false)
		{
			try
			{

				for (int index = 1; index <= M_Count; index++)
				{

					WorldMgr.Monsters o = new WorldMgr.Monsters();
					o.ID = M_ObjID;
					o.Type = M_Type;
					o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
					o.UniqueID = o.Ids.GetUniqueID;
					if (Zoe)
					{
                        ObjData.SpawnData.TempSpawn.Add(o.UniqueID);
						o.AutoMovement = false;
					}
					else
					{
						o.AutoMovement = true;
					}
					o.x = Character.Position.x;
					o.z = Character.Position.z;
					o.y = Character.Position.y;
					o.OriginalX = o.x;
					o.OriginalY = o.y;
					o.xSec = Character.Position.xSec;
					o.ySec = Character.Position.ySec;
					o.Angle = (ushort)(index / M_Count * 65535 / M_Count * index);
					o.HP = ObjData.Manager.ObjectBase[o.ID].HP;
					o.Agresif = ObjData.Manager.ObjectBase[o.ID].Agresif;
					if (ObjData.Manager.ObjectBase[o.ID].Type == 4) o.LocalType = 4;
					else if (ObjData.Manager.ObjectBase[o.ID].Type == 1) o.LocalType = 1;
					else if (ObjData.Manager.ObjectBase[o.ID].Type == 2) o.LocalType = 2;
					o.AutoSpawn = false;
					o.Kat = 1;
					o.WalkingSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedWalk;
					o.RunningSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedRun;
					o.BerserkerSpeed = ObjData.Manager.ObjectBase[o.ID].SpeedZerk;
					switch (M_Type)
					{
						case 1:
							o.Agresif = 1;
							o.HP *= 2;
							o.Kat = 2;
							o.StartAgressiveTimer(2000);
							break;
						case 4:
							o.HP *= 20;
							o.Kat = 20;
							o.Agresif = 1;
							o.StartAgressiveTimer(2000);
							break;
						case 5:
							o.HP *= 100;
							o.Kat = 100;
							o.Agresif = 1;
							o.StartAgressiveTimer(2000);
							break;
						case 16:
							o.HP *= 10;
							o.Kat = 10;
							break;
						case 17:
							o.HP *= 20;
							o.Kat = 20;
							o.Agresif = 1;
							o.StartAgressiveTimer(2000);
							break;
						case 20:
							o.HP *= 210;
							o.Kat = 210;
							o.Agresif = 1;
							o.StartAgressiveTimer(2000);
							break;
					}

					Helpers.Manager.Objects.Add(o);
					o.SpawnMe();
					if (M_Type == 3 || M_Type == 5)
					{
                        Helpers.SendToClient.SendAll(Packet.Unique_Data(5, o.ID, string.Empty));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
		void GM_LIENAME(string name)
		{
			Character.Information.Name = name;
			Character.InGame = false;

			BuffAllClose();

			DeSpawnMe();
			ObjectDeSpawnCheck();
			client.Send(Packet.TeleportOtherStart());

			Teleport_UpdateXYZ(Character.Information.Place);
			client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[Character.Information.Place].xSec, ObjData.Manager.PointBase[Character.Information.Place].ySec));
			Character.Teleport = true;
			Timer.Scroll.Dispose();
			Timer.Scroll = null;
			Character.Information.Scroll = false;
		}
		void GM_ISATTACK(int id, ushort type)
		{
			try
			{
				WorldMgr.Monsters o = Helpers.GetInformation.GetObject(id);
				o.Agresif = 1;

			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		}
		void GMmobkill(int id, ushort type)
		{
			try
			{
				WorldMgr.Monsters o = Helpers.GetInformation.GetObject(id);
				o.ChangeState(0, 2);
				o.AddAgroDmg(Character.Information.UniqueID, o.HP);
				o.HP = 0;
				Send(Packet.StatePack(Character.Information.UniqueID, 8, 0, false));
				o.Send(Packet.Testeffect(id, o.HP));
				o.GetDie = true;
				o.Die = true;
				o.MonsterDrop();
				o.SetExperience();
				o.StartDeadTimer(5000);
				if (o.Type == 3)
				{
                    Helpers.SendToClient.SendAll(Packet.Unique_Data(6, (int)o.ID, Character.Information.Name));
				}
				//Thread.Sleep(5000);
				/*o.Dispose();
				o = null;*/
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		}
		void GMban(string name)
		{
			lock (Helpers.Manager.clients)
			{
				for (int i = 0; i <= Helpers.Manager.clients.Count; i++)
				{
					if (Helpers.Manager.clients[i].Character.Information.Name == name)
					{
						DB.query("UPDATE users SET ban='" + 1 + "' WHERE id='" + Helpers.Manager.clients[i].Player.AccountName + "'");
						Helpers.Manager.clients[i].client.Send(Packet.ChatPacket(7, Character.Information.UniqueID, "from GM:You are banned.", null));
						Helpers.Manager.clients[i].Disconnect("normal");
						return;
					}
				}
			}
		}
		void GMtotown(string name) // need to rework...
		{
			byte number = FileDB.GetTeleport(name);
			if (number == 255) return;
			if (Timer.Movement != null) Timer.Movement.Dispose();
			BuffAllClose();
			client.Send(Packet.TeleportOtherStart());

			//ObjectDeSpawn();
			ObjectDeSpawnCheck();
			DeSpawnMe();

			Character.InGame = false;
			Teleport_UpdateXYZ(number);
			client.Send(Packet.TeleportImage(ObjData.Manager.PointBase[number].xSec, ObjData.Manager.PointBase[number].ySec));

			Character.Teleport = true;
			ObjectSpawnCheck();
		}
		void GMrecalluser(string name)
		{
			lock (Helpers.Manager.clients)
			{
				try
				{
					if (Character.Information.Name == name)
					{
						return;
					}
					else if (name == "GETALLUSER")
					{
						for (int i = 0; i <= Helpers.Manager.clients.Count; i++)
						{
							if (Helpers.Manager.clients[i].Character.Information.Name == name && Helpers.Manager.clients[i].Character.InGame)
							{
								if (Helpers.Manager.clients[i].Timer.Movement != null) Helpers.Manager.clients[i].Timer.Movement.Dispose();

								Helpers.Manager.clients[i].BuffAllClose();
								Helpers.Manager.clients[i].DeSpawnMe();
								Helpers.Manager.clients[i].ObjectDeSpawnCheck();
								Helpers.Manager.clients[i].client.Send(Packet.TeleportOtherStart());

								Helpers.Manager.clients[i].Character.Position.xSec = Character.Position.xSec;
								Helpers.Manager.clients[i].Character.Position.ySec = Character.Position.ySec;
								Helpers.Manager.clients[i].Character.Position.x = Character.Position.x;
								Helpers.Manager.clients[i].Character.Position.z = Character.Position.z;
								Helpers.Manager.clients[i].Character.Position.y = Character.Position.y;

								Helpers.Manager.clients[i].client.Send(Packet.TeleportImage(Character.Position.xSec, Character.Position.xSec));
								Helpers.Manager.clients[i].Character.InGame = false;
								Helpers.Manager.clients[i].Character.Teleport = true;
								break;
							}
						}
					}
					else
					{
						for (int i = 0; i <= Helpers.Manager.clients.Count; i++)
						{
							if (Helpers.Manager.clients[i].Character.Information.Name == name && Helpers.Manager.clients[i].Character.InGame)
							{
								if (Helpers.Manager.clients[i].Timer.Movement != null) Helpers.Manager.clients[i].Timer.Movement.Dispose();

								Helpers.Manager.clients[i].BuffAllClose();
								Helpers.Manager.clients[i].DeSpawnMe();
								Helpers.Manager.clients[i].ObjectDeSpawnCheck();
								Helpers.Manager.clients[i].client.Send(Packet.TeleportOtherStart());

								Helpers.Manager.clients[i].Character.Position.xSec = Character.Position.xSec;
								Helpers.Manager.clients[i].Character.Position.ySec = Character.Position.ySec;
								Helpers.Manager.clients[i].Character.Position.x = Character.Position.x;
								Helpers.Manager.clients[i].Character.Position.z = Character.Position.z;
								Helpers.Manager.clients[i].Character.Position.y = Character.Position.y;

								Helpers.Manager.clients[i].client.Send(Packet.TeleportImage(Character.Position.xSec, Character.Position.xSec));
								Helpers.Manager.clients[i].Character.InGame = false;
								Helpers.Manager.clients[i].Character.Teleport = true;
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
				}
			}
		}
		void GMmovetouser(string name)
		{
			lock (Helpers.Manager.clients)
			{
				try
				{
					for (int i = 0; i <= Helpers.Manager.clients.Count; i++)
					{
						if (Helpers.Manager.clients[i].Character.Information.Name == name && Helpers.Manager.clients[i].Character.InGame)
						{

							BuffAllClose();
							DeSpawnMe();
							//ObjectDeSpawn();
							ObjectDeSpawnCheck();
							client.Send(Packet.TeleportOtherStart());

							Character.Position.xSec = Helpers.Manager.clients[i].Character.Position.xSec;
							Character.Position.ySec = Helpers.Manager.clients[i].Character.Position.ySec;
							Character.Position.x = Helpers.Manager.clients[i].Character.Position.x;
							Character.Position.z = Helpers.Manager.clients[i].Character.Position.z;
							Character.Position.y = Helpers.Manager.clients[i].Character.Position.y;

							client.Send(Packet.TeleportImage(Character.Position.xSec, Character.Position.xSec));
							Character.InGame = false;
							Character.Teleport = true;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
				}
			}
		}
		void GM_WP(byte xSec, byte ySec, float x, float z, float y)
		{
			//Close buffs
			BuffAllClose();
			//Send teleport packet #1
			client.Send(Packet.TeleportStart());
			//Despawn objects
			ObjectDeSpawnCheck();
			//Despawn player to other players
			DeSpawnMe();
			//Set state
			Character.InGame = false;
			//Update location
			Character.Position.xSec = xSec;
			Character.Position.ySec = ySec;
			Character.Position.x = Formule.gamex(x, xSec);
			Character.Position.z = z;
			Character.Position.y = Formule.gamey(y, ySec);
			//Required
			client.Send(Packet.TeleportStart2());
			//Send loading screen image
			client.Send(Packet.TeleportImage(xSec, ySec));
			//Set bool
			Character.Teleport = true;
			SavePlayerPosition();
			PlayerDataLoad();
		}
	}
}