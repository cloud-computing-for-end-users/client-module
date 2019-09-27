using Core.ExternalComms;
using Core.InternalComms;

namespace Core
{
    class CommsWrapper
    {
        public ExternalComm ExternalComms { get; set; }
        public InternalComm InternalComm { get; set; }

        public CommsWrapper(bool localhost)
        {
            ExternalComms = new ExternalComm(localhost);
            InternalComm = new InternalComm(ExternalComms.comm);
        }
    }
}
