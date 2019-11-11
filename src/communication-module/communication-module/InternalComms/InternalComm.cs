using System;
using Core.Exceptions;
using Core.ExternalComms;
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
            // DevUtils
            internalConnection.On<string, string>("EstablishCGIConnection", param => "Established");
            // EntryView
            internalConnection.On<string, string>("Login", param =>
            {
                try { return externalCommunication.Login(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("CreateAccount", param =>
            {
                try { return externalCommunication.CreateAccount(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            // AppView
            internalConnection.On<string, string>("GetListOfApplications", param =>
            {
                try { return externalCommunication.GetListOfApplications(); } catch (PollingException e) { return e.Handle(); } catch(Exception e) { return Handle(e); }
            });
            // FileView
            internalConnection.On<string, string>("GetListOfFiles", param =>
            {
                try { return externalCommunication.GetListOfFiles(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("UploadFile", param =>
            {
                try { return externalCommunication.UploadFile(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("DownloadFile", param =>
            {
                try { return externalCommunication.DownloadFile(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("RenameFile", param =>
            {
                try { return externalCommunication.RenameFile(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("RemoveFile", param =>
            {
                try { return externalCommunication.RemoveFile(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("TellSlaveToFetchFile", param =>
            {
                try { return externalCommunication.TellSlaveToFetchFile(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("SaveFilesAndTerminate", param =>
            {
                try { return externalCommunication.SaveFilesAndTerminate(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            // Slave
            internalConnection.On<string, string>("GetImagesFromSlave", param =>
            {
                try { return externalCommunication.GetImagesFromSlave(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("MouseDown", param =>
            {
                try { return externalCommunication.MouseDown(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("MouseUp", param =>
            {
                try { return externalCommunication.MouseUp(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("KeyDown", param =>
            {
                try { return externalCommunication.KeyDown(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
            internalConnection.On<string, string>("KeyUp", param =>
            {
                try { return externalCommunication.KeyUp(param); } catch (PollingException e) { return e.Handle(); } catch (Exception e) { return Handle(e); }
            });
        }

        private static string Handle(Exception e)
        {
            Logger.Debug(e);
            return "(Exception) General exception occured, refer to log";
        }
    }
}
