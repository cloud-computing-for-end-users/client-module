﻿using System;
using System.Collections.Generic;
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
        private Port _port;

        // todo figure out the imagePath
        private const string IMAGE_PATH = @"C:\Users\kryst\Downloads\imagesFromPython\";

        // todo Key
        internal Dictionary<string, SlaveInfo> SlaveProxies;

        internal SlaveControllerHandler()
        {
            SlaveProxies = new Dictionary<string, SlaveInfo>();
        }

        internal string ConnectToSlave(Tuple<SlaveConnection, Port> slaveOwnerConnectionInfo, ModuleType moduleType, ConnectionInformation forSelf, BaseCommunicationModule that)
        {
            ProxyHelper proxyHelper = new ProxyHelper();
            proxyHelper.Setup(new ConnectionInformation() { IP = slaveOwnerConnectionInfo.Item1.IP, Port = slaveOwnerConnectionInfo.Item1.Port },
                slaveOwnerConnectionInfo.Item2, moduleType, new ConnectionInformation() { IP = forSelf.IP, Port = new Port() { ThePort = forSelf.Port.ThePort + 69 } }, that, new CustomEncoding());
            Logger.Info("ProxyHelper setup done");

            Logger.Debug("Slave proxy module ID: " + proxyHelper.ModuleID.ID);
            // todo Key?
            var key = GetDictionaryKey(slaveOwnerConnectionInfo);
            Logger.Debug("Slave Owner Connection Info Hash: " + key);
            SlaveProxies.Add(key, new SlaveInfo{SlaveProxy = new SlaveProxy(proxyHelper, that)});
            return key;
        }

        // todo make type for dictionary key that is a wrapper for string
        private static string GetDictionaryKey(Tuple<SlaveConnection, Port> slaveOwnerConnectionInfo)
        {
            return "" + slaveOwnerConnectionInfo.GetHashCode();
        }

        internal void Handshake(string slaveProxyKey, PrimaryKey pk)
        {
            _widthHeightTuple = null;
            Logger.Info("Handshake initiated, " + nameof(_widthHeightTuple) + " set to null");
            Logger.Debug("Slave Proxy Key: " + slaveProxyKey);
            _keyForCallback = slaveProxyKey;
            SlaveProxies[slaveProxyKey].SlaveProxy.Handshake(SlaveHandshakeCallback, pk);
            GeneralHandler.PollVariableFor10Seconds(ref _widthHeightTuple);
        }

        internal string GetImageProducerConnectionInformation(string slaveProxyKey)
        {
            if (_port != null) return ImagePathForCurrentSlave();
            Logger.Info("GetImageProducerConnectionInformation initiated");
            Logger.Debug("Slave Proxy Key: " + slaveProxyKey);
            _keyForCallback = slaveProxyKey;
            SlaveProxies[slaveProxyKey].SlaveProxy.GetImageProducerConnectionInformation(GetImageProducerConnectionInformationCallBack);
            GeneralHandler.PollVariableFor10Seconds(ref _port);
            return ImagePathForCurrentSlave();
        }

        internal string MouseDown(MouseDownParamsWrapper parameters)
        {
            try
            {
                // todo null callback
                // todo left mouse click x,y params
                /*
                SlaveProxies[parameters.Key].SlaveProxy.DoMouseAction(null, new MouseMoveAction()
                {
                    relativeScreenLocation = new RelativeScreenLocation()
                    {
                        FromLeft = new Percent()
                        {
                            ThePercentage = parameters.XinPercent
                        },
                        FromTop = new Percent()
                        {
                            ThePercentage = parameters.YinPercent
                        }
                    }
                });
                */
                SlaveProxies[parameters.Key].SlaveProxy.DoMouseAction(null, new LeftMouseDownAction());
            }
            catch (Exception e)
            {
                // todo remove this try/catch
                Logger.Debug("CAUGHT EXCEPTION");
                Logger.Debug(e);
            }
            // todo better return value; related to using callback
            return "Sent";
        }

        internal string MouseUp(MouseUpParamsWrapper parameters)
        {
            // todo null callback
            // todo left mouse click x,y params
            SlaveProxies[parameters.Key].SlaveProxy.DoMouseAction(null, new LeftMouseUpAction());
            // todo better return value; related to using callback
            return "Sent";
        }

        internal string MouseMove(MouseMoveParamsWrapper parameters)
        {
            // todo null callback
            // todo left mouse click x,y params
            SlaveProxies[parameters.Key].SlaveProxy.DoMouseAction(null, new MouseMoveAction()
            {
                RelativeScreenLocation = new RelativeScreenLocation()
                {
                    FromLeft = new Percent()
                    {
                        ThePercentage = parameters.XinPercent
                    },
                    FromTop = new Percent()
                    {
                        ThePercentage = parameters.YinPercent
                    }
                }
            });
            // todo better return value; related to using callback
            return "Sent";
        }

        internal string MouseScroll(MouseScrollParamsWrapper parameters)
        {
            // todo null callback
            // todo left mouse click x,y params
            SlaveProxies[parameters.Key].SlaveProxy.DoMouseAction(null, new MouseScrollAction()
            {
                ScrollAmountX = new ScrollAmount() { Amount = parameters.ScrollAmountX }, 
                ScrollAmountY = new ScrollAmount() { Amount = parameters.ScrollAmountY }
            });
            // todo better return value; related to using callback
            return "Sent";
        }

        private string ImagePathForCurrentSlave()
        {
            // todo figure out the imagePath
            return IMAGE_PATH + _port.ThePort + "\\";
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