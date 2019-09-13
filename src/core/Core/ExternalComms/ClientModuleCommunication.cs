using System;
using System.Collections.Generic;
using System.Threading;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.model;
using message_based_communication.module;

namespace Core
{

    class ClientModuleCommunication : BaseClientModule
    {
        SlaveOwnerServermoduleProxy slaveOwner;

        public ClientModuleCommunication(ModuleType moduleType) : base(moduleType) { }

        public override string CALL_ID_PREFIX => "ClientModule";

        private SlaveOwnerServermoduleProxy SlaveOwnerServerProxy => slaveOwner ?? (slaveOwner = new SlaveOwnerServermoduleProxy(base.proxyHelper, this));

        public void GetSlaveConnection(PrimaryKey pk, ApplicationInfo appInfo, Action<SlaveConnection> callBack) => SlaveOwnerServerProxy.GetSlave(pk, appInfo, callBack);

        public string GetListOfApplications()
        {
            SlaveOwnerServerProxy.GetListOfRunningApplications(callBack);
            var counter = 0;
            while (null == callbackResult && counter < 100)
            {
                Thread.Sleep(100);
                counter++;
            }

            if (null == callbackResult)
            {
                return "Good luck next time";
            }
            var toReturn = callbackResult;
            callbackResult = null;
            return "JSONized result";
        }

        private List<ApplicationInfo> callbackResult = null;

        private void callBack(List<ApplicationInfo> appInfo)
        {
            callbackResult = appInfo;
        }
    }
}