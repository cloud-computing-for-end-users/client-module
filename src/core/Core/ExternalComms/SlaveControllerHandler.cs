using System;
using System.Collections.Generic;
using System.Text;
using client_slave_message_communication.encoding;
using client_slave_message_communication.proxy;
using Core.Model;
using custom_message_based_implementation.model;
using message_based_communication.connection;
using message_based_communication.model;
using message_based_communication.module;

namespace Core.ExternalComms
{
    internal class SlaveControllerHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Tuple<int, int> _widthHeightTuple;
        private int _keyForCallback;
        private Port _port;

        // todo figure out the imagePath
        private const string ImagePath = @"C:\Users\kryst\Downloads\imagesFromPython\";

        // todo key
        internal Dictionary<int, SlaveInfo> SlaveProxies;

        internal SlaveControllerHandler()
        {
            SlaveProxies = new Dictionary<int, SlaveInfo>();
        }

        internal int ConnectToSlave(Tuple<SlaveConnection, Port> slaveOwnerConnectionInfo, ModuleType moduleType, ConnectionInformation forSelf, BaseCommunicationModule that)
        {
            ProxyHelper proxyHelper = new ProxyHelper();
            proxyHelper.Setup(new ConnectionInformation() { IP = slaveOwnerConnectionInfo.Item1.IP, Port = slaveOwnerConnectionInfo.Item1.Port },
                slaveOwnerConnectionInfo.Item2, moduleType, new ConnectionInformation() { IP = forSelf.IP, Port = new Port() { ThePort = forSelf.Port.ThePort + 69 } }, that, new CustomEncoding());
            Logger.Info("ProxyHelper setup done");

            Logger.Debug("Slave proxy module ID: " + proxyHelper.ModuleID.ID);
            // todo key?
            var key = slaveOwnerConnectionInfo.GetHashCode();
            Logger.Debug("Slave Owner Connection Info Hash: " + key);
            SlaveProxies.Add(key, new SlaveInfo{SlaveProxy = new SlaveProxy(proxyHelper, that)});
            return key;
        }

        internal void Handshake(int SlaveProxyKey, PrimaryKey pk)
        {
            _widthHeightTuple = null;
            Logger.Info("Handshake initiated, " + nameof(_widthHeightTuple) + " set to null");
            Logger.Debug("Slave Proxy Key: " + SlaveProxyKey);
            _keyForCallback = SlaveProxyKey;
            SlaveProxies[SlaveProxyKey].SlaveProxy.Handshake(SlaveHandshakeCallback, pk);
            GeneralHandler.PollVariableFor10Seconds(ref _widthHeightTuple);
        }

        internal string GetImageProducerConnectionInformation(int SlaveProxyKey)
        {
            if (_port != null) return ImagePathForCurrentSlave();
            Logger.Info("GetImageProducerConnectionInformation initiated");
            Logger.Debug("Slave Proxy Key: " + SlaveProxyKey);
            _keyForCallback = SlaveProxyKey;
            SlaveProxies[SlaveProxyKey].SlaveProxy.GetImageProducerConnectionInformation(GetImageProducerConnectionInformationCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _port);
            return ImagePathForCurrentSlave();
        }

        private string ImagePathForCurrentSlave()
        {
            // todo figure out the imagePath
            return ImagePath + _port.ThePort + "\\";
        }

        // Callbacks
        private void SlaveHandshakeCallback(Tuple<int, int> widthHeightTuple)
        {
            var (width, height) = widthHeightTuple;
            SlaveProxies[_keyForCallback].AppDimensions = new AppDimensions(){Width = width, Height = height};
            Logger.Info("AppDimensions set in callback");
            // todo remove below if possible, has to be set now for polling check
            _widthHeightTuple = widthHeightTuple;
        }

        private void GetImageProducerConnectionInformationCallBack(Port port)
        {
            SlaveProxies[_keyForCallback].Port = port;
            Logger.Info("Port set in callback");
            // todo remove below if possible, has to be set now for polling check
            _port = port;
        }
    }
}
