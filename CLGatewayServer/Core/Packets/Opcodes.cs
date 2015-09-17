namespace Clowen.Core.Packets
{
    class Opcodes
    {
        public static class CLIENT
        {
            public const ushort
            CLIENT_INFO = 0x2001,
            CLIENT_PING_CHECK = 0x2002,
			CLIENT_GATEWAY_PATCH_REQUEST = 0x6100,
			CLIENT_GATEWAY_SERVERLIST_REQUEST = 0x6101,
			CLIENT_GATEWAY_LOGIN_REQUEST = 0x6102,
			CLIENT_GATEWAY_NOTICE_REQUEST = 0x6104,
            CLIENT_UNKNOWN2 = 0x6107,
            CLIENT_HANDSHAKE = 0x9000;
        }
        public static class SERVER
        {
            public const ushort
            SERVER_INFO = 0x2001,
            SERVER_SERVERLIST = 0xA101,
			SERVER_GATEWAY_LOGIN_RESPONSE = 0xA102,
            SERVER_LOGIN_OK = 0x5000,
            SERVER_MAIN = 0x600D;
        }
    }
}
