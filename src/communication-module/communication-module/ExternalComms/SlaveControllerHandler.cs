using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using client_slave_message_communication.encoding;
using client_slave_message_communication.model;
using client_slave_message_communication.model.mouse_action;
using client_slave_message_communication.proxy;
using Core.ImageReceiver;
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
        private string _keyForCallback;
        private string _stateOfTerminate;
        private string _stateOfTellSlaveToFetchFile;

        // todo figure out the imagePath
        private static readonly string ImagePath = AppContext.BaseDirectory;

        // todo Key
        internal Dictionary<string, SlaveInfo> SlaveProxies;

        internal SlaveControllerHandler()
        {
            SlaveProxies = new Dictionary<string, SlaveInfo>();
        }

        internal (string key, string imagePath) ConnectToSlave(Slave slave, PrimaryKey pk, ModuleType moduleType, ConnectionInformation forSelf, BaseCommunicationModule that)
        {
            Logger.Info("Attempting to connect to slave with network settings: {ip: " + slave.SlaveConnection.ConnectionInformation.IP.TheIP + " comm port: " + slave.SlaveConnection.ConnectionInformation.Port.ThePort + " registration port: " + slave.SlaveConnection.RegistrationPort.ThePort + "}");

            //dynamically getting an available TCP port
            var selfPort = new Port();
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            selfPort.ThePort = port;

            var proxyHelper = new ProxyHelper();
            proxyHelper.Setup(new ConnectionInformation() { IP = slave.SlaveConnection.ConnectionInformation.IP, Port = slave.SlaveConnection.ConnectionInformation.Port },
                slave.SlaveConnection.RegistrationPort, moduleType, new ConnectionInformation() { IP = forSelf.IP, Port = selfPort /*{ ThePort = 0  *//* TODO does this work forSelf.Port.ThePort + 69 }*/ }, that, new CustomEncoding());
            Logger.Info("ProxyHelper setup done");

            Logger.Debug("Slave proxy module ID: " + proxyHelper.ModuleID.ID);
            // todo Key?
            var key = GetDictionaryKey(slave);
            Logger.Debug("Slave Owner Connection Info Hash: " + key);
            SlaveProxies.Add(key, new SlaveInfo { SlaveProxy = new SlaveProxy(proxyHelper, that) });

            Handshake(key, pk);

            var imagePath = ImagePathForCurrentSlave(slave.SlaveConnection.ConnectToRecieveImagesPort);

            SlaveProxies[key].ImageReceiver = new ImageReceiver.ImageReceiver(slave.SlaveConnection, imagePath);
            SlaveProxies[key].ImageReceiver.StartImageReceivingThread();

            return (key, imagePath);
        }

        // todo make type for dictionary key that is a wrapper for string
        private static string GetDictionaryKey(Slave slaveOwnerConnectionInfo)
        {
            return "" + slaveOwnerConnectionInfo.GetHashCode();
        }

        internal void Handshake(string slaveProxyKey, PrimaryKey pk)
        {
            _widthHeightTuple = null;
            Logger.Info("Handshake initiated, " + nameof(_widthHeightTuple) + " set to null");
            Logger.Debug("Slave Proxy Key: " + slaveProxyKey);
            Logger.Debug("Calling handshake on slave proxy, with primary key: " + pk?.TheKey);
            _keyForCallback = slaveProxyKey;
            SlaveProxies[slaveProxyKey].SlaveProxy.Handshake(SlaveHandshakeCallback, pk);

            GeneralHandler.PollVariableFor10Seconds(ref _widthHeightTuple);
        }

        internal string MouseAction(MouseUpAndDownParamsWrapper parameters, bool down)
        {
            var location = new RelativeScreenLocation()
            {
                FromLeft = new Percent()
                {
                    ThePercentage = parameters.XinPercent
                },
                FromTop = new Percent()
                {
                    ThePercentage = parameters.YinPercent
                }
            };

            BaseMouseAction action;
            if (down)
            {
                if (parameters.Button.Equals("Left"))
                {
                    action = new LeftMouseDownAction() { RelativeScreenLocation = location };
                }
                else
                {
                    action = new RightMouseDownAction() { RelativeScreenLocation = location };
                }
            }
            else
            {
                if (parameters.Button.Equals("Left"))
                {
                    action = new LeftMouseUpAction() { RelativeScreenLocation = location };
                }
                else
                {
                    action = new RightMouseUpAction() { RelativeScreenLocation = location };
                }
            }

            // todo null callback
            SlaveProxies[parameters.SlaveKey].SlaveProxy.DoMouseAction(null, action);
            // todo better return value; related to using callback
            return "Sent (MouseAction)";
        }

        private string FormatKeySpelling(KeyUpAndDownParamsWrapper param){
            switch(param.Key){
                case "Control":
                    return "ctrl";
                case "ArrowUp":
                    return "up";
                case "ArrowDown":
                    return "down";
                case "ArrowLeft":
                    return "left";
                case "ArrowRight":
                    return "Right";
                default:
                    return param.Key;
            }
        }
        internal string KeyAction(KeyUpAndDownParamsWrapper parameters, bool down)
        {
            if(parameters.Key ==  "Unidentified"){
                Logger.Debug("Skipping sending Unidentified key action");
                return "Skipped sending (KeyAction)";
            }
                
            parameters.Key = FormatKeySpelling(parameters);
            
            // todo null callback
            SlaveProxies[parameters.SlaveKey].SlaveProxy.DoKeyboardAction(null, parameters.Key, down);
            // todo better return value; related to using callback
            return "Sent (KeyAction)";
        }

        internal string TellSlaveToFetchFile(SlaveKeyAndFileWrapper parameters)
        {
            _stateOfTellSlaveToFetchFile = null;
            Logger.Debug("Telling slave with key " + parameters.SlaveKey + " to fetch file");
            SlaveProxies[parameters.SlaveKey].SlaveProxy.FetchRemoteFile(TellSlaveToFetchFileCallBack, parameters.FileName);
            return GeneralHandler.PollVariableFor10Seconds(ref _stateOfTellSlaveToFetchFile);
        }

        internal string SaveFilesAndTerminate(SlaveKeyWrapper parameters)
        {
            _stateOfTerminate = null;
            Logger.Info("SaveFilesAndTerminate initiated");
            
            Logger.Info("Cancelled image received thread; there should be \"Image Receiver cancelled\" somewhere after this log");
            SlaveProxies[parameters.SlaveKey].ImageReceiver.CancelLocal = true;
            
            SlaveProxies[parameters.SlaveKey].SlaveProxy.SaveFilesAndTerminate(SaveFilesAndTerminateCallBack);
            return GeneralHandler.PollVariableFor10Seconds(ref _stateOfTerminate);
        }

        public string ImagePathForCurrentSlave(Port port)
        {
            // todo figure out the imagePath
            return ImagePath + port.ThePort + "\\";
        }

        // Callbacks
        private void SlaveHandshakeCallback(Tuple<int, int> widthHeightTuple)
        {
            var (width, height) = widthHeightTuple;
            SlaveProxies[_keyForCallback].AppDimensions = new AppDimensions() { Width = width, Height = height };
            Logger.Info("AppDimensions (W: " + width + "; H: " + height + ") set in callback");
            // todo remove below if possible, has to be set now for polling check
            _widthHeightTuple = widthHeightTuple;
        }

        private void SaveFilesAndTerminateCallBack()
        {
            Logger.Info("(SaveFilesAndTerminateCallBack)");

            _stateOfTerminate = "Sent (SaveFilesAndTerminate)";
        }

        private void TellSlaveToFetchFileCallBack()
        {
            Logger.Info("(TellSlaveToFetchFileCallBack)");

            _stateOfTellSlaveToFetchFile = "Sent (TellSlaveToFetchFile)";
        }
    }
}
