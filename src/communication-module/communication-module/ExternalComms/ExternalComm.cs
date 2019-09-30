using System;
using System.IO;
using custom_message_based_implementation.consts;
using custom_message_based_implementation.encoding;
using custom_message_based_implementation.model;
using message_based_communication.model;

namespace Core.ExternalComms
{
    class ExternalComm
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ClientModuleCommunication comm { get; set; }
        
        public ExternalComm(bool localhost)
        {
            logger.Info("Setting up external communication " + (localhost ? "on" : "NOT on") + " localhost");
            var routerInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.212.21" }, // Kenneth
                Port = new Port() { ThePort = 5522 } // todo set port from constant or config
            };

            var selfConnInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.209.249" }, // Me
                Port = new Port() { ThePort = 5542 } // todo set port from constant or config
            };

            comm = new ClientModuleCommunication(new ModuleType() { TypeID = ModuleTypeConst.MODULE_TYPE_CLIENT }, selfConnInfo);
            
            comm.Setup(routerInfo, new Port() {ThePort = 5523}, selfConnInfo, new CustomEncoder()); // todo set port from constant or config
        }
    }
}
