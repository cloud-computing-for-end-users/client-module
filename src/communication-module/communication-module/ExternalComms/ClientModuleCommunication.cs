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

        
        private readonly ConnectionInformation _forSelf;
        
        public override string CALL_ID_PREFIX => "ClientModule";

        public ClientModuleCommunication(ModuleType moduleType, ConnectionInformation forSelf) : base(moduleType)
        {
            this._forSelf = forSelf;
        }

        public override void Setup(ConnectionInformation baseRouterModule, Port baseRouterRegistrationPort,
            ConnectionInformation forSelf, Encoding customEncoding)
        {
            base.Setup(baseRouterModule, baseRouterRegistrationPort, forSelf, customEncoding);
            SlaveOwnerHandler = new SlaveOwnerHandler(proxyHelper, this);
            SlaveControllerHandler = new SlaveControllerHandler();
            DatabaseHandler = new DatabaseHandler(proxyHelper, this);
        }

        public string GetImagesFromSlave(PrimaryKey pk, ApplicationInfo appInfo)
        {
            CancelCurrentImageReceiver();

            SlaveOwnerHandler.GetSlaveConnectionInfo(pk, appInfo);

            var key = SlaveControllerHandler.ConnectToSlave(SlaveOwnerHandler.Slave, ModuleType, _forSelf, this);

            SlaveControllerHandler.Handshake(key, pk);

            var imagePath = SlaveControllerHandler.GetImageProducerConnectionInformation(key);

            StartImageReceiving(key, imagePath);

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
            return GeneralHandler.ReturnAsJSON(SlaveOwnerHandler.ListOfApplications);
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

        private void StartImageReceiving(string key, string imagePath)
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