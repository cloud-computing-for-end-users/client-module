using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using message_based_communication.model;

namespace Core.ImageReceiver
{
    public class ImageReceiver
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public const string ImageFileName = "img.jpg";
        private const string BUFFER_FILE_NAME = "buffer";
        private const int SLEEP_TIME = 1000;
        public static bool CancelLocal;

        private static FileStream _fs;

        public static void StartImageReceivingThread(ConnectionInformation connInfo, string filePath)
        {
            PrepareFilePath(filePath);

            var t = new Thread(
                () =>
                {
                    try
                    {
                        StartReceivingImages(connInfo, filePath);
                    } catch (Exception e) { Logger.Debug(e); }
                }) { IsBackground = true};
            t.Start();
        }

        private static void PrepareFilePath(string filePath)
        {
            foreach (var entry in Directory.GetDirectories(filePath + @"..\"))
            {
                Logger.Debug("Removing directory: " + entry);
                Directory.Delete(entry, true);
            }

            Logger.Debug("Creating directory: " + filePath);
            Directory.CreateDirectory(filePath);
        }

        private static void StartReceivingImages(ConnectionInformation connInfo, string filePath)
        {
            IPAddress ipAddr = IPAddress.Parse(connInfo.IP.TheIP);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, connInfo.Port.ThePort);

            var receiver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            receiver.Connect(endPoint);
            Logger.Info("Connection established with: " + receiver);
            Logger.Info("Starting to receive images");

            while (true)
            {
                if (CancelLocal)
                {
                    receiver.Close();
                    Logger.Info("Image Receiver cancelled");
                    return;
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

                byte[] fileBuffer = new byte[imageDataSize];
                var receivedBytes = receiver.Receive(fileBuffer);
                Logger.Debug("Data to receive " + imageDataSize + " and data received " + receivedBytes);

                _fs.Write(fileBuffer);
                _fs.Flush(true);
                _fs.Close();

                Logger.Info("Saved an image received from Python process to " + BUFFER_FILE_NAME);
                try
                {
                    File.Copy(filePath + BUFFER_FILE_NAME, filePath + ImageFileName, true);
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
