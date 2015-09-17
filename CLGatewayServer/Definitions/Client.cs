namespace Clowen.Definitions
{
    class Clientdefinition
    {
        internal CLFramework.SRClient client;
        internal CLFramework.Decode PacketInformation;
        public static short Connected_Users;
        public Clientdefinition(CLFramework.SRClient de)
        {
            client = de;
        }
    }
}
