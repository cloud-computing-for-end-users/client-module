using System;
using Core.Exceptions;
using Core.ExternalComms;
using custom_message_based_implementation.model;
using ElectronCgi.DotNet;

namespace Core.InternalComms
{
    class InternalComm
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Connection internalConnection;
        private readonly ClientModuleCommunication externalCommunication;

        public InternalComm(ClientModuleCommunication externalCommunication)
        {
            internalConnection = new ConnectionBuilder().WithLogging().Build();
            this.externalCommunication = externalCommunication;
            RegisterMethodsAndCallbacks();
            Logger.Info("Listening for method calls");
            internalConnection.Listen(); 
        }

        public void RegisterMethodsAndCallbacks()
        {
            Logger.Info("Registering callable methods from Electron");
            internalConnection.On<string, string>("GetListOfApplications", param =>
            {
                try { return externalCommunication.GetListOfApplications(); } catch (PollingException e) { return e.Handle(); } catch(Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("GetImagesFromSlave", param =>
            {
                try { return externalCommunication.GetImagesFromSlave(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("MouseDown", param =>
            {
                try { return externalCommunication.MouseDown(param); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("MouseUp", param =>
            {
                try { return externalCommunication.MouseUp(param); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("Login", param =>
            {
                try { return externalCommunication.Login(param); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("CreateAccount", param =>
            {
                try { return externalCommunication.CreateAccount(param); } catch (Exception e) { return Handle(e); }
            });
        }

        private static string Handle(Exception e)
        {
            Logger.Debug(e);
            return "(Exception) General exception occured, refer to log";
        }
    }
}
