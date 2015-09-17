using System;
using CLGameServer.Client;
using CLFramework;
namespace CLGameServer
{
    public partial class PlayerMgr
    {
        /////////////////////////////////////////////////////////////////////////////
        // Handle new premium tickets
        /////////////////////////////////////////////////////////////////////////////
        void HandlePremiumType(int ItemID)
        {
            //First we check if the user allready has a ticket active or not.
            if (!Character.Premium.Active)
            {
                //Set default values that we will use for the ticket.
                DateTime StartTime = DateTime.Now;
                double Timespan = StartTime.Minute;
                byte NewRate = 0;

                //Check which ticket the user has selected.
                if (ObjData.Manager.ItemBase[ItemID].Ticket == ObjData.item_database.Tickets.GOLD_1_DAY)
                {

                }
                else if (ObjData.Manager.ItemBase[ItemID].Ticket == ObjData.item_database.Tickets.GOLD_4_WEEKS)
                {
                    NewRate = 2;
                }
                    //Send visual packet for the icon
                    
            }
            else
            {
                //Send message to user that an active ticket excists.
                client.Send(Packet.Message(OperationCode.SERVER_TICKET, Messages.UIIT_MSG_PREMIUM_NOT_USE));
            }
        }
    }
}
