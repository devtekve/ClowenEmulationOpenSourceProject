using System;
using System.Linq;
using System.Collections.Generic;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        public void Exchange_Request()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int targetid = Reader.Int32();
                Reader.Close();

                PlayerMgr sys = Helpers.GetInformation.GetPlayer(targetid);
                Character.Network.TargetID = targetid;

                sys.Character.Network.TargetID = Character.Information.UniqueID;
                sys.client.Send(Packet.PartyRequest(1, Character.Information.UniqueID, 0));

                Character.State.Exchanging = true;
                sys.Character.State.Exchanging = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
         void Request()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                if (Reader.Byte() == 1 && Reader.Byte() == 0)
                {
                    PlayerMgr sys = Helpers.GetInformation.GetPlayer(Character.Network.TargetID);
                    sys.client.Send(Packet.Exchange_Cancel());
                }
                else
                {
                    PlayerMgr sys = Helpers.GetInformation.GetPlayer(Character.Network.TargetID);
                    sys.client.Send(Packet.OpenExhangeWindow(1, Character.Information.UniqueID));
                    client.Send(Packet.OpenExhangeWindow(sys.Character.Information.UniqueID));

                    Character.Network.Exchange.Window = true;
                    Character.Network.Exchange.ItemList = new List<ObjData.slotItem>();
                    sys.Character.Network.Exchange.Window = true;
                    sys.Character.Network.Exchange.ItemList = new List<ObjData.slotItem>();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            
        }
        public void Exchange_Close()
        {
            try
            {
                PlayerMgr sys = Helpers.GetInformation.GetPlayer(Character.Network.TargetID);
                client.Send(Packet.Exchange_Cancel());
                client.Send(Packet.CloseExhangeWindow());
                Character.Network.Exchange.Window = false;
                Character.Network.Exchange.ItemList = null;

                if (sys != null)
                {
                    sys.client.Send(Packet.Exchange_Cancel());
                    sys.client.Send(Packet.CloseExhangeWindow());
                    sys.Character.Network.Exchange.Window = false;
                    sys.Character.Network.Exchange.ItemList = null;
                    Character.State.Exchanging = false;
                    sys.Character.State.Exchanging = false;
                }
                Character.State.Exchanging = false;
                sys.Character.State.Exchanging = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        public void Exchange_Accept()
        {

            try
            {
                PlayerMgr sys = Helpers.GetInformation.GetPlayer(Character.Network.TargetID);

                client.Send(Packet.Exchange_ItemPacket(Character.Information.UniqueID, Character.Network.Exchange.ItemList, true));
                client.Send(Packet.Exchange_Accept());

                sys.client.Send(Packet.Exchange_ItemPacket(Character.Information.UniqueID, Character.Network.Exchange.ItemList, false));
                sys.client.Send(Packet.Exchange_Gold(Character.Network.Exchange.Gold));
                sys.client.Send(Packet.Exchange_Accept2());
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
        public void Exchange_Approve()
        {
            try
            {
                PlayerMgr sys = Helpers.GetInformation.GetPlayer(Character.Network.TargetID);
                client.Send(Packet.Exchange_Approve());
                Character.Network.Exchange.Approved = true;
                if (sys.Character.Network.Exchange.Approved)
                {
                    #region Gold update
                    if (Character.Network.Exchange.Gold != 0)
                    {
                        Character.Information.Gold -= Character.Network.Exchange.Gold;
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        SaveGold();

                        sys.Character.Information.Gold += Character.Network.Exchange.Gold;
                        sys.client.Send(Packet.UpdateGold(sys.Character.Information.Gold));
                        sys.SaveGold();
                    }
                    if (sys.Character.Network.Exchange.Gold != 0)
                    {
                        sys.Character.Information.Gold -= sys.Character.Network.Exchange.Gold;
                        sys.client.Send(Packet.UpdateGold(sys.Character.Information.Gold));
                        sys.SaveGold();

                        Character.Information.Gold += sys.Character.Network.Exchange.Gold;
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        SaveGold();
                    }
                    #endregion

                    #region Items
                    if (Character.Network.Exchange.ItemList.Count > 0)
                    {
                        foreach (ObjData.slotItem item in Character.Network.Exchange.ItemList)
                        {
                            byte t_slot = sys.GetFreeSlot();
                            DB.query("UPDATE char_items SET owner='" + sys.Character.Information.CharacterID + "' WHERE owner='" + Character.Information.CharacterID + "' AND itemid='" + item.ID + "' AND id='" + item.dbID + "' AND storagetype='0'");
                        }
                    }

                    if (sys.Character.Network.Exchange.ItemList.Count > 0)
                    {
                        foreach (ObjData.slotItem item in sys.Character.Network.Exchange.ItemList)
                        {
                            byte t_slot = GetFreeSlot();
                            DB.query("UPDATE char_items SET owner='" + Character.Information.CharacterID + "' WHERE owner='" + sys.Character.Information.CharacterID + "' AND itemid='" + item.ID + "' AND id='" + item.dbID + "' AND storagetype='0'");
                        }
                    }
                    #endregion

                    client.Send(Packet.Exchange_Finish());
                    sys.client.Send(Packet.Exchange_Finish());
                    Character.State.Exchanging = false;
                    sys.Character.State.Exchanging = false;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
