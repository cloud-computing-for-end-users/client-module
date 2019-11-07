using System.Threading;
using Core.ImageReceiver;
using custom_message_based_implementation.model;
using message_based_communication.encoding;
using message_based_communication.model;
using message_based_communication.module;
using Newtonsoft.Json;

namespace Core.ExternalComms
{
    public class ClientModuleCommunication : BaseClientModule
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private SlaveOwnerHandler SlaveOwnerHandler { get; set; }
        private DatabaseHandler DatabaseHandler { get; set; }
        private SlaveControllerHandler SlaveControllerHandler { get; set; }
        private FileHandler FileHandler { get; set; }


        private readonly ConnectionInformation _forSelf;
        
        public override string CALL_ID_PREFIX => "ClientModule";

        public ClientModuleCommunication(ModuleType moduleType, ConnectionInformation forSelf) : base(moduleType)
        {
            this._forSelf = forSelf;
        }

        public override void Setup(ConnectionInformation baseRouterModule, Port baseRouterRegistrationPort,
            ConnectionInformation forSelf, Encoding customEncoding)
        {
            Logger.Debug("Setting up handlers");
            base.Setup(baseRouterModule, baseRouterRegistrationPort, forSelf, customEncoding);

            SlaveOwnerHandler = new SlaveOwnerHandler(proxyHelper, this);
            Logger.Debug("Setup of SlaveOwnerHandler done");
            SlaveControllerHandler = new SlaveControllerHandler();
            Logger.Debug("Setup of SlaveControllerHandler done");
            DatabaseHandler = new DatabaseHandler(proxyHelper, this);
            Logger.Debug("Setup of DatabaseHandler done");
            FileHandler = new FileHandler(proxyHelper, this);
            Logger.Debug("Setup of FileHandler done");
        }

        public string GetImagesFromSlave(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<GetImagesFromSlaveWrapper>(parametersInJson);
            Logger.Debug("GetImagesFromSlave; PrimaryKey: " + parameters.PrimaryKey + "; ApplicationName: " + parameters.ApplicationName + "; ApplicationVersion: " + parameters.ApplicationVersion + "; OS: " + parameters.RunningOnOperatingSystem);

            var pk = new PrimaryKey {TheKey = parameters.PrimaryKey};
            var appInfo = new ApplicationInfo
            {
                ApplicationName = parameters.ApplicationName, 
                ApplicationVersion = parameters.ApplicationVersion,
                RunningOnOperatingSystem = parameters.RunningOnOperatingSystem
            };

            CancelCurrentImageReceiver();

            SlaveOwnerHandler.GetSlaveConnectionInfo(pk, appInfo);

            var key = SlaveControllerHandler.ConnectToSlave(SlaveOwnerHandler.Slave, ModuleType, _forSelf, this);

            SlaveControllerHandler.Handshake(key, pk);

            var imagePath = SlaveControllerHandler.ImagePathForCurrentSlave(SlaveOwnerHandler.Slave.SlaveConnection.ConnectToRecieveImagesPort);

            StartImageReceiving(imagePath);

            return GeneralHandler.ReturnAsJSON(new InitializeSlaveAppWindowWrapper
            {
                SlaveKey = key,
                PathToImages = imagePath + ImageReceiver.ImageReceiver.ImageFileName,
                WindowWidth = SlaveControllerHandler.SlaveProxies[key].AppDimensions.Width,
                WindowHeight = SlaveControllerHandler.SlaveProxies[key].AppDimensions.Height
            });
        }

        public string GetListOfApplications()
        {
            Logger.Debug("GetListOfApplications;");
            return SlaveOwnerHandler.GetListOfApplications();
        }

        public string MouseDown(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<MouseUpAndDownParamsWrapper>(parametersInJson);
            Logger.Debug("MouseDown; Button: " + parameters.Button + "; XinPercent: " + parameters.XinPercent + "; YinPercent: " + parameters.YinPercent + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseAction(parameters, true);
        }
        
        public string MouseUp(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<MouseUpAndDownParamsWrapper>(parametersInJson);
            Logger.Debug("MouseUp; Button: " + parameters.Button + "; XinPercent: " + parameters.XinPercent + "; YinPercent: " + parameters.YinPercent + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseAction(parameters, false);
        }

        public string Login(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<EmailAndPasswordWrapper>(parametersInJson);
            // todo do not log personal info
            Logger.Debug("Login; Email: " + parameters.Email + "; Password: " + parameters.Password);
            return DatabaseHandler.Login(parameters);
        }

        public string CreateAccount(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<EmailAndPasswordWrapper>(parametersInJson);
            // todo do not log personal info
            Logger.Debug("CreateAccount; Email: " + parameters.Email + "; Password: " + parameters.Password);
            return DatabaseHandler.CreateAccount(parameters);
        }

        public string GetListOfFiles(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<PrimaryKeyWrapper>(parametersInJson);
            Logger.Debug("GetListOfFiles; Primary key: " + parameters.PrimaryKey);
            return FileHandler.GetListOfFiles(parameters);
        }

        public string UploadFile(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<PrimaryKeyAndFileWrapper>(parametersInJson);
            Logger.Debug("UploadFile; Primary key: " + parameters.PrimaryKey + "; File name: " + parameters.FileName);
            return FileHandler.UploadFile(parameters);
        }

        public string DownloadFile(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<PrimaryKeyAndFileWrapper>(parametersInJson);
            Logger.Debug("DownloadFile; Primary key: " + parameters.PrimaryKey + "; File name: " + parameters.FileName);
            return FileHandler.DownloadFile(parameters);
        }

        public string RenameFile(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<RenameFileWrapper>(parametersInJson);
            Logger.Debug("RenameFile; Primary key: " + parameters.PrimaryKey + "; Old file name: " + parameters.OldFileName + "; Old file name: " + parameters.NewFileName);
            return FileHandler.RenameFile(parameters);
        }

        public string RemoveFile(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<PrimaryKeyAndFileWrapper>(parametersInJson);
            Logger.Debug("RemoveFile; Primary key: " + parameters.PrimaryKey + "; File name: " + parameters.FileName);
            return FileHandler.RemoveFile(parameters);
        }

        public string TellSlaveToFetchFile(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<SlaveKeyAndFileWrapper>(parametersInJson);
            Logger.Debug("TellSlaveToFetchFile; Slave key: " + parameters.SlaveKey + "; File name: " + parameters.FileName);
            return SlaveControllerHandler.TellSlaveToFetchFile(parameters);
        }

        public string SaveFilesAndTerminate(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<SlaveKeyWrapper>(parametersInJson);
            Logger.Debug("SaveFilesAndTerminate; Slave key: " + parameters.SlaveKey);
            return SlaveControllerHandler.SaveFilesAndTerminate(parameters);
        }

        private void StartImageReceiving(string imagePath)
        {
            Logger.Info("StartImageReceivingThread initiated");

            ImageReceiver.ImageReceiver.CancelLocal = false;
            
            ImageReceiver.ImageReceiver.StartImageReceivingThread(SlaveOwnerHandler.Slave.SlaveConnection, imagePath);
        }

        private void CancelCurrentImageReceiver()
        {
            ImageReceiver.ImageReceiver.CancelLocal = true;
            Thread.Sleep(5000); // todo give up instead
            Logger.Info("Gave up time to cancel thread; line above should say \"Image Receiver cancelled\"");
        }
    }
}