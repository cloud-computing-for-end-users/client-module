using Core.ExternalComms;
using Core.InternalComms;

namespace Core
{
    class CommsWrapper
    {
        public static ExternalComm ExternalComms { get; set; }
        public static InternalComm InternalComm { get; set; }

        public static void Setup(bool localhost)
        {
            ExternalComms = new ExternalComm(localhost);
            InternalComm = new InternalComm(ExternalComms.Comm);
        }
    }
}
