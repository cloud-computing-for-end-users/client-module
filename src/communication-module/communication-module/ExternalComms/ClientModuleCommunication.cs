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
        }

        public string GetImagesFromSlave(PrimaryKey pk, ApplicationInfo appInfo)
        {
            CancelCurrentImageReceiver();

            SlaveOwnerHandler.GetSlaveConnectionInfo(pk, appInfo);

            var key = SlaveControllerHandler.ConnectToSlave(SlaveOwnerHandler.SlaveConnectionInfo, ModuleType, _forSelf, this);

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
            var parameters = JsonConvert.DeserializeObject<MouseDownParamsWrapper>(parametersInJson);
            Logger.Debug("MouseDown; XinPercent: " + parameters.XinPercent + "; YinPercent: " + parameters.YinPercent + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseDown(parameters);
        }
        
        public string MouseUp(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<MouseUpParamsWrapper>(parametersInJson);
            Logger.Debug("MouseUp; XinPercent: " + parameters.XinPercent + "; YinPercent: " + parameters.YinPercent + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseUp(parameters);
        }

        public string MouseMove(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<MouseMoveParamsWrapper>(parametersInJson);
            Logger.Debug("MouseMove; XinPercent: " + parameters.XinPercent + "; YinPercent: " + parameters.YinPercent + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseMove(parameters);
        }

        public string MouseScroll(string parametersInJson)
        {
            var parameters = JsonConvert.DeserializeObject<MouseScrollParamsWrapper>(parametersInJson);
            Logger.Debug("MouseScroll; Scroll amount X: " + parameters.ScrollAmountX + "; Scroll amount Y: " + parameters.ScrollAmountY + "; Key: " + parameters.Key);
            return SlaveControllerHandler.MouseScroll(parameters);
        }

        private void StartImageReceiving(string key, string imagePath)
        {
            Logger.Info("StartImageReceivingThread initiated");

            ImageReceiver.ImageReceiver.CancelLocal = false;
            ImageReceiver.ImageReceiver.StartImageReceivingThread(new ConnectionInformation
            {
                IP = SlaveOwnerHandler.SlaveConnectionInfo.Item1.IP,
                Port = SlaveControllerHandler.SlaveProxies[key].Port
            }, imagePath);
        }

        private void CancelCurrentImageReceiver()
        {
            ImageReceiver.ImageReceiver.CancelLocal = true;
            Thread.Sleep(5000); // todo give up instead
            Logger.Info("Gave up time to cancel thread; line above should say \"Image Receiver cancelled\"");
        }
    }
}