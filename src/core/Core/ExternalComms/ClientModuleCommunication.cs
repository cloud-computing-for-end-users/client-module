using System;
using System.Collections.Generic;
using System.Threading;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.model;
using message_based_communication.module;
using Newtonsoft.Json;

namespace Core
{

    class ClientModuleCommunication : BaseClientModule
    {
        SlaveOwnerServermoduleProxy slaveOwner;
        private object listOfApplications;
        private object pathToImage;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ClientModuleCommunication(ModuleType moduleType) : base(moduleType) { }

        public override string CALL_ID_PREFIX => "ClientModule";

        private SlaveOwnerServermoduleProxy SlaveOwnerServerProxy => slaveOwner ?? (slaveOwner = new SlaveOwnerServermoduleProxy(base.proxyHelper, this));

        public void GetSlaveConnection(PrimaryKey pk, ApplicationInfo appInfo, Action<SlaveConnection> callBack) => SlaveOwnerServerProxy.GetSlave(pk, appInfo, callBack);

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