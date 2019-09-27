using System;
using System.Collections.Generic;
using System.Threading;
using client_slave_message_communication.encoding;
using client_slave_message_communication.proxy;
using Core.ImageReceiver;
using custom_message_based_implementation.model;
using custom_message_based_implementation.proxy;
using message_based_communication.connection;
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

            Logger.Debug((null == SlaveOwnerHandler.SlaveConnectionInfo) + " " + (null == base.ModuleType) + " " +
                         (null == _forSelf) + " " + (null == this));
            var key = SlaveControllerHandler.ConnectToSlave(SlaveOwnerHandler.SlaveConnectionInfo, base.ModuleType,
                _forSelf, this);

            SlaveControllerHandler.Handshake(key, pk);

            var imagePath = SlaveControllerHandler.GetImageProducerConnectionInformation(key);

            StartImageReceiving(key, imagePath);

            return GeneralHandler.ReturnAsJSON(new PathAndWindowDimensionsWrapper
            {
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

            return "Success";
        }

        private void StartImageReceiving(int key, string imagePath)
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