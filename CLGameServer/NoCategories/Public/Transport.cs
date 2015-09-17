using System;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Special Transports / Unicorns etc
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool HandleSpecialTrans(int ItemID)
        {
            try
            {
                int model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName);
                if (Character.Information.Level < ObjData.Manager.ItemBase[ItemID].Level) return true;

                {
                    model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName);
                    if (model == 0) return true;
                }
                WorldMgr.pet_obj o = new WorldMgr.pet_obj();
                o.Model = model;
                o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = ObjData.Manager.ObjectBase[model].HP;
                o.OwnerID = Character.Information.UniqueID;

                Character.Transport.Right = true;

                List<int> S = o.SpawnMe();
                o.Information = true;
                client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(S, Packet.Player_UpToHorse(Character.Information.UniqueID, true, o.UniqueID));
                Helpers.Manager.HelperObject.Add(o);
                Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Normal Transport
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool HandleHorseScroll(int ItemID)
        {
            try
            {
                int model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName);
                if (model == 0)
                {
                    string extrapath = null;
                    if (Character.Information.Level >= 1 && Character.Information.Level <= 5)
                        extrapath = "_5";
                    else if (Character.Information.Level >= 6 && Character.Information.Level <= 10)
                        extrapath = "_10";
                    else if (Character.Information.Level >= 11 && Character.Information.Level <= 20)
                        extrapath = "_20";
                    else if (Character.Information.Level >= 21 && Character.Information.Level <= 30)
                        extrapath = "_30";
                    else if (Character.Information.Level >= 31 && Character.Information.Level <= 45)
                        extrapath = "_45";
                    else if (Character.Information.Level >= 46 && Character.Information.Level <= 60)
                        extrapath = "_60";
                    else if (Character.Information.Level >= 61 && Character.Information.Level <= 75)
                        extrapath = "_75";
                    else if (Character.Information.Level >= 76 && Character.Information.Level <= 90)
                        extrapath = "_90";
                    else if (Character.Information.Level >= 91 && Character.Information.Level <= 105)
                        extrapath = "_105";
                    else if (Character.Information.Level >= 106 && Character.Information.Level <= 120)
                        extrapath = "_120";
                    model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName + extrapath);
                    if (model == 0) return true;
                }
                WorldMgr.pet_obj o = new WorldMgr.pet_obj();
                o.Model = model;
                o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = ObjData.Manager.ObjectBase[model].HP;
                o.OwnerID = Character.Information.UniqueID;
                o.Speed1 = ObjData.Manager.ObjectBase[model].Speed1;
                o.Speed2 = ObjData.Manager.ObjectBase[model].Speed2;
                Character.Transport.Right = true;

                List<int> S = o.SpawnMe();
                o.Information = true;
                client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(Packet.SetSpeed(o.UniqueID, o.Speed1, o.Speed2));//Global Speed Update
                Send(Packet.ChangeStatus(o.UniqueID, 3, 0));// Global Status 
                Send(S, Packet.Player_UpToHorse(Character.Information.UniqueID, true, o.UniqueID));

                Helpers.Manager.HelperObject.Add(o);
                Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }

        bool HandleJobTransport(int ItemID)
        {
            try
            {
                int model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName);
                if (Character.Information.Level < ObjData.Manager.ItemBase[ItemID].Level) return true;

                {
                    model = ObjData.objectdata.GetItem(ObjData.Manager.ItemBase[ItemID].ObjectName);
                    if (model == 0) return true;
                }
                WorldMgr.pet_obj o = new WorldMgr.pet_obj();
                o.Model = model;
                o.Named = 4;
                o.Ids = new GenerateUniqueID(GenerateUniqueID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = Character.Position.x;
                o.z = Character.Position.z;
                o.y = Character.Position.y;
                o.xSec = Character.Position.xSec;
                o.ySec = Character.Position.ySec;
                o.Hp = ObjData.Manager.ObjectBase[model].HP;
                o.OwnerID = Character.Information.UniqueID;
                o.OwnerName = Character.Information.Name;
                Character.Transport.Right = true;

                o.Information = true;
                //client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(Packet.Player_UpToHorse(Character.Information.UniqueID, true, o.UniqueID));
                Helpers.Manager.HelperObject.Add(o);
                Character.Transport.Horse = o;
                return false;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
    }
}