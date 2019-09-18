using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using client_slave_message_communication.encoding;
using client_slave_message_communication.proxy;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.connection;
using message_based_communication.model;
using message_based_communication.module;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core
{

    public class ClientModuleCommunication : BaseClientModule
    {
        private SlaveOwnerServermoduleProxy slaveOwner;
        private ConnectionInformation forSelf;
        
        private object slaveConnectionInfo;
        private object listOfApplications;
        private object pathToImage;
        private object port;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ClientModuleCommunication(ModuleType moduleType, ConnectionInformation forSelf) : base(moduleType)
        {
            this.forSelf = forSelf;
        }

        public override string CALL_ID_PREFIX => "ClientModule";

        private SlaveOwnerServermoduleProxy SlaveOwnerServerProxy => slaveOwner ?? (slaveOwner = new SlaveOwnerServermoduleProxy(base.proxyHelper, this));

        private SlaveProxy SlaveProxy { get; set; }


        public void GetSlaveConnection(PrimaryKey pk, ApplicationInfo appInfo)
        {
            
            slaveConnectionInfo = null;
            logger.Info("GetSlave initiated, set to null");
            SlaveOwnerServerProxy.GetSlave(pk, appInfo, GetSlaveCallBack);
            PollVariableFor10Seconds(ref slaveConnectionInfo, true);

            while (null == slaveConnectionInfo)
            {
                Thread.Sleep(100);
            }

            ProxyHelper proxyHelper = new ProxyHelper();
            Tuple<SlaveConnection, Port> Tuple = slaveConnectionInfo as Tuple<SlaveConnection, Port>;
            proxyHelper.Setup(new ConnectionInformation(){IP = Tuple.Item1.IP, Port = Tuple.Item1.Port}, Tuple.Item2, base.ModuleType, this.forSelf, this, new CustomEncoding());
            
            SlaveProxy = new SlaveProxy(proxyHelper, this);
            SlaveProxy.GetImageProducerConnectionInformation(GetImageProducerConnectionInformationCallBack);

            while (null == port)
            {
                Thread.Sleep(100);
            }

            ImageReceiver.StartImageReceivingThread(new ConnectionInformation(){IP = Tuple.Item1.IP, Port = port as Port}, @"C:\Users\kryst\Downloads\imagesFromPython\");
        } 

        public string GetListOfApplications()
        {
            listOfApplications = null;
            logger.Info("GetListOfApplications initiated, set to null");
            SlaveOwnerServerProxy.GetListOfRunningApplications(GetListOfApplicationsCallBack);
            return PollVariableFor10Seconds(ref listOfApplications, true);
        }

        public string GetPathToImage()
        {
            pathToImage = new string(@"C:\Users\kryst\Downloads\imagesFromPython\img.jpg");
            return PollVariableFor10Seconds(ref pathToImage, false);
        }
        
        private void GetListOfApplicationsCallBack(List<ApplicationInfo> appInfo)
        {
            listOfApplications = appInfo;
            logger.Info("listOfApplications set");
        }

        private void GetSlaveCallBack(Tuple<SlaveConnection, Port> slaveConnectionInfo)
        {
            this.slaveConnectionInfo = slaveConnectionInfo;
            logger.Info("slaveConnectionInfo set");
        }

        private void GetImageProducerConnectionInformationCallBack(Port port)
        {
            this.port = port;
            logger.Info("port set");
        }

        private string PollVariableFor10Seconds(ref object obj, bool JSONize) {
            var counter = 0;
            while (null == obj && counter < 100)
            {
                Thread.Sleep(100);
                counter++;
            }

            if (null == obj)
            {
                logger.Info("Polling finished after 10 seconds without result");
                return "Good luck next time";
            }

            logger.Info("Polling finished before 10 seconds ran out, returning " + (JSONize ? "" : "NON-") + "JSONized result");
            return (JSONize) ? JsonConvert.SerializeObject(obj, Formatting.Indented) : obj.ToString();
        }
    }
}