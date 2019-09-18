using ElectronCgi.DotNet;

namespace Core
{
    class InternalComms
    {
        private Connection internalConnection;
        private ClientModuleCommunication externalCommunication;

        public InternalComms(ClientModuleCommunication externalCommunication)
        {
            internalConnection = new ConnectionBuilder().WithLogging().Build();
            this.externalCommunication = externalCommunication;
            RegisterMethodsAndCallbacks();
            internalConnection.Listen();   
        }

        public void RegisterMethodsAndCallbacks()
        {
            internalConnection.On<string, string>("GetPathToImage", name => externalCommunication.GetPathToImage());            
            internalConnection.On<string, string>("GetListOfApplications", name => externalCommunication.GetListOfApplications());
        }
    }
}
