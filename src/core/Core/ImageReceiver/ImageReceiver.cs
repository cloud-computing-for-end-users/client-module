using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using message_based_communication.model;

namespace Core
{
    public class ImageReceiver
    {
        private static int MAX_REVICE_BUFFER_SIZE = 100000;

        public static void StartImageReceivingThread(ConnectionInformation connInfo, string filePath)
        {
            var t = new Thread(
                () => { ReceiveImages(connInfo, filePath); })
            { IsBackground = true };
            t.Start();
        }
        private static void ReceiveImages(ConnectionInformation connInfo, string filePath)
        {
            IPAddress ipAddr = IPAddress.Parse(connInfo.IP.TheIP);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, connInfo.Port.ThePort);

            var reciver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            reciver.Connect(endPoint);
            Console.WriteLine("Connection established with: " + reciver.ToString());
            ReceiveImages(reciver, filePath);
        }

        private static void ReceiveImages(Socket reciver, string filePath)
        {
            var imageSizeBuffer = new byte[sizeof(int)];

            reciver.Receive(imageSizeBuffer);
            var imageDataSize = BitConverter.ToInt32(imageSizeBuffer, 0);

            Console.WriteLine("Image size in byte from python: " + imageSizeBuffer.ToString());

            if (0 == imageDataSize)
            {
                Console.WriteLine("Recived an image size of 0 so am skipping save to file");
                //recursion
                ReceiveImages(reciver, filePath);
            }

            //fileName = ++counter + ".jpg";
            string fileName = "img.jpg";
            using (var fs = new FileStream(filePath + fileName, FileMode.Create, FileAccess.Write))
            {
                int totalRecivedData = 0;
                byte[] fileBuffer = null;
                while (imageDataSize > totalRecivedData)
                {
                    Console.WriteLine("Remaining data to recive: " + imageDataSize);
                    fileBuffer = new byte[MAX_REVICE_BUFFER_SIZE];

                    //read maxReciveSize
                    int bytesToRead = imageDataSize - totalRecivedData > MAX_REVICE_BUFFER_SIZE ? MAX_REVICE_BUFFER_SIZE : imageDataSize - totalRecivedData;
                    var recivedBytes = reciver.Receive(fileBuffer, 0, bytesToRead, SocketFlags.None);
                    totalRecivedData += recivedBytes;
                    Console.WriteLine("Amount of recived bytes so far: " + totalRecivedData);
                    fs.Write(fileBuffer, 0, recivedBytes);
                }

                fs.Flush();
                fs.Close();
            }

            Console.WriteLine("Saved a file that was recived from python");

            //recursion
            ReceiveImages(reciver, filePath);
        }
    }
}
