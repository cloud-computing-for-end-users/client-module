using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using message_based_communication.model;

namespace image_receiver_test_temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = "10.152.212.11" },
                Port = new Port() { ThePort = 30303 }
            };

            StartImageReceivingThread(connInfo, @"C:\Users\kryst\Downloads\imagesFromPython\");
        }

        private static int MAX_REVICE_BUFFER_SIZE = 100000;
        public static void StartImageReceivingThread(ConnectionInformation connInfo, string filePath)
        {
            var t = new Thread(
                () => { ReceiveImages(connInfo, filePath); })
            { IsBackground = false };
            t.Start();
        }

        private static void ReceiveImages(ConnectionInformation connInfo, string filePath)
        {
            IPAddress ipAddr = IPAddress.Parse(connInfo.IP.TheIP);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, connInfo.Port.ThePort);

            var reciver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            reciver.Connect(endPoint);
            Console.WriteLine("Connection established with: " + reciver.ToString());

            //reciver.Listen(10)/*;*/
            //var connection = reciver.Accept();
            string fileName = string.Empty;

            var startTime = DateTime.Now;
            Console.WriteLine("Starting to recive images at: " + startTime);

            while (true)
            {
                //if(images_to_recive_before_breakup == counter)
                //{
                //    Console.WriteLine("Recived " + images_to_recive_before_breakup + " images in : " + DateTime.Now.Subtract(startTime));
                //    break;
                //}
                var imageSizeBuffer = new byte[sizeof(int)];

                reciver.Receive(imageSizeBuffer);

                var imageDataSize = BitConverter.ToInt32(imageSizeBuffer, 0);
                Console.WriteLine("Image size in byte from python: " + imageSizeBuffer.ToString());
                Console.WriteLine("Expecting image of: " + imageDataSize + "bytes");
                if (0 == imageDataSize)
                {
                    Console.WriteLine("Recived an image size of 0 so am skipping save to file");
                    continue;
                }

                //fileName = ++counter + ".jpg";
                fileName = "img.jpg";
                using (var fs = new FileStream(filePath + fileName, FileMode.Create, FileAccess.Write))
                {
                    while ((imageDataSize - MAX_REVICE_BUFFER_SIZE) > 0)
                    {
                        Console.WriteLine("Remaining data to recive: " + imageDataSize);

                        byte[] fileBuffer = new byte[MAX_REVICE_BUFFER_SIZE];

                        imageDataSize -= MAX_REVICE_BUFFER_SIZE;
                        //read maxReciveSize
                        reciver.Receive(fileBuffer);
                        fs.Write(fileBuffer);
                    }

                    if (imageDataSize > 0)
                    {

                        var imageBuffer = new byte[imageDataSize];
                        reciver.Receive(imageBuffer);
                        imageDataSize = 0;
                        fs.Write(imageBuffer, 0, imageBuffer.Length);
                    }

                    Console.WriteLine("Remaining data to recive: " + imageDataSize);

                    fs.Flush();
                    fs.Close();
                }

                Console.WriteLine("Saved a file that was recived from python");
                break;
            }
        }
    }
}
