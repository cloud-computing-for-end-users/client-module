using System;
using System.Collections.Generic;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.connection;
using message_based_communication.model;

namespace Core.ExternalComms
{
    internal class SlaveOwnerHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private SlaveOwnerServermoduleProxy SlaveOwnerServerProxy { get; }

        private Slave _slave;
        private List<ApplicationInfo> _listOfApplications;

        internal Slave Slave => _slave;
        
        internal SlaveOwnerHandler(ProxyHelper ph, ClientModuleCommunication cmm)
        {
            this.SlaveOwnerServerProxy = new SlaveOwnerServermoduleProxy(ph, cmm);
        }

        internal void GetSlaveConnectionInfo(PrimaryKey pk, ApplicationInfo appInfo)
        {
            _slave= null;
            Logger.Info("GetSlave initiated, " + nameof(_slave) + " set to null");
            SlaveOwnerServerProxy.GetSlave(pk, appInfo, GetSlaveCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _slave);
            /*_slaveConnectionInfo = new Tuple<SlaveConnection, Port>(new SlaveConnection()
            {
                IP = new IP() { TheIP = "192.168.137.149".Trim() },
                Port = new Port() { ThePort = 10142 }
                //IP = new IP(){TheIP = "127.0.0.1"}, Port = new Port() { ThePort = 10142 }
            }, new Port(){ThePort = 10143}); //TODO DELETE THIS IS ONLY FOR DEBUGGING
            */
        }

        internal string GetListOfApplications()
        {
            _listOfApplications = null;
            Logger.Info("GetListOfApplications initiated, set to null");
            SlaveOwnerServerProxy.GetListOfRunningApplications(GetListOfApplicationsCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _listOfApplications);
            return GeneralHandler.ReturnAsJSON(_listOfApplications);
        }

        // Callbacks

        private void GetSlaveCallBack(Slave slave)
        {
            _slave= slave;
            Logger.Info(this._slave.GetType().Name + " " + nameof(this._slave) + " set: " + _slave);
            Logger.Info("With data: {" + _slave.SlaveConnection.ConnectionInformation.IP + ", " + _slave.SlaveConnection.ConnectionInformation.Port + ", " + _slave.SlaveConnection.RegistrationPort + ", " + _slave.SlaveConnection.ConnectToRecieveImagesPort +" }");
        }

        private void GetListOfApplicationsCallBack(List<ApplicationInfo> appInfo)
        {
            _listOfApplications = appInfo;
            Logger.Info(this._listOfApplications.GetType().Name + " " + nameof(this._listOfApplications) + " set");
        }
    }
}
