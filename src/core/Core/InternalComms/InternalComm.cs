using Core.Exceptions;
using Core.ExternalComms;
using custom_message_based_implementation.model;
using ElectronCgi.DotNet;

namespace Core.InternalComms
{
    class InternalComm
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Connection internalConnection;
        private readonly ClientModuleCommunication externalCommunication;

        public InternalComm(ClientModuleCommunication externalCommunication)
        {
            internalConnection = new ConnectionBuilder().WithLogging().Build();
            this.externalCommunication = externalCommunication;
            RegisterMethodsAndCallbacks();
            logger.Info("Listening for method calls");
            internalConnection.Listen(); 
        }

        public void RegisterMethodsAndCallbacks()
        {
            logger.Info("Registering callable methods from Electron");
            internalConnection.On<string, string>("GetListOfApplications", param =>
            {
                try { return externalCommunication.GetListOfApplications(); } catch (PollingException e) { return e.Handle(); }
            });
            internalConnection.On<string, string>("GetImagesFromSlave", param =>
            {
                try { return externalCommunication.GetImagesFromSlave(new PrimaryKey(), new ApplicationInfo()); } catch (PollingException e) { return e.Handle(); }
            });
            internalConnection.On<string, string>("MoveMouse", param => externalCommunication.MouseDown(param));
        }
    }
}
