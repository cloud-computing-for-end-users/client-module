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
        private static readonly bool DOING_CHUNKING = true;
        static void Main(string[] args)
        {
            var connInfo = new ConnectionInformation()
            {
                IP = new IP() { TheIP = "10.152.212.11" },
                Port = new Port() { ThePort = 30303 }
            };

            StartImageReceivingThread(connInfo, @"C:\Users\MSI\Downloads\imagesFromPython\");
        }

        private static int MAX_REVICE_BUFFER_SIZE = 100000;
        public static void StartImageReceivingThread(ConnectionInformation connInfo, string filePath)
        {
            var t = new Thread(
                () => { ReciveImages(connInfo, filePath); })
            { IsBackground = false };
            t.Start();
        }

        private static void ReciveImages(ConnectionInformation connInfo, string filePath)
        {
            IPAddress ipAddr = IPAddress.Parse(connInfo.IP.TheIP);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, connInfo.Port.ThePort);

            var reciver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            reciver.Connect(endPoint);
            Console.WriteLine("Connection established with: " + reciver.ToString());
            ReceiveImages(reciver, filePath, new byte[0]);
        }

        private static void ReceiveImages(Socket reciver, string filePath, byte[] excessData)
        {
            byte[] theExcessData = null;

            //reciver.Listen(10)/*;*/
            //var connection = reciver.Accept();
            string fileName = string.Empty;

            var startTime = DateTime.Now;
            Console.WriteLine("Starting to recive images at: " + startTime);

            //while (true)
            //{
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
                //recursion
                ReceiveImages(reciver, filePath, new byte[0]);
            }

            //fileName = ++counter + ".jpg";
            fileName = "img.jpg";
            using (var fs = new FileStream(filePath + fileName, FileMode.Create, FileAccess.Write))
            {

                if (false == DOING_CHUNKING)
                {
                    Console.WriteLine("Remaining data to recive: " + imageDataSize);
                    byte[] fileBuffer = new byte[imageDataSize];
                    var recivedBytes = reciver.Receive(fileBuffer);
                    Console.WriteLine("Number of recived bytes: " + recivedBytes);
                    fs.Write(fileBuffer);

                }
                else
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
                        Console.WriteLine("Amount of recived bytes: " + recivedBytes);
                        totalRecivedData += recivedBytes;
                        fs.Write(fileBuffer, 0, recivedBytes);
                    }

                    theExcessData = new byte[totalRecivedData - imageDataSize];
                    Array.Copy(fileBuffer, totalRecivedData - imageDataSize, theExcessData, 0, totalRecivedData - imageDataSize);
                    //if (imageDataSize > 0)
                    //{
                    //    var imageBuffer = new byte[imageDataSize];

                    //    var recivedBytes = reciver.Receive(imageBuffer);
                    //    Console.WriteLine("Amount of recived bytes: " + recivedBytes);

                    //    imageDataSize -= recivedBytes;
                    //    //imageDataSize = 0;
                    //    fs.Write(imageBuffer, 0, imageBuffer.Length);
                    //}

                    //Console.WriteLine("Remaining data to recive: " + imageDataSize);
                }


                fs.Flush();
                fs.Close();
            }

            Console.WriteLine("Saved a file that was recived from python");
            //break;
            //Console.ReadKey();
            //}

            //recursion
            ReceiveImages(reciver, filePath, theExcessData);
        }
    }
}
