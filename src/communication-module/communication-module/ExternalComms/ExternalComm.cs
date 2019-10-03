using custom_message_based_implementation.consts;
using custom_message_based_implementation.encoding;
using message_based_communication.model;

namespace Core.ExternalComms
{
    class ExternalComm
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ClientModuleCommunication Comm { get; set; }
        
        public ExternalComm(bool localhost)
        {
            Logger.Info("Setting up external communication " + (localhost ? "on" : "NOT on") + " localhost");
            var routerInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.212.28" }, // Kenneth
                Port = new Port() { ThePort = 5522 } // todo set port from constant or config
            };

            var selfConnInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.209.250" }, // Me
                Port = new Port() { ThePort = 5542 } // todo set port from constant or config
            };

            Comm = new ClientModuleCommunication(new ModuleType() { TypeID = ModuleTypeConst.MODULE_TYPE_CLIENT }, selfConnInfo);
            
            Comm.Setup(routerInfo, new Port() {ThePort = 5523}, selfConnInfo, new CustomEncoder()); // todo set port from constant or config
        }
    }
}
