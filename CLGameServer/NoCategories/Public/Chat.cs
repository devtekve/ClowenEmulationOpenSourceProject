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
        // Chat Base
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte[] sendnoticecon(int type, int id, string text, string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(OperationCode.SERVER_CHAT);
            Writer.Byte(7);
            string welcome = text;
            string Message1 = MessageToMessagelong(welcome);
            Writer.Textlen(Message1);
            for (int g = 0; g < Message1.Length; )
            {
                Writer.Word(int.Parse(Message1.Substring(g, 2), System.Globalization.NumberStyles.HexNumber, null));
                g = g + 2;
            }
            return Writer.GetBytes();
        }
        public void Chat()
        {
            try
            {
                List<int> lis = Character.Spawn;
                //Main getinfo = new Main();
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte chatType = Reader.Byte();
                byte chatIndex = Reader.Byte();
                byte linkCount = Reader.Byte(); // added in 295 client

                switch (chatType)
                {
                    case 1://town chat
                        string Text = Reader.Text3();
                        Reader.Close();
                        Send(lis, Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, null));
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    case 3://town chat pink
                        if (Character.Information.GM == 1)
                        {
                            Text = Reader.Text3();
                            Reader.Close();
                            Send(lis, Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, null));
                            client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                            break;
                        }
                        else
                        {
                            Disconnect("ban");
                        }
                        break;
                    case 2://Pm chat
                        string toName = Reader.Text();
                        PlayerMgr sys = null;
                        sys = Helpers.GetInformation.GetPlayerName(toName);
                        if (sys != null || sys.Character.InGame)
                        {
                            Text = Reader.Text3();
                            Reader.Close();

                            sys.client.Send(Packet.ChatPacket(chatType, 0, Text, Character.Information.Name));
                            client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        }
                        break;

                    case 4://Party chat
                        if (Character.Network.Party != null)
                        {
                            Text = Reader.Text3();
                            Reader.Close();

                            Character.Network.Party.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, Character.Information.Name));
                            client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        }

                        break;
                    case 5://Guild Chat
                        Text = Reader.Text3();
                        Reader.Close();
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //If the user is not the newly invited member get player info
                                PlayerMgr tomember = Helpers.GetInformation.GetPlayerMainid(member);
                                //Send guild update packet
                                if (tomember != null)
                                {
                                    tomember.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, Character.Information.Name));
                                }
                            }
                        }
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));

                        break;
                    case 6://Global chat
                        string FromName = Reader.Text();
                        Text = Reader.Text3();
                        Console.WriteLine(Text);
                        Helpers.SendToClient.SendAll(Packet.ChatPacket(chatType, Character.Information.UniqueID, " " + Text, FromName));
                        break;
                    case 7://Notice chat
                        if (Character.Information.GM == 1)
                        {
                            Text = Reader.Text3();
                            Reader.Close();
                            Helpers.SendToClient.SendAll(sendnoticecon(chatType, Character.Information.UniqueID, Text, null));
                        }
                        else
                        {
                            Disconnect("ban");
                        }
                        break;
                    case 9://Stall chat
                        Text = Reader.Text3();
                        Reader.Close();

                        Character.Network.Stall.Send(Packet.ChatPacket(chatType, Character.Network.Stall.ownerID, Text, Character.Information.Name));
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));

                        break;
                    case 10://Academy chat
                        Text = Reader.Text3();
                        Send(lis, Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, Character.Information.Name));
                        break;

                    case 11://Union chat
                        Text = Reader.Text3();
                        Reader.Close();

                        if (!Character.Network.Guild.UnionActive) return;
                        foreach (int member in Character.Network.Guild.UnionMembers)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //If the user is not the newly invited member get player info
                                PlayerMgr tomember = Helpers.GetInformation.GetPlayerMainid(member);
                                //Send guild update packet
                                if (tomember != null)
                                {
                                    if (!tomember.Character.Network.Guild.SingleSend)
                                    {
                                        tomember.Character.Network.Guild.SingleSend = true;
                                        tomember.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, Character.Information.Name));
                                    }
                                }
                            }
                        }
                        foreach (int member in Character.Network.Guild.UnionMembers)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //If the user is not the newly invited member get player info
                                PlayerMgr tomember = Helpers.GetInformation.GetPlayerMainid(member);
                                //Disable bool single send
                                if (tomember != null)
                                {
                                    tomember.Character.Network.Guild.SingleSend = false;
                                }
                            }
                        }
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chat error {0}", ex);
                Log.Exception(ex);
            }
        }
    }
}