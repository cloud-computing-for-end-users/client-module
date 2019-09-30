using client_slave_message_communication.proxy;
using message_based_communication.model;

namespace Core.Model
{
    public class SlaveInfo
    {
        public SlaveProxy SlaveProxy { get; set; }
        public AppDimensions AppDimensions { get; set; }
        public Port Port { get; set; }
    }
}
