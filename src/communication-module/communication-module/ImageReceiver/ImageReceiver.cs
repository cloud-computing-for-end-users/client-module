using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using custom_message_based_implementation.model;
using File = System.IO.File;

namespace Core.ImageReceiver
{
    public class ImageReceiver
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public const string ImageFileName = "img.jpg";
        private const string BUFFER_FILE_NAME = "buffer";
        private const int SLEEP_TIME = 10;
        public bool CancelLocal;
        private const int MAX_REVICE_BUFFER_SIZE = 100000;

        

        private readonly SlaveConnection connInfo;
        private readonly string filePath;
        private Socket receiver;

        public ImageReceiver(SlaveConnection connInfo, string filePath)
        {
            this.connInfo = connInfo;
            this.filePath = filePath;
            CancelLocal = false;

            PrepareFilePath();
        }

        public void StartImageReceivingThread()
        {
            Logger.Info("StartImageReceivingThread initiated");

            var t = new Thread(
                () =>
                {
                    try
                    {
                        StartReceivingImages();
                    }
                    catch (Exception e)
                    {
                        receiver.Close();
                        if (CancelLocal)
                        {
                            Logger.Info("Image Receiver thread cancelled properly, socket closed");
                        }
                        else
                        {
                            // todo
                            Logger.Error("Something bad happened");
                            Logger.Debug(e);
                        }
                    }
                }) { IsBackground = true};
            t.Start();
        }

        private void PrepareFilePath()
        {
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }

            Logger.Debug("Creating directory: " + filePath);
            Directory.CreateDirectory(filePath);
        }

        private void StartReceivingImages()
        {
            var ipAddress = IPAddress.Parse(connInfo.ConnectionInformation.IP.TheIP);
            var endPoint = new IPEndPoint(ipAddress, connInfo.ConnectToRecieveImagesPort.ThePort);

            receiver = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            receiver.Connect(endPoint);
            Logger.Info("Connection established with: " + receiver);
            Logger.Info("Starting to receive images");

            while (true)
            {
                FileStream _fs;

                if (CancelLocal)
                {
                    throw new Exception("CalcelLocal:true, throwing exception to stop image receiver");
                }
                var imageSizeBuffer = new byte[sizeof(int)];
                receiver.Receive(imageSizeBuffer);

                var imageDataSize = BitConverter.ToInt32(imageSizeBuffer, 0);
                Logger.Debug("Expecting image of: " + imageDataSize + "bytes");

                if (0 == imageDataSize)
                {
                    Logger.Info("Received an image size of 0 so skipping saving to file");
                    continue;
                }

                try
                {
                    _fs = new FileStream(filePath + BUFFER_FILE_NAME, FileMode.Create, FileAccess.Write);
                    Logger.Debug("Opened a FileStream");
                }
                catch (IOException e)
                {
                    Logger.Debug("IO Exception below caught, recovering after " + SLEEP_TIME + "ms");
                    // getting rid of the current received image
                    receiver.Receive(new byte[imageDataSize]);
                    Logger.Debug(e);
                    Thread.Sleep(SLEEP_TIME);
                    continue;
                }

                var totalReceivedBytes = 0;
                while (imageDataSize > totalReceivedBytes)
                {
                    var fileBuffer = new byte[MAX_REVICE_BUFFER_SIZE];

                    var bytesToRead = imageDataSize - totalReceivedBytes > MAX_REVICE_BUFFER_SIZE
                        ? MAX_REVICE_BUFFER_SIZE
                        : imageDataSize - totalReceivedBytes;
                    var receivedBytes = receiver.Receive(fileBuffer, 0, bytesToRead, SocketFlags.None);
                    totalReceivedBytes += receivedBytes;
                    Logger.Debug("Amount of received bytes so far: " + totalReceivedBytes);
                    _fs.Write(fileBuffer, 0, receivedBytes);
                }

                Logger.Debug("Data to receive " + imageDataSize + " and data received " + totalReceivedBytes);

                _fs.Flush(true);
                _fs.Close();

                Logger.Info("Saved an image received from Python process to " + BUFFER_FILE_NAME);
                try
                {
                    System.IO.File.Copy(filePath + BUFFER_FILE_NAME, filePath + ImageFileName, true);
                    Logger.Info("Copied buffer to " + ImageFileName);
                }
                catch (IOException e)
                {
                    Logger.Debug("IO Exception below caught, recovering after " + SLEEP_TIME + "ms");
                    Logger.Debug(e);
                    Thread.Sleep(SLEEP_TIME);
                }
            }
        }
    }
}
