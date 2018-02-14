using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using OneNoteOCRDll;

namespace TestServerConnection
{
    class StartClient
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for 5 seconds to connect to server");
            Thread.Sleep(5000);
            int counter = 0;
            try
            {
                while (true)
                {
                    var client = new TcpClient();
                    IPEndPoint Ip_End = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1289);
                    client.Connect(Ip_End);
                    var currentDat = DateTime.Now;
                    Console.WriteLine(String.Format("Current for client {0:HH:mm:ss:fff}", currentDat));
                    Console.WriteLine("Client connected");
                    if (client.Connected)
                    {
                        string imagePath = args[0];
                        if (args[0] == null)
                        {
                            Console.WriteLine("Read image path");
                            imagePath = Console.ReadLine();
                        }

                        SendText(client, "coin");
                        var s = ReceiveText(client);
                        if (s.Equals("ok"))
                        {
                            Console.WriteLine("Sending image");
                            SendImage(client, Image.FromFile(imagePath));
                        }
                        ReceiveTextFound(client);

                        counter++;
                        Console.WriteLine(String.Format("Sent {0} images", counter));
                        Console.WriteLine(String.Format("Sent {0:mmssfff} and closing connection", currentDat));
                        client.Close();
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot connect to server");
            }
        }

        public static void SendImage(TcpClient client, Image image)
        {
            NetworkStream serverStream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(serverStream);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] buffer = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(buffer, 0, buffer.Length);
            // write the size of the image buffer
            writer.Write(buffer.Length);
            // write the actual buffer
            writer.Write(buffer);
            serverStream.Flush();

        }

        public static void SendText(TcpClient client, string wantedText)
        {
            NetworkStream serverStream = client.GetStream();
            ImageConverter converter = new ImageConverter();
            Byte[] sendData = Encoding.ASCII.GetBytes(wantedText);
            serverStream.Write(sendData, 0, sendData.Length);
            serverStream.Flush();
        }

        public static string ReceiveText(TcpClient client)
        {
            NetworkStream serverStream = client.GetStream();
            byte[] inStream = new byte[9999999];
            Int32 bytes = serverStream.Read(inStream, 0, (int)client.ReceiveBufferSize);
            string responseData = System.Text.Encoding.ASCII.GetString(inStream, 0, bytes);
            serverStream.Flush();
            return responseData;
        }

        public static void ReceiveTextFound(TcpClient client)
        {
            NetworkStream serverStream = client.GetStream();
            byte[] inStream = new byte[9999999];
            Int32 bytes = serverStream.Read(inStream, 0, (int)client.ReceiveBufferSize);

            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(inStream, 0, inStream.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            TextFound obj = (TextFound)binForm.Deserialize(memStream);
            Console.WriteLine("Received");
            Console.WriteLine(obj.SearchText);
            Console.WriteLine(obj.ImageText);
        }

    }
}
