using System;
using System.IO;
using custom_message_based_implementation.consts;
using custom_message_based_implementation.encoding;
using custom_message_based_implementation.model;
using message_based_communication.model;

namespace Core
{
    class ExternalComms
    {
        public ClientModuleCommunication comm { get; set; }
        
        public ExternalComms()
        {
            var localhost = true;
            
            var routerInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.212.11" },
                Port = new Port() { ThePort = 5522 }
            };

            var selfConnInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = (localhost) ? "127.0.0.1" : "10.152.210.23" },
                Port = new Port() { ThePort = 5542 }
            };

            comm = new ClientModuleCommunication(new ModuleType() { TypeID = ModuleTypeConst.MODULE_TYPE_CLIENT }, selfConnInfo);

            TextWriter consoleOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            comm.Setup(routerInfo, new Port() {ThePort = 5523}, selfConnInfo, new CustomEncoder());
            Console.SetOut(consoleOut);

            //comm.GetSlaveConnection(new PrimaryKey(), new ApplicationInfo());
        }
    }
}
