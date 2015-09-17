using System;
using CLFramework;

namespace Clowen.Core.Packets
{
    class Login
    {
        public static byte[] WriteMessage(string Message)
        {
            PacketWriter Writer = new PacketWriter();
			Writer.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Writer.Byte(3);
            Writer.Byte(0x0F);
            Writer.Byte(2);
            Writer.Text(Message);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] PasswordFailed(int CurrentAttempts, int MaxAttempts)
        {
            PacketWriter Write = new PacketWriter();
            Write.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Write.Byte(2);
            Write.Byte(1);
            Write.DWord(MaxAttempts);
            Write.DWord(CurrentAttempts);
            return Write.GetBytes();
        }
        public static byte[] AccountBanned(string BanReason)
        {
            PacketWriter Write = new PacketWriter();
            Write.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            
            Write.Byte(2);                  // ResultType
            Write.Byte(2);                  // ErrorType
            Write.Byte(1);                  // BannedType:[1 = block login, 2 = block login for inspection, 3 = block p2p trade, 4 = block chat]
            Write.Text(BanReason);
            Write.Word((short)DateTime.Now.Year);
            Write.Word((short)DateTime.Now.Month);
            Write.Word((short)DateTime.Now.Day);
            Write.Word((short)DateTime.Now.Hour);
            Write.Word((short)DateTime.Now.Minute);
            Write.Word((short)DateTime.Now.Second);
            Write.Word((short)DateTime.Now.Millisecond);
            return Write.GetBytes();
        }
        public static byte[] ConnectWrong(ushort type)
        {
            PacketWriter Writer = new PacketWriter();
			Writer.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Writer.Word(type);
            return Writer.GetBytes();
        }
        public static byte[] ServerIsFull() // need research
        {
            PacketWriter Writer = new PacketWriter();
			Writer.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Writer.Byte(0x02);
            Writer.Byte(0x05);
            return Writer.GetBytes();
        }
        public static byte[] AllreadyConnected()
        {
            PacketWriter Writer = new PacketWriter();
			Writer.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Writer.Byte(0x02);
            Writer.Byte(0x03);
            return Writer.GetBytes();
        }
        public static byte[] ConnectSucces(string ip, short port, byte type)
        {
            PacketWriter Writer = new PacketWriter();
			Writer.Create(Opcodes.SERVER.SERVER_GATEWAY_LOGIN_RESPONSE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(0);
            Writer.Word(0);
            Writer.Text(ip);
            Writer.Word(port);
            Writer.Byte(3);
            return Writer.GetBytes();
        }
    }
}
