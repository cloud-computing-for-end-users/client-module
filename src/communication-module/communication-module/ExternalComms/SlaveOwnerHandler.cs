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

        private SlaveConnection _slaveConnectionInfo;
        private List<ApplicationInfo> _listOfApplications;

        internal SlaveConnection SlaveConnectionInfo => _slaveConnectionInfo;
        internal List<ApplicationInfo> ListOfApplications
        {
            get
            {
                GetListOfApplications();
                return _listOfApplications;
            }
        }

        internal SlaveOwnerHandler(ProxyHelper ph, ClientModuleCommunication cmm)
        {
            this.SlaveOwnerServerProxy = new SlaveOwnerServermoduleProxy(ph, cmm);
        }

        internal void GetSlaveConnectionInfo(PrimaryKey pk, ApplicationInfo appInfo)
        {
            _slaveConnectionInfo = null;
            Logger.Info("GetSlave initiated, " + nameof(_slaveConnectionInfo) + " set to null");
            SlaveOwnerServerProxy.GetSlave(pk, appInfo, GetSlaveCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _slaveConnectionInfo);
            /*_slaveConnectionInfo = new Tuple<SlaveConnection, Port>(new SlaveConnection()
            {
                IP = new IP() { TheIP = "192.168.137.149".Trim() },
                Port = new Port() { ThePort = 10142 }
                //IP = new IP(){TheIP = "127.0.0.1"}, Port = new Port() { ThePort = 10142 }
            }, new Port(){ThePort = 10143}); //TODO DELETE THIS IS ONLY FOR DEBUGGING
            */
        }

        internal void GetListOfApplications()
        {
            _listOfApplications = null;
            Logger.Info("GetListOfApplications initiated, set to null");
            SlaveOwnerServerProxy.GetListOfRunningApplications(GetListOfApplicationsCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _listOfApplications);
        }

        // Callbacks

        private void GetSlaveCallBack(SlaveConnection slaveConnectionInfo)
        {
            _slaveConnectionInfo = slaveConnectionInfo;
            Logger.Info(this._slaveConnectionInfo.GetType().Name + " " + nameof(this._slaveConnectionInfo) + " set: " + _slaveConnectionInfo);
            Logger.Info("With data: {" + _slaveConnectionInfo.IP + ", " + _slaveConnectionInfo.Port + ", " + _slaveConnectionInfo.RegistrationPort + ", " + _slaveConnectionInfo.ConnectToRecieveImagesPort +" }");
        }

        private void GetListOfApplicationsCallBack(List<ApplicationInfo> appInfo)
        {
            _listOfApplications = appInfo;
            Logger.Info(this._listOfApplications.GetType().Name + " " + nameof(this._listOfApplications) + " set");
        }
    }
}
